/*, ushort start
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

using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Common.IO;

namespace Tiveria.Home.Knx.IP.Structures
{
    /// <summary>
    /// Immutable container for a connection request information (CRI).
    /// The CRI structure is used for the additional information in a connection request.<br>
    /// </summary>
    public class CRI : DataElement
    {
        #region public properties
        public ConnectionType ConnectionType { get; init; }
        public byte[] OptionalData { get; init; }
        #endregion

        #region constructors
        public CRI(ConnectionType connectionType, byte[]? optionalData = null)
        {
            ConnectionType = connectionType;
            OptionalData = optionalData ?? Array.Empty<byte>();
            Size = (ushort)(2 + OptionalData.Length);
        }
        #endregion

        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write((byte)Size);
            writer.Write((byte)ConnectionType);
            if (OptionalData.Length > 0)
                writer.Write(OptionalData);
        }

        public static CRI Parse(BigEndianBinaryReader reader)
        {
            var size = reader.ReadByte();
            var contype = reader.ReadByteEnum<ConnectionType>("CRI.ConnectionType");
            var optional = reader.ReadBytes(size - 2);

            return new CRI(contype, optional);
        }

    }
}
