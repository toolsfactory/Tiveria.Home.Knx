using System;
using Tiveria.Knx.IP.Utils;

namespace Tiveria.Knx.IP.Structures
{
    public class Header : StructureBase
    {
        #region Constants
        public static byte KNXNETIP_VERSION_10 = 0x10;
        public static byte HEADER_SIZE_10 = 0x06;
        #endregion

        #region private fields
        private ServiceTypeIdentifier _serviceTypeIdentifier;
        private byte _size;
        private byte _version;
        private int _totalLength;
        #endregion

        #region public properties
        public byte Size { get => _size; }
        public byte Version { get => _version; }
        public ServiceTypeIdentifier ServiceTypeIdentifier { get => _serviceTypeIdentifier; }
        public int TotalLength { get => _totalLength; }
        #endregion

        #region Constructors
        public Header(byte version, byte size, ServiceTypeIdentifier servicetypeidentifier, ushort bodyLength)
        {
            if (version != KNXNETIP_VERSION_10)
                throw new ArgumentOutOfRangeException("Only version 0x10 of KnxNetIP is supported");
            if (size != HEADER_SIZE_10)
                throw new ArgumentOutOfRangeException("Only size 0x06 is supported in version 1 of the protocol");
            if(bodyLength<1)
                throw new ArgumentOutOfRangeException("Body size too small");
            if (bodyLength > 26)
                throw new ArgumentOutOfRangeException("Body size too big");

            _size = size;
            _version = version;
            _totalLength = _size + bodyLength;
            _serviceTypeIdentifier = servicetypeidentifier;
            _structureLength = _size;
        }

        public Header(ServiceTypeIdentifier servicetypeidentifier, ushort bodyLength)
            : this(KNXNETIP_VERSION_10, HEADER_SIZE_10, servicetypeidentifier, bodyLength)
        { }
        #endregion

        #region Static Parsing
        public static Header Parse(ref byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data is empty");
            if (data.Length < 6)
                throw new ArgumentException("data too small");

            if (data[0] != HEADER_SIZE_10)
                throw new ArgumentException("Invalid header size. 0x6 expected.");
            if (data[1] != KNXNETIP_VERSION_10)
                throw new ArgumentException("Invalid KNXNETIP Version. 0x10 expected.");

            var totalLength = (data[4] << 8) + data[5];
            if (totalLength != data.Length)
                throw new ArgumentException("data length and header totallength do not match");

            int sti = (data[2] << 8) + data[3];
            if (!Enum.IsDefined(typeof(ServiceTypeIdentifier), sti))
                throw new ArgumentException($"Unknown Servicetypeidentifier {sti:x}");

            return new Header((ServiceTypeIdentifier)sti, (ushort) (totalLength - HEADER_SIZE_10));
        }
        #endregion

        #region Public Methods
        public override void WriteToByteArray(ref byte[] buffer, ushort start)
        {
            base.WriteToByteArray(ref buffer, start);
            buffer[0] = HEADER_SIZE_10;
            buffer[1] = KNXNETIP_VERSION_10;
            buffer[2] = (byte)((int)ServiceTypeIdentifier >> 8);
            buffer[3] = (byte)(int)ServiceTypeIdentifier;
            buffer[4] = (byte)(_totalLength >> 8);
            buffer[5] = (byte)_totalLength;
        }
        #endregion

    }
}
