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
using Tiveria.Home.Knx.IP.Enums;

namespace Tiveria.Home.Knx.IP.Structures
{
    /// <summary>
    /// Represents the Lost Messages Info block described in chapter 7.5.4.2 of the spec 3/8/5 KnxIPNet Routing.
    /// </summary>
    /// <code>
    /// +--------+--------+--------+--------+
    /// | byte 1 | byte 2 | byte 3 | byte 4 |
    /// +--------+--------+--------+--------+
    /// |  Size  | Device | Number of lost  |
    /// |  (8)   | State  | Messages        |
    /// +--------+--------+-----------------+
    public class LostMessageInfo : KnxDataElement
    {
        public static readonly byte STRUCTURE_SIZE = 0x04;
        public DeviceStatus DeviceStatus { get; init; }
        public ushort NumberOfLostMessages { get; init; }

        public LostMessageInfo(DeviceStatus deviceStatus, ushort lostMessages)
        { 
            DeviceStatus = deviceStatus;
            NumberOfLostMessages = lostMessages;
            Size = STRUCTURE_SIZE;
        }

        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write(STRUCTURE_SIZE);
            writer.Write((byte) DeviceStatus);
            writer.Write(NumberOfLostMessages);
        }

        public static LostMessageInfo Parse(BigEndianBinaryReader reader)
        {
            var size = reader.ReadSizeAndCheck("LostMessageInfo", STRUCTURE_SIZE);
            var status = reader.ReadByteEnum<DeviceStatus>("LostMessageInfo.DeviceStatus");
            var lostmessages = reader.ReadUInt16();
            return new LostMessageInfo(status, lostmessages);
        }
    }
}
