using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.EMI
{
    /// <summary>
    /// For data that is 6 bits or less in length, only the first two bytes are used in a Common EMI
    /// frame. Common EMI frame also carries the information of the expected length of the Protocol
    /// Data Unit (PDU). Data payload can be at most 14 bytes long.  <p>
    /// 
    /// The first byte is a combination of transport layer control information (TPCI) and application
    /// layer control information (APCI). First 6 bits are dedicated for TPCI while the two least
    /// significant bits of first byte hold the two most significant bits of APCI field, as follows:
    /// <code>
    /// +-----------------------------------------------------------------------+
    /// |          APDU Byte 1: 6 bit TPCI & 2 bit APCI                         |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// | bit 0  | bit 1  | bit 2  | bit 3  | bit 4  | bit 5  | bit 6  | bit 7  |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// |                       TPCI Data                     |    APCI         |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// 
    /// +-----------------------------------------------------------------------+
    /// |          APDU Byte 2: 2 bit APCI & 6bit Data or APCI                  |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// | bit 0  | bit 1  | bit 2  | bit 3  | bit 4  | bit 5  | bit 6  | bit 7  |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// |    APCI         |          ACPI or Data                               |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// </code>
    /// 
    /// APCI is whether 4 bit or 10 bit long
    /// 4 bits: APDU Byte 1 bits 6+7 and APDU Byte 2 bits 0+1
    /// 10 bits: 4 bits as above and APDU byte 2 bits 2+3+4+5+6+7
    /// </summary>
    /// 

    public struct Apci
    {
        private ApciTypeDetail _TypeDetails;

        public int Size { get; private set; } = 2;
        public byte[] Data { get; init; } = Array.Empty<byte>();
        public ApciTypes Type { get; init; } = ApciTypes.GroupValue_Read;

        public Apci(ApciTypes type)
            : this(type, Array.Empty<byte>())
        { }

        public Apci(ApciTypes type, byte[] data)
        {
            Type = type;
            Data = data ?? Array.Empty<byte>();

            if (!ApciHelper.RelevantAPCITypes.TryGetValue((int)Type, out _TypeDetails))
                throw new NotSupportedException($"ApciType {Type} is not supported");

            if (_TypeDetails.Mode == DataMode.Required && Data.Length == 0)
                throw new ArgumentException($"Data cannot be null or empty for Apci Type {_TypeDetails.Type}");

            // Ensure that Data is empty for types that dont allow data
            if(_TypeDetails.Mode == DataMode.None && Data.Length > 0)
                Data = Array.Empty<byte>();

            Size = (_TypeDetails.CanOptimize && (Data.Length) == 1 && (Data[0] <= 63)) ? 2 : Data.Length + 2;
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[Size];
            WriteBytes(bytes);
            return bytes;
        }

        public int WriteBytes(Span<byte> buffer)
        {
            if (buffer.Length < Size)
                throw BufferSizeException.TooSmall(nameof(buffer));

            buffer[0] = _TypeDetails.HighBits;
            buffer[1] = _TypeDetails.LowBits;

            if (_TypeDetails.CanOptimize && Data.Length == 1 && Data[0] <= 63)
                buffer[1] |= Data[0];
            else
                Data.CopyTo(buffer.Slice(2));

            return Size;
        }

        public static Apci Parse(Span<byte> buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("data");
            if (buffer.Length < 2)
                BufferSizeException.TooSmall("data");

            var details = ApciHelper.GetTypeDetailsFromBytes(buffer);
            var data = ExtractAcpiData(details, buffer);
            return new Apci(details.Type, data);

            static byte[] ExtractAcpiData(ApciTypeDetail details, Span<byte> data)
            {
                if (details.CanOptimize && data.Length == 2)
                    return new[] { (byte)(data[1] & ApciHelper.DataMask6) };
                if (data.Length == 2)
                    return Array.Empty<byte>();
                return data.Slice(2).ToArray();
            }
        }
    }
}
