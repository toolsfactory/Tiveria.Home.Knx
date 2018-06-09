/*, ushort start
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
using Tiveria.Knx.Exceptions;
using Tiveria.Common.Extensions;
using Tiveria.Common.IO;

namespace Tiveria.Knx.IP.Structures
{
    /// <summary>
    /// Immutable container for a connection request information (CRI).
    /// The CRI structure is used for the additional information in a connection request.<br>
    /// </summary>
    public class CRI: StructureBase
    {
        protected byte[] _optionalData;

        private ConnectionType _connectionType;
        public ConnectionType ConnectionType { get => _connectionType; }

        public CRI(ConnectionType connectionType)
            : this(connectionType, null)
        { }

        public CRI(ConnectionType connectionType, byte[] optionalData) 
        {
            _connectionType = connectionType;
            _optionalData = optionalData ?? (new byte[0]);
            _size = 2 + _optionalData.Length;
        }

        /// <summary>
        /// Returns a cloned version of the optional data to ensure that the data is not modified directly
        /// </summary>
        /// <returns></returns>
        public byte[] GetOptionalData()
        {
            return (byte[])_optionalData.Clone();
        }

        public override void WriteToByteArray(byte[] buffer, int offset)
        {
            base.WriteToByteArray(buffer, offset);
            buffer[offset + 0] = (byte) (_size);
            buffer[offset + 1] = (byte)_connectionType;
            if (_optionalData.Length > 0)
                _optionalData.CopyTo(buffer, offset+2);
        }

        public override void Write(IndividualEndianessBinaryWriter writer)
        {
            base.Write(writer);
            writer.Write((byte)_size);
            writer.Write((byte)_connectionType);
            if (_optionalData.Length > 0)
                writer.Write(_optionalData);
        }

        public static CRI FromBuffer(byte[] buffer, int offset = 0)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");

            var structlen = buffer[offset];
            if (structlen < 2)
                throw new ArgumentException("invalid structure size(<2)");
            if (buffer.Length - offset != structlen)
                throw new ArgumentException("buffer has not the correct size");

            var contype = buffer[offset + 1];
            if (!Enum.IsDefined(typeof(ConnectionType), contype))
                throw BufferFieldException.TypeUnknown("CRI", contype);

            switch (contype) {
                case (byte)ConnectionType.TUNNEL_CONNECTION:
                    return CRITunnel.FromBuffer(buffer, offset);
                default:
                    var optional = new byte[structlen - 2];
                    buffer.Slice(optional, offset + 2, 0, structlen - 2);
                    return new CRI((ConnectionType)contype, optional);
            }
        }
    }
}
