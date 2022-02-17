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
    public class FrameHeader : KnxDataElement
    {
        #region Constants
        public static bool ThrowExceptionOnUnknownVersion = true;
        #endregion

        #region public properties
        /// <summary>
        /// Version information embedded in the header
        /// </summary>
        public KnxNetIPVersion Version { get; init; }
        /// <summary>
        /// 
        /// </summary>
        public ushort ServiceTypeIdentifier { get; init; } 
        public ushort TotalLength { get; init; }
        #endregion

        #region Constructors
        public FrameHeader(KnxNetIPVersion version, ushort servicetypeidentifier, int bodyLength)
        {
            if (!KnxNetIPVersion.IsSupportedVersion(version) && ThrowExceptionOnUnknownVersion)
                throw new KnxBufferFieldException($"Unknown KnxNetIPVersion {version.Identifier} with header size {version.HeaderLength}");

            Size = version.HeaderLength;
            Version = version;
            TotalLength = (ushort)(Size + bodyLength);
            ServiceTypeIdentifier = servicetypeidentifier;
        }


        public FrameHeader(ushort servicetypeidentifier, int bodyLength)
            : this(KnxNetIPVersion.DefaultVersion, servicetypeidentifier, bodyLength)
        { }

        public FrameHeader(IKnxNetIPService service)
            : this(KnxNetIPVersion.DefaultVersion, service.ServiceTypeIdentifier, service.Size)
        { }

        public FrameHeader(KnxNetIPVersion version, IKnxNetIPService service)
            : this(version, service.ServiceTypeIdentifier, service.Size)
        { }
        #endregion

        #region Public Methods

        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write((byte)Size);
            writer.Write(Version.Identifier);
            writer.Write((ushort)ServiceTypeIdentifier);
            writer.Write((ushort)TotalLength);
        }

        public override String ToString()
        {
            return String.Format($"KNXnet/IP {Version.VersionString} - {Enums.ServiceTypeIdentifier.ToString(ServiceTypeIdentifier)} ({(byte)ServiceTypeIdentifier:x}0x) - {Size} bytes / {TotalLength} bytes");
        }
        #endregion

        #region Static Parsing
        public static FrameHeader Parse(BigEndianBinaryReader reader)
        {
            var headersize = reader.ReadByte();
            var versionidentifier = reader.ReadByte();
            if (!KnxNetIPVersion.FindSupportedVersion(versionidentifier, headersize, out var version))
                version = new KnxNetIPVersion("Custom Version", versionidentifier, headersize);
            var serviceTypeId = reader.ReadUInt16();
            var totalLength = reader.ReadUInt16();
            if (totalLength - headersize > reader.Available)
                throw KnxBufferSizeException.TooSmall("Buffer<Header|TotalLength");

            return new FrameHeader(version!, serviceTypeId, totalLength - headersize);
        }

        public static bool TryParse(BigEndianBinaryReader reader, out FrameHeader? frameHeader)
        {
            try
            {
                frameHeader = Parse(reader);
                return true;
            }
            catch
            {
                frameHeader = null;
                return false;
            }
        }

        public static bool TryParse(byte[] buffer, out FrameHeader? frameHeader)
        {
            try
            {
                var reader = new BigEndianBinaryReader(buffer);
                frameHeader = Parse(reader);
                return true;
            }
            catch
            {
                frameHeader = null;
                return false;
            }
        }
        /// <summary>
        /// Performs basic validation if the provided buffer starts with a correct KnxNetIP Header.
        /// </summary>
        /// <param name="buffer">The buffer containing the header and the payload</param>
        /// <param name="offset">Position where the KnxNetIP Header is supposed to start</param>
        /// <param name="version">Version</param>
        /// <param name="size">Header size</param>
        /// <returns>validity of hte buffer</returns>
        /// <remarks>besides knxnetip header version, lenght of header and payload are checked. ServiceType is not checked in detail</remarks>
        public static bool IsValidHeader(byte[] buffer, int offset, KnxNetIPVersion version)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");
            // check basics
            return buffer.Length - offset == version.HeaderLength &&
                buffer[offset] == version.HeaderLength &&
                buffer[offset + 1] == version.Identifier &&
                (buffer[offset + 4] << 8) + buffer[offset + 5] == buffer.Length - offset;
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
            return IsValidHeader(buffer, offset, KnxNetIPVersion.Version10);
        }
        #endregion
    }
}
