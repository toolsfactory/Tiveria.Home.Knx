using System;

namespace Tiveria.Home.Knx.ObjectServer
{
    public interface IObjectServerService
    {
        int Size { get; }
        ushort ServiceIdentifier { get; }
        byte[] ToBytes();
        void WriteToBuffer(Span<byte> buffer);
    }
}
