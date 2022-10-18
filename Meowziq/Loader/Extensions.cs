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

namespace Meowziq.Loader {
    /// <summary>
    /// loader extension methods.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    internal static class Extensions {
        /// <summary>
        /// gets the length of the beat.
        /// </summary>
        internal static int GetBeatLength(this string source) {
            const int MEAS_TO_BEAT = 4;
            int length = source.Replace("][", "@").Split('@').Select(x => x.Replace("[", string.Empty).Replace("]", string.Empty)).ToArray().Length;
            return length * MEAS_TO_BEAT;
        }
    }
}