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

using Tiveria.Common.Extensions;
using Tiveria.Knx.IP.Structures;
using Tiveria.Knx.IP.Utils;

namespace Tiveria.Knx.IP.ServiceTypes
{
    /// <summary>
    /// Represents a KnxNetIP connection request message
    /// </summary>
    public class ConnectionRequest : ServiceTypeBase, IServiceType
    {
        private HPAI _discoveryHPAI;
        private HPAI _dataHPAI;
        private CRI _cri;

        public HPAI DiscoveryHPAI { get => _discoveryHPAI; }
        public HPAI DataHPAI { get => _dataHPAI; }
        public CRI Cri { get => _cri; }

        protected ConnectionRequest()
            : base(ServiceTypeIdentifier.CONNECT_REQUEST)
        { }
        /// <summary>
        /// Creates a ConnectRequest instance based on the values specified in the parameter objects
        /// </summary>
        /// <param name="requestInfo">Additional configuration options depending on the connection request type</param>
        /// <param name="discoverEndpoint">address of the udp endpoint taking care of discovery and control messages</param>
        /// <param name="dataEndpoint">address of the udp endpoint taking care of data messages</param>
        /// <remarks>Both the discoveryEndpoint and the dataEndpoint can be equal or even the same object. This results in one udpclient handling both parts.</remarks>
        public ConnectionRequest(CRI requestInfo, HPAI discoveryEndpoint, HPAI dataEndpoint)
            : this()
        {
            _cri = requestInfo;
            _discoveryHPAI = discoveryEndpoint;
            _dataHPAI = dataEndpoint;
            _structureLength = _cri.StructureLength + _discoveryHPAI.StructureLength + _dataHPAI.StructureLength;
        }

        public override void WriteToByteArray(ref byte[] buffer, int offset = 0)
        {
            base.WriteToByteArray(ref buffer, offset);
            _cri.WriteToByteArray(ref buffer, offset);
            _discoveryHPAI.WriteToByteArray(ref buffer, offset += _cri.StructureLength);
            _dataHPAI.WriteToByteArray(ref buffer, offset += _discoveryHPAI.StructureLength);
        }

        public static ConnectionRequest FromBuffer(ref byte[] buffer, int offset = 0)
        {
            var disEP = HPAI.FromBuffer(ref buffer, offset);
            var datEP = HPAI.FromBuffer(ref buffer, offset += disEP.StructureLength);
            var cri = CRI.FromBuffer(ref buffer, offset+= datEP.StructureLength);
            return new ConnectionRequest(cri, disEP, datEP);
        }
    }
}
