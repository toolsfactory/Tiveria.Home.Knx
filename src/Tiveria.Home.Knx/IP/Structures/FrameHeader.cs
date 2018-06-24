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
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Common.IO;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.IP.Structures
{
    /// <summary>
    /// <code>
    /// +--------+--------+--------+--------+--------+--------+
    /// | byte 1 | byte 2 | byte 3 | byte 4 | byte 5 | byte 6 |
    /// +--------+--------+--------+--------+--------+--------+
    /// | Header |KNXNETIP| Service Type    | Total Length    |
    /// | Length |Version |                 |                 |
    /// +--------+--------+--------+--------+--------+--------+
    /// | 0x06   | 0x10   |                 |                 |
    /// +--------+--------+-----------------+--------+--------+
    ///
    /// Serice Type:  <see cref="Tiveria.Home.Knx.IP.Enums.ServiceTypeIdentifier"/>
    /// Total Length: Header Length + sizeof(cEMI Frame)
    /// </code>
    /// </summary>
    public class FrameHeader : StructureBase
    {
        #region Constants
        public static readonly byte KNXNETIP_VERSION_10 = 0x10;
        public static readonly byte HEADER_SIZE_10 = 0x06;
        public static bool ThrowExceptionOnUnknownVersion = true;
        #endregion

        #region private fields
        private ServiceTypeIdentifier _serviceTypeIdentifier;
        private byte _version;
        private int _totalLength;
        private ushort _serviceTypeRaw;
        #endregion

        #region public properties
        public byte Version => _version;
        public int VersionMajor => _version / 0x10; 
        public int VersionMinor => _version % 0x10; 
        public ServiceTypeIdentifier ServiceTypeIdentifier => _serviceTypeIdentifier; 
        public ushort ServiceTypeRaw  => _serviceTypeRaw;
        public int TotalLength => _totalLength; 
        #endregion

        #region Constructors
        public FrameHeader(IndividualEndianessBinaryReader br)
        {
            if (br == null)
                throw new ArgumentNullException("buffer is null");
            ParseHeaderSizeAndVersion(br);
            ParseServiceType(br);
            ParseTotalSize(br);
        }

        public FrameHeader(byte version, byte size, ServiceTypeIdentifier servicetypeidentifier, ushort bodyLength)
        {
            ValidateVersionAndSize(version, size);
            _size = size;
            _version = version;
            _totalLength = _size + bodyLength;
            _serviceTypeIdentifier = servicetypeidentifier;
            base._size = _size;
        }

        public FrameHeader(ServiceTypeIdentifier servicetypeidentifier, ushort bodyLength)
            : this(KNXNETIP_VERSION_10, HEADER_SIZE_10, servicetypeidentifier, bodyLength)
        { }
        #endregion

        #region private parsing and verification methods
        private void ParseTotalSize(IndividualEndianessBinaryReader br)
        {
            var hi = br.ReadByte();
            var lo = br.ReadByte();
            _totalLength = (hi << 8) + lo;
            if (_totalLength - _size > br.Available)
                throw BufferSizeException.TooSmall("Buffer<Header|TotalLength");
        }

        private void ParseHeaderSizeAndVersion(IndividualEndianessBinaryReader br)
        {
            _size = br.ReadByte();
            _version = br.ReadByte();
            ValidateVersionAndSize(_version, _size);
        }

        private void ParseServiceType(IndividualEndianessBinaryReader br)
        {
            var hi = br.ReadByte();
            var lo = br.ReadByte();
            _serviceTypeRaw = (ushort)((hi << 8) + lo);
            if (Enum.IsDefined(typeof(ServiceTypeIdentifier), _serviceTypeRaw))
                _serviceTypeIdentifier = (ServiceTypeIdentifier)_serviceTypeRaw;
            else
                _serviceTypeIdentifier = ServiceTypeIdentifier.UNKNOWN;
        }

        private void ValidateVersionAndSize(byte version, int size)
        {
            if (ThrowExceptionOnUnknownVersion)
            {
                if (version != KNXNETIP_VERSION_10)
                    throw BufferFieldException.WrongValue("KnxNetIP Frame Version", 0x10, version);
                if (size != HEADER_SIZE_10)
                    throw BufferFieldException.WrongValue("KnxNetIP Frame Size", 0x06, size);
            }
        }
        #endregion

        #region Public Methods
        public override void WriteToByteArray(byte[] buffer, int offset)
        {
            base.WriteToByteArray(buffer, offset);
            buffer[0] = HEADER_SIZE_10;
            buffer[1] = KNXNETIP_VERSION_10;
            buffer[2] = (byte)((int)ServiceTypeIdentifier >> 8);
            buffer[3] = (byte)(int)ServiceTypeIdentifier;
            buffer[4] = (byte)(_totalLength >> 8);
            buffer[5] = (byte)_totalLength;
        }

        public override void Write(IndividualEndianessBinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(HEADER_SIZE_10);
            writer.Write(KNXNETIP_VERSION_10);
            writer.Write((byte)((int)ServiceTypeIdentifier >> 8));
            writer.Write((byte)ServiceTypeIdentifier);
            writer.Write((byte)(_totalLength >> 8));
            writer.Write((byte)_totalLength);
        }

        public override String ToString()
        {
            return String.Format("KNXnet/IP {0}.{1} - {2} (0x{3:x}) - {4} bytes / {5} bytes", VersionMajor, VersionMinor, ServiceTypeIdentifier.ToDescription(), (byte)ServiceTypeIdentifier, Size, TotalLength);
        }
        #endregion

        #region Static Parsing
        public static FrameHeader Parse(IndividualEndianessBinaryReader br)
        {
            return new FrameHeader(br);
        }

        public static FrameHeader Parse(byte[] buffer, int offset)
        {
            return Parse(new IndividualEndianessBinaryReader(buffer, offset));
        }

        public static bool TryParse(IndividualEndianessBinaryReader br, out FrameHeader header)
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

        public static bool TryParse(byte[] buffer, int offset, out FrameHeader header)
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

        /// <summary>
        /// Performs basic validation if the provided buffer starts with a correct KnxNetIP Header.
        /// </summary>
        /// <param name="buffer">The buffer containing the header and the payload</param>
        /// <param name="offset">Position where the KnxNetIP Header is supposed to start</param>
        /// <returns>validity of hte buffer</returns>
        /// <remarks>besides knxnetip header version, lenght of header and payload are checked. ServiceType is not checked in detail</remarks>
        public static bool IsValidHeaderV10(byte[] buffer, int offset)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");
            // check basics
            return buffer.Length - offset == 6 &&
                buffer[offset] == HEADER_SIZE_10 &&
                buffer[offset + 1] == KNXNETIP_VERSION_10 &&
                (buffer[offset + 4] << 8) + buffer[offset + 5] == buffer.Length - offset;
        }
        #endregion
    }
}
