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

using System;
using System.Collections.Generic;
using System.Text;
using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.Utils;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Common.Logging;
using Tiveria.Common.IO;

namespace Tiveria.Home.Knx.IP.ServiceTypes
{
    /// <summary>
    /// <code>
    /// +--------+--------+--------+--------+
    /// | byte 1 | byte 2 | byte 3 | byte 4 |
    /// +--------+--------+--------+--------+
    /// | Header |Channel |Sequence|Status  |
    /// | Length |ID      |Counter |Code    |
    /// +--------+--------+--------+--------+
    /// | 0x04   |        |        |        |
    /// +--------+--------+-----------------+
    ///
    /// Serice Type:  <see cref="Tiveria.Home.Knx.IP.Enums.ServiceTypeIdentifier"/>
    /// </code>
    /// </summary>
    public class ServiceAcknowledgement : ServiceTypeBase, IServiceType
    {
        #region Constants
        public static readonly byte SERVICEACK_HEADER_SIZE_10 = 0x04;
        #endregion

        #region private fields
        private byte _channelId;
        private byte _sequenceCounter;
        private ErrorCodes _statusCode;
        #endregion

        #region public properties
        public byte ChannelId => _channelId;
        public byte SequenceCounter => _sequenceCounter;
        public ErrorCodes StatusCode => _statusCode;
        #endregion

        #region Constructors
        public ServiceAcknowledgement(ServiceTypeIdentifier svcTypeIdentifier, IndividualEndianessBinaryReader br)
            : base(svcTypeIdentifier)
        {
            if (br == null)
                throw new ArgumentNullException("buffer is null");
            ParseHeaderSize(br);
            ParseChannelId(br);
            ParseSequenceCounter(br);
            ParseLastByte(br);
        }

        public ServiceAcknowledgement(ServiceTypeIdentifier svcTypeIdentifier, byte channelId, byte sequenceCounter, ErrorCodes statusCode)
            : base(svcTypeIdentifier)
        {
            _size = SERVICEACK_HEADER_SIZE_10;
            _channelId = channelId;
            _sequenceCounter = sequenceCounter;
            _statusCode = statusCode;
        }
        #endregion

        #region private parsing and verification methods
        private void ParseHeaderSize(IndividualEndianessBinaryReader br)
        {
            var len = br.ReadByte();
            ValidateSize(len);
            _size = len;
        }

        private void ParseChannelId(IndividualEndianessBinaryReader br)
        {
            _channelId = br.ReadByte();
        }

        private void ParseSequenceCounter(IndividualEndianessBinaryReader br)
        {
            _sequenceCounter = br.ReadByte();
        }

        private void ValidateSize(byte size)
        {
            if (size != SERVICEACK_HEADER_SIZE_10)
                throw BufferFieldException.WrongValue("KnxNetIP ServiceAcknowledgement Size", SERVICEACK_HEADER_SIZE_10, size);
        }

        protected virtual void ParseLastByte(IndividualEndianessBinaryReader br)
        {
            var statusCode = br.ReadByte();
            if (!Enum.IsDefined(typeof(ErrorCodes), statusCode))
                throw BufferFieldException.TypeUnknown("StatusCode", statusCode);
            _statusCode = (ErrorCodes)statusCode;
        }
        #endregion

        #region Public Methods
        public override void WriteToByteArray(byte[] buffer, int offset)
        {
            base.WriteToByteArray(buffer, offset);
            buffer[0] = SERVICEACK_HEADER_SIZE_10;
            buffer[1] = _channelId;
            buffer[2] = _sequenceCounter;
            buffer[3] = (byte)_statusCode;
        }

        public override String ToString()
        {
            return String.Format($"ServiceAck: Size={_size}, ChannelId={_channelId}, SequenceCounter={_sequenceCounter}, StatusCode={_statusCode}");
        }
        #endregion

        #region Static Parsing
        public static ServiceAcknowledgement Parse(ServiceTypeIdentifier svcTypeIdentifier, IndividualEndianessBinaryReader br)
        {
            return new ServiceAcknowledgement(svcTypeIdentifier, br);
        }

        public static ServiceAcknowledgement Parse(ServiceTypeIdentifier svcTypeIdentifier, byte[] buffer, int offset)
        {
            return Parse(svcTypeIdentifier, new IndividualEndianessBinaryReader(buffer, offset));
        }

        public static bool TryParse(ServiceTypeIdentifier svcTypeIdentifier, IndividualEndianessBinaryReader br, out ServiceAcknowledgement header)
        {
            try
            {
                header = Parse(svcTypeIdentifier, br);
                return true;
            }
            catch
            {
                header = null;
                return false;
            }
        }

        public static bool TryParse(ServiceTypeIdentifier svcTypeIdentifier, byte[] buffer, int offset, out ServiceAcknowledgement header)
        {
            try
            {
                header = Parse(svcTypeIdentifier, buffer, offset);
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