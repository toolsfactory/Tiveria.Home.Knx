﻿/*
    Tiveria.Home.Knx - a .Net Core base KNX library
    Copyright (c) 2018-2022 M. Geissler

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    Linking this library statically or dynamically with other modules is
    making a combined work based on this library. Thus, the terms and
    conditions of the GNU Lesser General Public License cover the whole
    combination.
*/

namespace Tiveria.Home.Knx.Cemi
{
    /// <summary>
    /// Enumeration of all available values for the priority flag in <see cref="ControlField1"/>
    /// </summary>
    public enum Priority : byte
    {
        //System priority, reserved for high priority management and system configuration.
        System = 0b00,
        // Normal priority, the default for short frames.
	    Normal = 0b01,
        // Urgent priority, for urgent frames.
	    Urgent = 0b10,
        // Low priority, used for long frames.
	    Low = 0b11
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
