using System;

namespace Tiveria.Knx.IP.Structures
{
    public abstract class StructureBase
    {
        protected int _structureLength;
        public int StructureLength { get => _structureLength; }

        public byte[] ToBytes()
        {
            var data = new byte[StructureLength];
            WriteToByteArray(ref data, 0);
            return data;
        }

        public virtual void WriteToByteArray(ref byte[] buffer, int offset = 0)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");
            if (offset + _structureLength > buffer.Length)
                throw new ArgumentOutOfRangeException("buffer too small");
        }
    }
}
