using System;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.ObjectServer
{
    public class SetDatapointValueResService : IObjectServerService
    {
        public const byte MainService = 0xF0;
        public const byte SubService = 0x86;

        public int Size => 7;
        public ushort ServiceIdentifier => 0xF086;

        public ushort StartDataPoint { get; private set; }
        public byte ErrorCode { get; private set; }
        public ushort NumberOfDataPoints { get; private set; }

        public SetDatapointValueResService(ushort startDataPoint, byte errorCode = 0x00)
        {
            StartDataPoint = startDataPoint;
            ErrorCode = errorCode;
        }

        public SetDatapointValueResService(Span<byte> data)
        {
            ParseHeader(data);
        }

        private void ParseHeader(Span<byte> rawdata)
        {
            if (rawdata.Length < 6)
                KnxBufferSizeException.TooSmall("SetDatapointReqService");

            if (rawdata[0] != MainService || rawdata[1] != SubService)
                KnxBufferFieldException.WrongValueAt("SetDatapointReqService", "Main/Subervice", 0);

            StartDataPoint = (ushort)((rawdata[2] << 8) + rawdata[3]);
            NumberOfDataPoints = (ushort)((rawdata[4] << 8) + rawdata[5]);

            if (NumberOfDataPoints != 0)
                KnxBufferFieldException.WrongValue("NumberOfDataPoints in SetDatapointValue.Res", 0, NumberOfDataPoints);
            ErrorCode = rawdata[6];
        }

        public byte[] ToBytes()
        {
            if (Size == 0)
                return null;
            var buffer = new byte[Size];
            WriteToBuffer(buffer);
            return buffer;
        }

        public void WriteToBuffer(Span<byte> buffer)
        {
            buffer[0] = SetDatapointValueResService.MainService;
            buffer[1] = SetDatapointValueResService.SubService;
            buffer[2] = (byte)(StartDataPoint >> 8);
            buffer[3] = (byte)StartDataPoint;
            buffer[4] = 0;
            buffer[5] = 0;
            buffer[6] = ErrorCode;
        }
    }
}
