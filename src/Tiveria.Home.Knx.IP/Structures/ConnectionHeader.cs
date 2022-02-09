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

namespace Tiveria.Home.Knx.IP.Structures
{
    /// <summary>
    /// <code>
    /// +--------+--------+--------+--------+
    /// | byte 1 | byte 2 | byte 3 | byte 4 |
    /// +--------+--------+--------+--------+
    /// | Header |Channel |Sequence|Reserved|
    /// | Length |ID      |Counter |        |
    /// +--------+--------+--------+--------+
    /// | 0x04   |        |        | 0x00   |
    /// +--------+--------+-----------------+
    ///
    /// Serice Type:  <see cref="Tiveria.Home.Knx.IP.Enums.ServiceTypeIdentifier"/>
    /// </code>
    /// </summary>
    public class ConnectionHeader : KnxDataElement
    {
        #region Constants
        public static readonly byte STRUCTURE_SIZE = 0x04;
        #endregion

        #region public properties
        public byte ChannelId { get; init; }
        public byte SequenceCounter { get; init; }
        #endregion

        #region Constructors
        public ConnectionHeader(byte channelId, byte sequenceCounter)
        {
            Size = STRUCTURE_SIZE;
            ChannelId = channelId;
            SequenceCounter = sequenceCounter;
        }
        #endregion

        #region Public Methods
        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write(STRUCTURE_SIZE);
            writer.Write(ChannelId);
            writer.Write(SequenceCounter);
            writer.Write((byte)0x00);
        }

        public override String ToString()
        {
            return String.Format($"ConnectionHeader: Size={Size}, ChannelId={ChannelId}, SequenceCounter={SequenceCounter}");
        }
        #endregion

        #region Static Parsing
        public static ConnectionHeader Parse(BigEndianBinaryReader reader)
        {
            reader.ReadSizeAndCheck("ConnectionHeader", STRUCTURE_SIZE);
            var channelId = reader.ReadByte();
            var seqCounter = reader.ReadByte();
            var dummy = reader.ReadByte();
            return new ConnectionHeader(channelId, seqCounter);
        }
        #endregion
    }
}
