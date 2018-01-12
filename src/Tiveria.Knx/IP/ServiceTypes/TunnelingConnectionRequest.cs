using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Tiveria.Knx.IP.Structures;
using Tiveria.Knx.IP.Utils;

namespace Tiveria.Knx.IP.ServiceTypes
{
    public class TunnelingConnectionRequest : StructureBase, IServiceType
    {
        public IPAddress LocalEndpoint { get; set; }

        private HPAI _discoveryHPAI;
        private HPAI _dataHPAI;
        private CRI _cri; 

        public TunnelingConnectionRequest(IPAddress address, ushort port, TunnelingLayer layer = TunnelingLayer.TUNNEL_LINKLAYER) :
            this(address, port, address, port, layer)
        { }

        public TunnelingConnectionRequest(IPAddress address, ushort discoveryport, ushort dataport, TunnelingLayer layer = TunnelingLayer.TUNNEL_LINKLAYER) :
            this(address, discoveryport, address, dataport, layer)
        { }
        public TunnelingConnectionRequest(IPAddress discoveryip, ushort discoveryport, IPAddress dataip, ushort dataport, TunnelingLayer layer = TunnelingLayer.TUNNEL_LINKLAYER)
        {
            _discoveryHPAI = new HPAI(HPAIEndpointType.IPV4_UDP, discoveryip, discoveryport);
            _dataHPAI = new HPAI(HPAIEndpointType.IPV4_UDP, dataip, dataport);
            _cri = new CRI(ConnectionType.TUNNEL_CONNECTION, (byte)layer);
            _structureLength = _discoveryHPAI.StructureLength + _dataHPAI.StructureLength + _cri.StructureLength;
        }

        public override void WriteToByteArray(ref byte[] buffer, ushort start)
        {
            base.WriteToByteArray(ref buffer, start);
            _discoveryHPAI.WriteToByteArray(ref buffer, (ushort)(start + 0));
            _dataHPAI.WriteToByteArray(ref buffer, (ushort)(start + 8));
            _cri.WriteToByteArray(ref buffer, (ushort)(start + 16));
        }

        public static TunnelingConnectionRequest Parse(ref byte[] buffer, ushort start)
        {
            return null;
        }

        public static bool TryParse(ref byte[] buffer, out TunnelingConnectionRequest req)
        {
            req = null;
            try
            {
                return true;
            }
            catch { return false; }
        }

    }
}
