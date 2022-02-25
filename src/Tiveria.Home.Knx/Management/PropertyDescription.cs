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

using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.Management
{
    /// <summary>
    /// Contains the property description information in a PropertyDescription_Response service.
    /// </summary>
    public class PropertyDescription
    {
        public const int Size = 7;
        public byte ObjectIndex { get; init; }
        public byte PropertyId { get; init; }
        public byte PropertyIndex { get; init; }
        public bool WriteEnabled { get; init; }
        public int MaxElements { get; init; }
        public byte PropertyType { get; init; }
        public byte ReadLevel { get; init; }
        public byte WriteLevel { get; init; }

        public PropertyDescription(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");
            if(data.Length != Size) throw KnxBufferSizeException.WrongSize("PropertyDescription", Size, data.Length);
            ObjectIndex   = data[0];
            PropertyId    = data[1];
            PropertyIndex = data[2];
            WriteEnabled  = (data[3] & 0b1000_0000) == 0b1000_0000;
            PropertyType  = (byte) (data[3] & 0b00_111111);
            MaxElements   = ((data[4] & 0b0000_1111) << 8) + data[5];
            ReadLevel     = (byte)((data[6] & 0b1111_0000) >> 4);
            WriteLevel    = (byte)(data[6] & 0b0000_1111);
        }

        public byte[] ToBytes()
        {
            var data = new byte[Size];
            data[0] = ObjectIndex;
            data[1] = PropertyId;
            data[2] = PropertyIndex;
            data[3] = (byte) (WriteEnabled ? 0b1000_0000 : 0x00);
            data[3] |=(byte) (PropertyType & 0b00_111111);
            data[4] = (byte) ((MaxElements >> 8) & 0b0000_1111);
            data[5] = (byte) MaxElements;
            data[6] = (byte) ((ReadLevel << 4) | (WriteLevel & 0b0000_11111));
            return data;
        }
    }
}
