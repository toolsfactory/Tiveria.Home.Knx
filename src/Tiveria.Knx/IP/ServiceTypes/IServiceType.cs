namespace Tiveria.Knx.IP.ServiceTypes
{
    public interface IServiceType
    {
        byte[] ToBytes();
        void WriteToByteArray(ref byte[] buffer, ushort start);
    }
}
