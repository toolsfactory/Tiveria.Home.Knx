using Tiveria.Common.IO;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.IP.Frames
{
    public class RawFrame : IKnxNetIPFrame
    {
        public int Size { get; init; }
        public byte[] Payload { get; init; }
        public FrameHeader FrameHeader { get; init; }
        public ServiceTypeIdentifier ServiceTypeIdentifier { get; init; }

        public RawFrame(ServiceTypeIdentifier servicetypeidentifier, byte[]? body)
           : this (new FrameHeader(servicetypeidentifier, (ushort)((body != null) ? body.Length : 0)), body)
        { }

        public RawFrame(FrameHeader frameHeader , byte[]? body)
        {
            FrameHeader = frameHeader;
            ServiceTypeIdentifier = frameHeader.ServiceTypeIdentifier;
            Payload = body ?? Array.Empty<byte>();
            Size = (ushort) (FrameHeader.Size + Payload.Length);
        }
    }
}