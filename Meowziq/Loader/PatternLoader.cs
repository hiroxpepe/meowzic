﻿/*
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Meowziq.Core;
using static Meowziq.Env;

namespace Meowziq.Loader {
    /// <summary>
    /// loader class for Pattern object.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class PatternLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// creates a list of Core.Pattern objects.
        /// </summary>
        public static List<Core.Pattern> Build(Stream target) {
            return loadJson(target).PatternArray.Select(selector: x => convertPattern(pattern: x)).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// converts a Pattern object to a Core.Pattern object.
        /// </summary>
        static Core.Pattern convertPattern(Pattern pattern) {
            getCountBeatLength(pattern);
            return new Core.Pattern(name: pattern.Name, meas_list: convertMeasList(pattern_string: pattern.Data));
        }

        /// <summary>
        /// converts a pattern string to a list of Core.Meas objects.
        /// </summary>
        static List<Meas> convertMeasList(string pattern_string) {
            pattern_string = interpretModePrefix(pattern_string);
            return pattern_string.GetMeasStringArray().Select(selector: x => new Meas(convertSpanList(meas_string: x))).ToList();
        }

        /// <summary>
        /// interprets the church mode abbreviations.
        /// </summary>
        static string interpretModePrefix(string target) {
            target = target.Replace(":l", ":Lyd");
            target = target.Replace(":i", ":Ion");
            target = target.Replace(":m", ":Mix");
            target = target.Replace(":d", ":Dor");
            target = target.Replace(":a", ":Aeo");
            target = target.Replace(":p", ":Phr");
            target = target.Replace(":o", ":Loc");
            return target;
        }

        /// <summary>
        /// converts a meas string to a list of Span objects.
        /// </summary>
        static List<Span> convertSpanList(string meas_string) {
            // splits a meas string into beats.
            string[] beat_array = meas_string.Split('|')
                .Select(x => x.Replace("     ", " ")) // replaces 5 spaces with 1 space.
                .Select(x => x.Replace("    ", " ")) // replaces 4 spaces with 1 space.
                .Select(x => x.Replace("   ", " ")) // replaces 3 spaces with 1 space.
                .Select(x => x.Replace("  ", " ")) // replaces 2 spaces with 1 space.
                .ToArray();
            // creates a list of Span objects.
            List<Span> span_list = new();
            // all 4 beats : "I | | | "
            if (beat_array[1].Equals(" ") && beat_array[2].Equals(" ") && beat_array[3].Equals(" ")) {
                span_list.Add(convertSpan(beat: 4, beat_string: beat_array[0])); // 1st beat.
            }
            // 3 beats and 1 beat : "I | | |V "
            else if (beat_array[1].Equals(" ") && beat_array[2].Equals(" ") && !beat_array[3].Equals(" ")) {
                span_list.Add(convertSpan(beat: 3, beat_string: beat_array[0])); // 1st beat.
                span_list.Add(convertSpan(beat: 1, beat_string: beat_array[3])); // 4th beat.
            }
            // 2 beats and 2 beats : "I | |V | "
            else if (beat_array[1].Equals(" ") && !beat_array[2].Equals(" ") && beat_array[3].Equals(" ")) {
                span_list.Add(convertSpan(beat: 2, beat_string: beat_array[0])); // 1st beat.
                span_list.Add(convertSpan(beat: 2, beat_string: beat_array[2])); // 3rd beat.
            }
            // 1 beat and 3 beats : "I |V | | "
            else if (!beat_array[1].Equals(" ") && beat_array[2].Equals(" ") && beat_array[3].Equals(" ")) {
                span_list.Add(convertSpan(beat: 1, beat_string: beat_array[0])); // 1st beat.
                span_list.Add(convertSpan(beat: 3, beat_string: beat_array[1])); // 2nd beat.
            }
            // 1 beat and 2 beats and 1 beat : "I |V | |I |"
            else if (!beat_array[1].Equals(" ") && beat_array[2].Equals(" ") && !beat_array[3].Equals(" ")) {
                span_list.Add(convertSpan(beat: 1, beat_string: beat_array[0])); // 1st beat.
                span_list.Add(convertSpan(beat: 2, beat_string: beat_array[1])); // 2nd beat.
                span_list.Add(convertSpan(beat: 1, beat_string: beat_array[3])); // 4th beat.
            }
            // FIXME: 2 beats and 1 beat and 1 beat.           : "I | |V |I "
            // FIXME: 1 beat and 1 beat and 2 beats.           : "I |V |I | |"
            // FIXME: 1 beat and 1 beat and 1 beat and 1 beat. : "I |V |I |V "
            return span_list;
        }

        /// <summary>
        /// converts a beat string and number of beats to a Span object.
        /// </summary>
        static Span convertSpan(int beat, string beat_string) {
            string[] part = beat_string.Split(separator: ':');
            if (part.Length == 1) {
                return new Span(beat: beat, degree: Degree.Enum.Parse(part[0].Trim()));
            }
            // a span has church mode specified.
            else {
                return new Span(beat: beat, degree: Degree.Enum.Parse(part[0].Trim()), span_mode: Mode.Enum.Parse(part[1].Trim()));
            }
        }

        /// <summary>
        /// gets the length of the beat for the "count" pattern.
        /// </summary>
        static void getCountBeatLength(Pattern pattern) {
            if (pattern.Name.Equals(COUNT_PATTERN)) {
                State.CountBeatLength = pattern.Data.GetBeatLength();
            }
        }

        /// <summary>
        /// reads a .json file to the JSON object.
        /// </summary>
        static Json loadJson(Stream target) {
            DataContractJsonSerializer serializer = new(typeof(Json));
            return (Json) serializer.ReadObject(target);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        [DataContract]
        class Json {
            [DataMember(Name = "pattern")]
            public Pattern[] PatternArray { get; set; }
        }

        [DataContract]
        class Pattern {
            [DataMember(Name = "name")]
            public string Name { get; set; }
            [DataMember(Name = "data")]
            public string Data { get; set; }
        }
    }
}
