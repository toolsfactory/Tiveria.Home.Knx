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
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    Linking this library statically or dynamically with other modules is
    making a combined work based on this library. Thus, the terms and
    conditions of the GNU General Public License cover the whole
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
        public byte Version { get; init; }
        public int VersionMajor => Version / 0x10; 
        public int VersionMinor => Version % 0x10; 
        public ServiceTypeIdentifier ServiceTypeIdentifier { get; init; } 
        public ushort ServiceTypeIdentifierRaw { get; init; }
        public ushort TotalLength { get; init; }
        #endregion

        #region Constructors
        public FrameHeader(ServiceTypeIdentifier servicetypeidentifier, int bodyLength)
            : this(KnxNetIPVersion.Version10, (ushort)servicetypeidentifier, bodyLength)
        { }

        public FrameHeader(KnxNetIPVersion version, ushort servicetypeidentifierRaw, int bodyLength)
        {
            if (!KnxNetIPVersion.IsSupportedVersion(version) && ThrowExceptionOnUnknownVersion)
                throw new BufferFieldException($"Unknown KnxNetIPVersion {version.Identifier} with header size {version.HeaderLength}");

            if (Enum.IsDefined(typeof(ServiceTypeIdentifier), servicetypeidentifierRaw))
                ServiceTypeIdentifier = (ServiceTypeIdentifier)servicetypeidentifierRaw;
            else
                ServiceTypeIdentifier = ServiceTypeIdentifier.Unknown;

            Size = version.HeaderLength;
            Version = version.Identifier;
            TotalLength = (ushort)(Size + bodyLength);
            ServiceTypeIdentifierRaw = servicetypeidentifierRaw;
        }

        #endregion

        #region Public Methods

        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write((byte)Size);
            writer.Write(Version);
            writer.Write((ushort)ServiceTypeIdentifier);
            writer.Write((ushort)TotalLength);
        }

        public override String ToString()
        {
            return String.Format("KNXnet/IP {0}.{1} - {2} (0x{3:x}) - {4} bytes / {5} bytes", VersionMajor, VersionMinor, ServiceTypeIdentifier.ToDescription(), (byte)ServiceTypeIdentifier, Size, TotalLength);
        }
        #endregion

        #region Static Parsing
        public static FrameHeader Parse(BigEndianBinaryReader reader)
        {
            var headersize = reader.ReadByte();
            var versionidentifier = reader.ReadByte();
            if (!KnxNetIPVersion.TryGetFindSupportedVersion(versionidentifier, headersize, out var version))
                version = new KnxNetIPVersion("Custom Version", versionidentifier, headersize);

            var serviceTypeIdRaw = reader.ReadUInt16();

            var totalLength = reader.ReadUInt16();
            if (totalLength - headersize > reader.Available)
                throw BufferSizeException.TooSmall("Buffer<Header|TotalLength");

            return new FrameHeader(version, serviceTypeIdRaw, totalLength - headersize);
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
