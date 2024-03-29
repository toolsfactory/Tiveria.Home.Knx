﻿using Tiveria.Common.IO;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.IP.Services
{
    public class RawService : IKnxNetIPService
    {
        public int Size { get; init; }
        public byte[] Payload { get; init; }
        public ushort ServiceTypeIdentifier { get; init; }


        public RawService(ushort serviceTypeIdentifier, byte[]? body)
        {
            ServiceTypeIdentifier = serviceTypeIdentifier;
            Payload = body ?? Array.Empty<byte>();
            Size = (ushort) (Payload.Length);
        }
    }
}