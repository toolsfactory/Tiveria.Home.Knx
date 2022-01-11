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

using Tiveria.Common.IO;

namespace Tiveria.Home.Knx.IP.Structures
{
    /// <summary>
    /// Represents the Device Information DIB block described in chapter 7.5.4.2 of the spec 3/8/2 KnxIPNet core.
    /// </summary>
    /// <code>
    /// +--------+--------+--------+--------+
    /// | byte 1 | byte 2 | byte 3 to n     |
    /// +--------+--------+--------+--------+
    /// |  Size  |Descript| Any other DIB   |
    /// |  (8)   | Type   |                 |
    /// +--------+--------+--------+--------+
    /// </code>
    public class OtherDIB : DataElement
    {
        public byte DIBType { get; init; }
        public byte[] Data { get; init; }

        public OtherDIB(byte dibType, byte[] data)
        {
            DIBType = dibType;
            Data = data ?? Array.Empty<byte>();
            Size = (ushort)(2 + Data.Length);
        }

        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write((byte)Size);
            writer.Write(DIBType);
            if(Data.Length>0)
                writer.Write(Data);
        }

        public static OtherDIB Parse(BigEndianBinaryReader reader)
        {
            var size = reader.ReadByte();
            var dibType = reader.ReadByte();
            var data = (size > 2) ? reader.ReadBytes(size-2) : Array.Empty<byte>();
            return new OtherDIB(dibType, data);
        }
    }
}
