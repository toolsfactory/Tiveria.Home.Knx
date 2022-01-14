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
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Common.IO;

namespace Tiveria.Home.Knx.IP.Structures
{
    /// <summary>
    /// Immutable representation of the connection request information block (CRI) for a tunneling connection.
    /// Official KNX Documentation: "03_04_08 Tunneling v01.05.03 AS.pdf" -> 4.4.4.3
    /// </summary>
    /// <code>
    /// +--------+--------+--------+--------+
    /// | byte 1 | byte 2 | byte 3 | byte 4 |
    /// +--------+--------+--------+--------+
    /// |  Size  |Con_Type| Layer  |Reserved|
    /// |  0x04  | 0x04   |        |  0x00  |
    /// +--------+--------+--------+--------+
    /// </code>
    public class CRITunnel : CRI
    {
        public static int STRUCTURE_SIZE = 4;

        #region properties
        public TunnelingLayer Layer { get; init; }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new structure and initializes is with the layer
        /// </summary>
        /// <param name="layer">KNX tunnel connection link layer</param>
        public CRITunnel(TunnelingLayer layer) 
            : base(ConnectionType.Tunnel)
        {
            Layer = layer;
            Size = STRUCTURE_SIZE;
        }
        #endregion

        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write((byte)Size);
            writer.Write((byte)ConnectionType);
            writer.Write((byte)Layer);
            writer.Write((byte)0);
        }

        #region static methods
        /// <summary>
        /// Parses the referenced buffer and creates a new CRITunnel Structure from the information.
        /// </summary>
        /// <returns>New instance of CRITunnel</returns>
        public static CRITunnel Parse(BigEndianBinaryReader reader)
        {
            reader.ReadSizeAndCheck("CRITunnel", STRUCTURE_SIZE);
            var contype = reader.ReadByteEnum<ConnectionType>("CRITunnel.ConnectionType");
            var layer = reader.ReadByteEnum<TunnelingLayer>("CRITunnel.Layer");
            reader.ReadByte(); // reserved Byte

            if (contype != ConnectionType.Tunnel)
                throw BufferFieldException.WrongValue("CRITunnel.ConnectionType", (byte)ConnectionType.Tunnel, (byte)contype);

            return new CRITunnel(layer);
        }
        #endregion
    }
}
