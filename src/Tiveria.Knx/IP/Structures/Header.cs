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
using Tiveria.Knx.IP.Utils;

namespace Tiveria.Knx.IP.Structures
{
    public class Header : StructureBase
    {
        #region Constants
        public static byte KNXNETIP_VERSION_10 = 0x10;
        public static byte HEADER_SIZE_10 = 0x06;
        #endregion

        #region private fields
        private ServiceTypeIdentifier _serviceTypeIdentifier;
        private byte _size;
        private byte _version;
        private int _totalLength;
        #endregion

        #region public properties
        public byte Size { get => _size; }
        public byte Version { get => _version; }
        public int VersionMajor { get => _version / 0x10; }
        public int VersionMinor { get => _version % 0x10; }
        public ServiceTypeIdentifier ServiceTypeIdentifier { get => _serviceTypeIdentifier; }
        public int TotalLength { get => _totalLength; }
        #endregion

        #region Constructors
        public Header(byte version, byte size, ServiceTypeIdentifier servicetypeidentifier, ushort bodyLength)
        {
            if (version != KNXNETIP_VERSION_10)
                throw new ArgumentOutOfRangeException("Only version 0x10 of KnxNetIP is supported");
            if (size != HEADER_SIZE_10)
                throw new ArgumentOutOfRangeException("Only size 0x06 is supported in version 1 of the protocol");
            if(bodyLength<1)
                throw new ArgumentOutOfRangeException("Body size too small");
            if (bodyLength > 26)
                throw new ArgumentOutOfRangeException("Body size too big");

            _size = size;
            _version = version;
            _totalLength = _size + bodyLength;
            _serviceTypeIdentifier = servicetypeidentifier;
            _structureLength = _size;
        }

        public Header(ServiceTypeIdentifier servicetypeidentifier, ushort bodyLength)
            : this(KNXNETIP_VERSION_10, HEADER_SIZE_10, servicetypeidentifier, bodyLength)
        { }
        #endregion

        #region Public Methods
        public override void WriteToByteArray(ref byte[] buffer, int offset)
        {
            base.WriteToByteArray(ref buffer, offset);
            buffer[0] = HEADER_SIZE_10;
            buffer[1] = KNXNETIP_VERSION_10;
            buffer[2] = (byte)((int)ServiceTypeIdentifier >> 8);
            buffer[3] = (byte)(int)ServiceTypeIdentifier;
            buffer[4] = (byte)(_totalLength >> 8);
            buffer[5] = (byte)_totalLength;
        }

        public override String ToString()
        {
            return String.Format("KNXnet/IP {0}.{1} - {2} (0x{3:x}) - {4} bytes / {5} bytes", VersionMajor, VersionMinor, ServiceTypeIdentifier.ToDescription(), (byte)ServiceTypeIdentifier, Size, TotalLength);
        }
        #endregion

        #region Static Parsing
        public static Header FromBuffer(ref byte[] buffer, int offset)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");
            if (buffer.Length-offset < 6)
                throw new ArgumentException("buffer too small");

            if (buffer[offset] != HEADER_SIZE_10)
                throw new ArgumentException("Invalid header size. 0x6 expected.");
            if (buffer[offset+1] != KNXNETIP_VERSION_10)
                throw new ArgumentException("Invalid KNXNETIP Version. 0x10 expected.");

            var totalLength = (buffer[offset+4] << 8) + buffer[offset + 5];
            if (totalLength != buffer.Length-offset)
                throw new ArgumentException("data length and header totallength do not match");

            int sti = (buffer[offset + 2] << 8) + buffer[offset + 3];
            if (!Enum.IsDefined(typeof(ServiceTypeIdentifier), (ushort)sti))
                throw new ArgumentException($"Unknown Servicetypeidentifier {sti:x}");

            return new Header((ServiceTypeIdentifier)sti, (ushort) (totalLength - HEADER_SIZE_10));
        }
        #endregion
    }
}
