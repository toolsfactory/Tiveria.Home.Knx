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
    /// Class representing the KnxNetIP ConnectionHeader used to indicate the channelID and sequenceNo when communication via Tunneling with a Knx Interface.
    /// This class allows changing its properties as both values are only set moments before the message is sent.
    /// <code>
    /// +--------+--------+--------+--------+
    /// | byte 1 | byte 2 | byte 3 | byte 4 |
    /// +--------+--------+--------+--------+
    /// | Header |Channel |Sequence|Reserved|
    /// | Length |ID      |Counter |        |
    /// +--------+--------+--------+--------+
    /// | 0x04   |        |        | 0x00   |
    /// +--------+--------+-----------------+
    /// </code>
    /// </summary>
    public class ConnectionHeader : KnxDataElement
    {
        #region Constants
        /// <summary>
        /// Size of the structure when serialized to its binary representation
        /// </summary>
        public static readonly byte STRUCTURE_SIZE = 0x04;
        #endregion

        #region public properties
        /// <summary>
        /// The ID of the channel used for communication with the Knx Interface.
        /// </summary>
        public byte ChannelId { get; set; } = 0;

        /// <summary>
        /// The message sequence number
        /// </summary>
        public byte SequenceCounter { get; set; } = 0;
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the ConnectionHeader class.
        /// </summary>
        public ConnectionHeader()
        { }

        /// <summary>
        /// Creates a new instance of the ConnectionHeader class.
        /// </summary>
        /// <param name="channelId">The initial channel id</param>
        /// <param name="sequenceCounter">The initial sequence Counter</param>
        public ConnectionHeader(byte channelId, byte sequenceCounter)
        {
            Size = STRUCTURE_SIZE;
            ChannelId = channelId;
            SequenceCounter = sequenceCounter;
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write(STRUCTURE_SIZE);
            writer.Write(ChannelId);
            writer.Write(SequenceCounter);
            writer.Write((byte)0x00);
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            return String.Format($"(Len {Size}, ChId {ChannelId}, SeqCnt {SequenceCounter})");
        }
        #endregion

        #region Static Parsing
        /// <summary>
        /// Parses a binary representation and creates a matching instance of the <see cref="ConnectionHeader"/> class
        /// </summary>
        /// <param name="reader">The binary reader to use for parsing</param>
        /// <returns>Instance of <see cref="ConnectionHeader"/></returns>
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
