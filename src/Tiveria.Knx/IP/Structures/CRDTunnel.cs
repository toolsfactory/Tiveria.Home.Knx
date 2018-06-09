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

namespace Tiveria.Knx.IP.Structures
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
        public static int CRDTUNNELSIZE = 4;

        #region properties
        private IndividualAddress _assignedAddress;
        public IndividualAddress AssignedAddress { get => _assignedAddress; }
        #endregion

        #region constructors
        /// <summary>
        /// Create a new CRDTunnel and initialize it with a specific individual address
        /// </summary>
        /// <param name="assignedAddress">The knx individual address assigned to the connection</param>
        public CRDTunnel(IndividualAddress assignedAddress)
            : base(ConnectionType.TUNNEL_CONNECTION, 
                   new byte[2] { (byte)(assignedAddress.RawAddress >> 8), (byte)assignedAddress.RawAddress } )
        {
            _assignedAddress = assignedAddress;   
        }
        #endregion

        #region static methods
        /// <summary>
        /// Parses the referenced buffer and creates a new CRDTunnel Structure from the information.
        /// </summary>
        /// <param name="buffer">THe buffer to be parsed</param>
        /// <param name="offset">THe offset the new structure begins in the buffer</param>
        /// <returns>New instance of CRDTunnel</returns>
        public static new CRDTunnel FromBuffer(byte[] buffer, int offset)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");
            var len = buffer.Length - offset;
            if (len != CRDTUNNELSIZE)
                throw BufferSizeException.WrongSize("CRDTunnel", CRDTUNNELSIZE, len);
            if (buffer[offset + 1] != (byte)ConnectionType.TUNNEL_CONNECTION)
                throw BufferFieldException.WrongValueAt("CRDTunnel", "ConnectionType", offset + 1);
            var assignedAddress = IndividualAddress.FromBuffer(buffer, offset + 2);
            return new CRDTunnel(assignedAddress);
        }
        #endregion
    }
}
