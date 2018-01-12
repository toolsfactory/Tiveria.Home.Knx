using System;
using System.Net;
using Tiveria.Knx.IP.Utils;
using Tiveria.Knx.Exceptions;
using Tiveria.Common.Extensions;

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

        public override void WriteToByteArray(ref byte[] buffer, int offset)
        {
            base.WriteToByteArray(ref buffer, offset);
            buffer[offset + 0] = HPAI.HPAI_SIZE;
            buffer[offset + 1] = (byte)_endpointType;
            _ip.GetAddressBytes().CopyTo(buffer, offset + 2);
            buffer[offset + 6] = (byte)(_port >> 8);
            buffer[offset + 7] = (byte)_port;
        }

        public static HPAI FromBuffer(ref byte[] buffer, int offset =  0)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");

            if (buffer.Length - offset < HPAI.HPAI_SIZE)
                StructureBufferSizeException.TooSmall("HPAI");

            var structlen = buffer[offset];
            if (structlen != HPAI.HPAI_SIZE)
                ValueInterpretationException.WrongValue("HPAI.Size", HPAI.HPAI_SIZE, structlen);

            var endpointType = buffer[offset + 1];
            if (!Enum.IsDefined(typeof(HPAIEndpointType), endpointType))
                ValueInterpretationException.TypeUnknown("EndpointType", endpointType);

            var ipbytes = new byte[4];
            buffer.Slice(ipbytes, offset + 2, 0, 4);
            var port = buffer[offset + 6] << 8 + buffer[offset + 7];

            return new HPAI((HPAIEndpointType)endpointType, new IPAddress(ipbytes), (ushort)port);
        }
    }
}
