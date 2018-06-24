/*
    Tiveria.Home.Knx - a .Net Core base KNX library
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

namespace Tiveria.Home.Knx.Exceptions
{
    /// <summary>
    /// Exception raised whenever a value for a field with predefined correct values is found that is not matching.
    /// </summary>
    public class BufferFieldException : BufferException
    {
        public BufferFieldException(string message) : base(message)
        { }

        public static BufferFieldException TypeUnknown(string structure, int value) => new BufferFieldException($"Value 0x{value:x}/{value} not know for structure '{structure}'.");
        public static BufferFieldException WrongValue(string structure, int expected, int actual) => new BufferFieldException($"For '{structure}' value '{expected}' expected but '{actual}' provided.");
        public static BufferFieldException WrongValue(string structure, string expected, string actual) => new BufferFieldException($"For '{structure}' value '{expected}' expected but '{actual}' provided.");
        public static BufferFieldException WrongValueAt(string structure, string field, int pos) => new BufferFieldException($"Wrong value in '{structure}' for '{field}' at position {pos}.");
        public static BufferFieldException WrongSize(string structure, int expected, int actual) => new BufferFieldException($"For '{structure}' a size of {expected} expected but {actual} provided.");
        public static BufferFieldException TooSmall(string structure, int min, int max, int actual) => new BufferFieldException($"For '{structure}' a size between {min} and {max} expected but {actual} provided.");
        public static BufferFieldException TooSmall(string structure, int min, int actual) => new BufferFieldException($"For '{structure}' a size of at least {min} expected but {actual} provided.");
    }
}
