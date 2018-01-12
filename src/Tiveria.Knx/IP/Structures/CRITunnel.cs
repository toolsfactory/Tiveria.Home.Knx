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
    public class CRITunnel : CRI
    {
        public static int CRITUNNELSIZE = 4;

        private TunnelingLayer _layer;
        public TunnelingLayer Layer { get => _layer; }

        public CRITunnel(TunnelingLayer layer) 
            : base (ConnectionType.TUNNEL_CONNECTION, new byte[2] { (byte)layer, 0x00 })
        {
            _layer = layer;
        }

        public static new CRITunnel FromBuffer(ref byte[] buffer, int offset)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");
            var len = buffer.Length - offset;
            if (len != CRITUNNELSIZE)
                StructureBufferSizeException.WrongSize("CRITunnel", CRITUNNELSIZE, len);
            if (buffer[offset] != (byte)ConnectionType.TUNNEL_CONNECTION)

                throw new ArgumentException("buffer is not of connectiontype tunnel_connection");

            var layer = buffer[offset + 1];
            if (!Enum.IsDefined(typeof(TunnelingLayer), layer))
                ValueInterpretationException.TypeUnknown("TunnelingLayer", layer);

            return new CRITunnel((TunnelingLayer) layer);
        }

    }
}
