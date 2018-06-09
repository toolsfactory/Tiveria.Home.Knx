/*
    Tiveria.Knx - a .Net Core base KNX library
    Copyright (c) 2018 M. Geissler

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    Linking this library statically or dynamically with other modules is
    making a combined work based on this library. Thus, the terms and
    conditions of the GNU General Public License cover the whole
    combination.
*/

using System;

namespace Tiveria.Knx.Cemi
{

    public enum Priority : byte
    {
        //System priority, reserved for high priority management and system configuration.
        System = 0x00,
        // Normal priority, the default for short frames.
	    Normal = 0x01,
        // Urgent priority, for urgent frames.
	    Urgent = 0x02,
        // Low priority, used for long frames.
	    Low = 0x03
    }

    public static partial class EnumExtensions
    {
        /// <summary>
        /// translate <see cref="Priority"/> to a readable string
        /// </summary>
        /// <returns>the string representation of the enum value</returns>
        public static String ToDescription(this Priority priority)
        {
            switch (priority)
            {
                case Priority.Low:
                    return "Low";
                case Priority.Normal:
                    return "Normal";
                case Priority.System:
                    return "System";
                case Priority.Urgent:
                    return "Urgent";
                default:
                    return "Unknown";
            }
        }
    }
}
