using System;
using Tiveria.Common.IO;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.IP.Services.Serializers;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.ObjectServer
{
    public class SetDatapointValueResService : IObjectServerService
    {
        public const byte MainService = 0xF0;
        public const byte SubService = 0x86;

        public int Size => 7;
        public ushort StartDataPoint { get; private set; }
        public byte ErrorCode { get; private set; }
        public ushort NumberOfDataPoints { get; private set; }
        public ushort ServiceTypeIdentifier => 0xF086;

        byte IObjectServerService.MainService => MainService;

        byte IObjectServerService.SubService => SubService;

        public SetDatapointValueResService(ushort startDataPoint, ushort noOfDPs, byte errorCode = 0x00)
        {
            StartDataPoint = startDataPoint;
            ErrorCode = errorCode;
            NumberOfDataPoints = noOfDPs;
        }
    }
}