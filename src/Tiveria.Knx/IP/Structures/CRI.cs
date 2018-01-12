using Tiveria.Knx.IP.Utils;

namespace Tiveria.Knx.IP.Structures
{
    // Connection Request Information
    public class CRI: StructureBase
    {
        public const int CRISize = 4;

        private ConnectionType _connectionType;
        private byte _layer;

        public ConnectionType ConnectionType { get => _connectionType; }
        public byte Layer { get => _layer; }

        public CRI(ConnectionType connectiontype, byte layer)
        {
            _structureLength = CRISize;
            _layer = layer;
            _connectionType = connectiontype;
        }

        public override void WriteToByteArray(ref byte[] buffer, ushort start)
        {
            base.WriteToByteArray(ref buffer, start);
            buffer[start + 0] = (byte)CRISize;
            buffer[start + 1] = (byte)_connectionType;
            buffer[start + 2] = (byte)_layer;
            buffer[start + 3] = 0x00; // reserved
        }
    }
}
