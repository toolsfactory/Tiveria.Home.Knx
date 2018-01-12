using System;
using System.Net;
using Tiveria.Knx.IP.Utils;

namespace Tiveria.Knx.IP.Structures
{

    public class HPAI : StructureBase
    {
        private static byte HPAI_SIZE = 8;
        private IPAddress _ip;
        private ushort _port;
        private HPAIEndpointType _endpointType;

        public HPAI(HPAIEndpointType ept, IPAddress address, ushort port)
        {
            if (address != null)
            {
                if (address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                    throw new ArgumentOutOfRangeException("not an IPv4 address");
                if (IPAddress.IsLoopback(address))
                    throw new ArgumentException("IPv4 loopback address");
            } else {
                address = IPAddress.Parse("0.0.0.0");
                port = 0;
            }
            _endpointType = ept;
            _ip = address;
            _port = port;
            _structureLength = HPAI_SIZE;
        }

        public override void WriteToByteArray(ref byte[] buffer, ushort start)
        {
            base.WriteToByteArray(ref buffer, start);
            buffer[start + 0] = HPAI.HPAI_SIZE;
            buffer[start + 1] = (byte)_endpointType;
            _ip.GetAddressBytes().CopyTo(buffer, start + 2);
            buffer[start + 6] = (byte)(_port >> 8);
            buffer[start + 7] = (byte)_port;
        }

        public static HPAI Parse(ref byte[] buffer, ushort start)
        {
            return null;
        }
    }
}
