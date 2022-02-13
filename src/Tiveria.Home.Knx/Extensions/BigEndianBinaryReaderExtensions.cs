/*
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

using Tiveria.Common.IO;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.Extensions
{
    public static class BigEndianBinaryReaderExtensions
    {
        public static T ReadByteEnum<T>(this BigEndianBinaryReader reader, string structure) where T : Enum
        {
            var value = reader.ReadByte();
            if (!Enum.IsDefined(typeof(T), value))
                throw KnxBufferFieldException.TypeUnknown($"{structure}-{nameof(T)}", value);
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static byte ReadSizeAndCheck(this BigEndianBinaryReader reader, string structure, int expected)
        {
            var size = reader.ReadByte();
            if (size != expected)
                throw KnxBufferFieldException.WrongSize(structure, expected, size);
            if (reader.Available < (size - 2))
                throw KnxBufferSizeException.TooSmall(structure);
            return size;
        }
    }

}
