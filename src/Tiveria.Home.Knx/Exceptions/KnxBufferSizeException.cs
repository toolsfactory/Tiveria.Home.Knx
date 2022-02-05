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
    /// Exception raised when the raw buffer provided to create a structure from doesn't fit in size
    /// </summary>
    public class KnxBufferSizeException : KnxBufferException
    {
        public KnxBufferSizeException(string message) : base(message)
        { }

        public static KnxBufferSizeException TooSmall(string structure) => new KnxBufferSizeException($"Buffer too small for structure '{structure}'");
        public static KnxBufferSizeException TooBig(string structure) => new KnxBufferSizeException($"Buffer too big for structure '{structure}'");
        public static KnxBufferSizeException WrongSize(string structure, int expected, int actual) => new KnxBufferSizeException($"Buffer for structure '{structure}' has wrong size {actual}. Expected: {expected}");
        public static KnxBufferSizeException WrongSize(string structure, int actual) => new KnxBufferSizeException($"Buffer for structure '{structure}' has wrong size {actual}. See specs for expected values.");
    }


}
