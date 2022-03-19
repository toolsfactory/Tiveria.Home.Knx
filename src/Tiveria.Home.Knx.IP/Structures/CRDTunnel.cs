/*, ushort start
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

using Tiveria.Home.Knx.Primitives;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Common.IO;

namespace Tiveria.Home.Knx.IP.Structures
{
    /// <summary>
    /// Immutable representation of the connection response data block (CRD) for a tunneling connection.
    /// Official KNX Documentation: "03_04_08 Tunneling v01.05.03 AS.pdf" -> 4.4.4.4
    /// </summary>
    /// <code>
    /// +--------+--------+--------+--------+
    /// | byte 1 | byte 2 | byte 3 | byte 4 |
    /// +--------+--------+--------+--------+
    /// |  Size  |Con_Type| Assigned Address|
    /// |  0x04  | 0x04   | KNX Individual A|
    /// +--------+--------+--------+--------+
    /// </code>
    public class CRDTunnel : CRD
    {
        public static int STRUCTURE_SIZE = 4;

        #region properties
        public IndividualAddress AssignedAddress { get; init; }
        #endregion

        #region constructors
        /// <summary>
        /// Create a new CRDTunnel and initialize it with a specific individual address
        /// </summary>
        /// <param name="connectionType">Connection Type details</param>
        /// <param name="assignedAddress">The knx individual address assigned to the connection</param>
        public CRDTunnel(ConnectionType connectionType, IndividualAddress assignedAddress)
            : base(connectionType, assignedAddress.ToBytes())
        {
            ConnectionType = connectionType;
            AssignedAddress = assignedAddress;
            Size = STRUCTURE_SIZE;
        }
        #endregion

        /*
        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write((byte)Size);
            writer.Write((byte)ConnectionType);
            AssignedAddress.Write(writer);
        }
        */

        #region static methods
        /// <summary>
        /// Parses the referenced buffer and creates a new CRDTunnel Structure from the information.
        /// </summary>
        /// <param name="reader">An instance of a <see cref="BigEndianBinaryReader"/></param>
        /// <returns>New instance of CRDTunnel</returns>


        public static CRDTunnel Parse(BigEndianBinaryReader reader)
        {
            var size = reader.ReadSizeAndCheck("CRDTunnel", STRUCTURE_SIZE);
            var contype = reader.ReadByteEnum<ConnectionType>("CRD");
            var assignedAddress = new IndividualAddress(reader.ReadUInt16());

            if (contype != ConnectionType.Tunnel)
                throw KnxBufferFieldException.WrongValue("CRDTunnel", (byte)ConnectionType.Tunnel, (byte)contype);

            return new CRDTunnel(contype, assignedAddress);
        }
        #endregion
    }
}
