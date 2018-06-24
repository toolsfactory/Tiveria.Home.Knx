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

using System;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.Exceptions;

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
        public static int CRITUNNELSIZE = 4;

        #region properties
        private TunnelingLayer _layer;
        public TunnelingLayer Layer { get => _layer; }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new structure and initializes is with the layer
        /// </summary>
        /// <param name="layer">KNX tunnel connection link layer</param>
        public CRITunnel(TunnelingLayer layer) 
            : base (ConnectionType.TUNNEL_CONNECTION, new byte[2] { (byte)layer, 0x00 })
        {
            _layer = layer;
        }
        #endregion

        #region static methods
        /// <summary>
        /// Parses the referenced buffer and creates a new CRITunnel Structure from the information.
        /// </summary>
        /// <param name="buffer">THe buffer to be parsed</param>
        /// <param name="offset">THe offset the new structure begins in the buffer</param>
        /// <returns>New instance of CRITunnel</returns>
        public static new CRITunnel FromBuffer(byte[] buffer, int offset)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");
            var len = buffer.Length - offset;
            if (len != CRITUNNELSIZE)
                throw BufferSizeException.WrongSize("CRITunnel", CRITUNNELSIZE, len);
            if (buffer[offset + 1] != (byte)ConnectionType.TUNNEL_CONNECTION)
                throw new ArgumentException("buffer is not of connectiontype tunnel_connection");

            var layer = buffer[offset + 2];
            if (!Enum.IsDefined(typeof(TunnelingLayer), layer))
                throw BufferFieldException.TypeUnknown("TunnelingLayer", layer);
            return new CRITunnel((TunnelingLayer) layer);
        }
        #endregion
    }
}
