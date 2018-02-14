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
using Tiveria.Knx.Structures;
using Tiveria.Knx.IP.Utils;
using Tiveria.Common.IO;
using Tiveria.Knx.Exceptions;

namespace Tiveria.Knx.IP.Structures
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
    /// Serice Type:  <see cref="Tiveria.Knx.IP.Utils.ServiceTypeIdentifier"/>
    /// </code>
    /// </summary>
    public class ConnectionHeader : StructureBase
    {
        #region Constants
        public static readonly byte CONNECTION_HEADER_SIZE_10 = 0x04;
        #endregion

        #region private fields
        private byte _channelId;
        private byte _sequenceCounter;
        #endregion

        #region public properties
        public byte ChannelId => _channelId;
        public byte SequenceCounter => _sequenceCounter;
        #endregion

        #region Constructors
        public ConnectionHeader(BinaryReaderEx br)
        {
            if (br == null)
                throw new ArgumentNullException("buffer is null");
            ParseHeaderSize(br);
            ParseChannelId(br);
            ParseSequenceCounter(br);
            // just read last byte as this is 0x00 and reserved
            br.ReadByte();
        }

        public ConnectionHeader(byte channelId, byte sequenceCounter)
        {
            _structureLength = CONNECTION_HEADER_SIZE_10;
            _channelId = channelId;
            _sequenceCounter = sequenceCounter;
        }
        #endregion

        #region private parsing and verification methods
        private void ParseHeaderSize(BinaryReaderEx br)
        {
            var len = br.ReadByte();
            ValidateSize(len);
            _structureLength = len;
        }

        private void ParseChannelId(BinaryReaderEx br)
        {
            _channelId = br.ReadByte();
        }

        private void ParseSequenceCounter(BinaryReaderEx br)
        {
            _sequenceCounter = br.ReadByte();
        }        

        private void ValidateSize(byte size)
        {
            if (size != CONNECTION_HEADER_SIZE_10)
                throw BufferFieldException.WrongValue("KnxNetIP ConnectionHeader Size", CONNECTION_HEADER_SIZE_10, size);
        }
        #endregion

        #region Public Methods
        public override void WriteToByteArray(byte[] buffer, int offset)
        {
            base.WriteToByteArray(buffer, offset);
            buffer[0] = CONNECTION_HEADER_SIZE_10;
            buffer[1] = _channelId;
            buffer[2] = _sequenceCounter;
            buffer[3] = 0x00;
        }

        public override String ToString()
        {
            return String.Format($"ConnectionHeader: Size={_structureLength}, ChannelId={_channelId}, SequenceCounter={_sequenceCounter}");
        }
        #endregion

        #region Static Parsing
        public static ConnectionHeader Parse(BinaryReaderEx br)
        {
            return new ConnectionHeader(br);
        }

        public static ConnectionHeader Parse(byte[] buffer, int offset)
        {
            return Parse(new BinaryReaderEx(buffer, offset));
        }

        public static bool TryParse(BinaryReaderEx br, out ConnectionHeader header)
        {
            try
            {
                header = Parse(br);
                return true;
            }
            catch
            {
                header = null;
                return false;
            }
        }

        public static bool TryParse(byte[] buffer, int offset, out ConnectionHeader header)
        {
            try
            {
                header = Parse(buffer, offset);
                return true;
            }
            catch
            {
                header = null;
                return false;
            }
        }
        #endregion
    }
}
