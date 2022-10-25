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

using System.Linq;
using static System.Convert;
using static System.Math;

namespace Meowziq.Value {
    /// <summary>
    /// conversion class.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    /// <todo>
    /// are extension methods enough?
    /// </todo>
    public static class Converter {
        /// <summary>
        /// converts numeric BPM to tempo information for SMF.
        /// </summary>
        /// <summary_jp>
        public static byte[] ToByteTempo(int tempo) {
            double double_value = 60 * Pow(x: 10, y: 6) / tempo;
            string hex = int.Parse(s: Round(a: double_value).ToString()).ToString("X6"); // hexadecimal 6-digit conversion.
            char[] char_array = hex.ToCharArray();
            return new byte[3]{ // returns in 3 bytes.
                ToByte(value: char_array[0].ToString() + char_array[1].ToString(), fromBase: 16),
                ToByte(value: char_array[2].ToString() + char_array[3].ToString(), fromBase: 16),
                ToByte(value: char_array[4].ToString() + char_array[5].ToString(), fromBase: 16)
            };
        }

        /// <summary>
        /// converts string to byte array.
        /// </summary>
        public static byte[] ToByteArray(string target) {
            return target.ToCharArray().Select(selector: x => ToByte(value: x)).ToArray();
        }
    }
}
