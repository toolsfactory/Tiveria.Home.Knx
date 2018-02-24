using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tiveria.Knx.Exceptions;
using Tiveria.Common.Extensions;

namespace Tiveria.Knx.Cemi
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
    /// 
    /// APCI is whether 4 bit or 10 bit long
    /// 4 bits: APDU Byte 1 bits 6+7 and APDU Byte 2 bits 0+1
    /// 10 bits: 4 bits as above and APDU byte 2 bits 2+3+4+5+6+7
    /// </summary>
    public struct Apci
    {
        private static readonly List<APCIEntry> RelevantAPCIEntry = new List<Cemi.APCIEntry>()
        {
            new Cemi.APCIEntry(0b0000, 0b000000, 10, APCIType.GroupValue_Read),
            new Cemi.APCIEntry(0b0001, 0xff,      4, APCIType.GroupValue_Response),
            new Cemi.APCIEntry(0b0010, 0xff,      4, APCIType.GroupValue_Write),
            new Cemi.APCIEntry(0b0011, 0b000000, 10, APCIType.IndividualAddress_Write),
            new Cemi.APCIEntry(0b0100, 0b000000, 10, APCIType.IndividualAddress_Read),
            new Cemi.APCIEntry(0b0101, 0b000000, 10, APCIType.IndividualAddress_Response),
            new Cemi.APCIEntry(0b0110, 0xff,      4, APCIType.ADC_Read),
            new Cemi.APCIEntry(0b0111, 0xff,      4, APCIType.ADC_Response),
            new Cemi.APCIEntry(0b1000, 0b00,      4, APCIType.Memory_Read),
            new Cemi.APCIEntry(0b1001, 0b00,      4, APCIType.Memory_Response),
            new Cemi.APCIEntry(0b1010, 0b00,      4, APCIType.Memory_Write)

            /*,
            new Cemi.APCIEntry(0b0111, 0b001000, 10, APCIType.SystemNetworkParameter_Read),
            new Cemi.APCIEntry(0b0111, 0b001001, 10, APCIType.SystemNetworkParameter_Resonse),
            new Cemi.APCIEntry(0b0111, 0b001010, 10, APCIType.SystemNetworkParameter_Write),
            new Cemi.APCIEntry(0b0111, 0b001011, 10, APCIType.Reserved_Broadcast),
            new Cemi.APCIEntry(0b1011, 0b000000, 10, APCIType.UserMemory_Read),
            new Cemi.APCIEntry(0b1011, 0b000001, 10, APCIType.UserMemory_Response),
            new Cemi.APCIEntry(0b1011, 0b000010, 10, APCIType.UserMemory_Write),
            new Cemi.APCIEntry(0b1011, 0b000100, 10, APCIType.UserMemoryBit_Write),
            new Cemi.APCIEntry(0b1011, 0b000101, 10, APCIType.UserManufacturerInfo_Read),
            new Cemi.APCIEntry(0b1011, 0b000110, 10, APCIType.UserManufacturerInfo_Response),
            new Cemi.APCIEntry(0b1011, 0b000111, 10, APCIType.FunctionPropertyCommand),
            new Cemi.APCIEntry(0b1011, 0b001000, 10, APCIType.FunctionPropertyState_Read),
            new Cemi.APCIEntry(0b1011, 0b001001, 10, APCIType.FunctionPropertyState_Response),
            new Cemi.APCIEntry(0b1100, 0b000000, 10, APCIType.DeviceDescriptor_Read),
            new Cemi.APCIEntry(0b1101, 0b000000, 10, APCIType.DeviceDescriptor_Response),
            new Cemi.APCIEntry(0b1110, 0b000000, 10, APCIType.Restart)


            */
        };

        private static byte DataMask6 = 0b00_111111;  //0x3f

        private byte[] _data;
        private byte _high;
        private byte _low;
        private APCIType _type;
        private byte[] _rawdata;

        public byte High => _high;
        public byte Low => _low;
        public byte[] Data => _data;
        public APCIType Type => _type;
        public Apci(byte[] buffer, int offset = 0)
        {
            _high = 0;
            _low = 0;
            _data = null;
            _rawdata = buffer.Clone(offset + 1, buffer.Length - offset - 1);
            _type = APCIType.Unknown;
            if (buffer.Length - offset < 2)
                throw BufferSizeException.TooSmall("APDU Structure must be at least 2 bytes");
            CreateApciHighLow(buffer[offset], buffer[offset + 1]);
            ParseApci();
        }

        private void CreateApciHighLow(byte apdu0, byte apdu1)
        {
            // high 4 bits of APCI
            _high = (byte)((apdu0 & 0x03) << 2 | (apdu1 & 0xC0) >> 6);
            // lowest 6 bits of APCI
            _low = (byte)(apdu1 & 0x3f);    
        }

        private void ParseApci()
        {
            // try to find a matching entry for the high bits
            var high = _high;
            var result1 = RelevantAPCIEntry.Where((item) => item.High == high);
            var count1 = result1?.Count();
            if (count1 == 0)
            {
                // no entry found
                _type = APCIType.Unknown;
                _data = _rawdata;
            }
            else if (count1 == 1)
            {
                // the entry found
                var item = result1.First();
                ExtractAcpiAndData(item);
            }
            else
            {
                // more than one entry, so search now on the low bits
                var low = _low;
                var result2 = result1.Where((item) => item.Low == low);
                var count2 = result2?.Count();
                if (count2 != 1)
                {
                    // no or more(not possible?) than one low bits combination found
                    _type = APCIType.Unknown;
                    _data = _rawdata;
                }
                else
                {
                    // the entry found
                    var item = result2.First();
                    ExtractAcpiAndData(item);
                }
            }
        }

        private void ExtractAcpiAndData(APCIEntry item)
        {
            _type = item.Type;
            switch (item.AcpiBits)
            {
                case 4:
                    // in case two or less bytes we assume size optimized data for GroupValue Read/Response otherwise not
                    if ((_type == APCIType.GroupValue_Write || _type == APCIType.GroupValue_Response) && _rawdata.Length > 2)
                        GetData10();
                    else
                        GetData4();
                    break;
                case 10:
                    GetData10();
                    break;
                default:
                    // just to show that in this case Data is not changed
                    _data = _rawdata;
                    break;
            }
        }

        private void GetData10()
        {
            _data = _rawdata.Clone(1, _rawdata.Length - 1);
        }

        private void GetData4()
        {
            _data = _rawdata.Clone(0, _rawdata.Length);
            _data[0] = (byte)(_data[0] & DataMask6);
        }
    }

    internal struct APCIEntry
    {
        public byte High { get; }
        public byte Low { get; }
        public byte AcpiBits { get; }
        public APCIType Type { get; }
        public APCIEntry(byte high, byte low, byte acpiBits, APCIType type)
        {
            Type = type;
            High = high;
            Low = low;
            AcpiBits = acpiBits;
        }
    }
    public enum APCIType {
        Unknown,
        GroupValue_Read,
        GroupValue_Response,
        GroupValue_Write,
        IndividualAddress_Write,
        IndividualAddress_Read,
        IndividualAddress_Response,
        ADC_Read,
        ADC_Response,
        SystemNetworkParameter_Read,
        SystemNetworkParameter_Resonse,
        SystemNetworkParameter_Write,
        Reserved_Broadcast,
        Memory_Read,
        Memory_Response,
        Memory_Write,
        UserMemory_Read,
        UserMemory_Response,
        UserMemory_Write,
        UserMemoryBit_Write,
        UserManufacturerInfo_Read,
        UserManufacturerInfo_Response,
        FunctionPropertyCommand,
        FunctionPropertyState_Read,
        FunctionPropertyState_Response,
        Reserved_UserMsg,
        Reserved_ManufacturerSpecific,
        DeviceDescriptor_Read,
        DeviceDescriptor_Response,
        Restart,
        OpenRoutingTable_Req,
        ReadRoutingTable_Req,
        ReadRoutingTable_Res,
        WriteRoutingTable_Req,
        ReadRouterMemory_Req,
        ReadRouterMemory_Res,
        WriteRouterMemory_Req,
        ReadRouterStatus_Req,
        ReadRouterStatus_Res,
        WriteRouterStatus_Req,
        MemoryBit_Write,
        Authorize_Request,
        Authorize_Response,
        Key_Write,
        Key_Response,
        PropertyValue_Read,
        PropertyValue_Response,
        PropertyValue_Write,
        PropertyDescription_Read,
        PropertyDescription_Response,
        NetworkParameter_Read,
        NetworkParameter_Response,
        IndividualAddressSerialNumber_Read,
        IndividualAddressSerialNumber_Response,
        IndividualAddressSerialNumber_Write,
        DomainAddress_Write,
        DomainAddress_Read,
        DomainAddress_Response,
        DomainAddressSelective_Read,
        NetworkParameter_Write,
        Link_Read,
        Link_Response,
        Link_Write,
        GroupPropValue_Read,
        GroupPropValue_Response,
        GroupPropValue_Write,
        GroupPropValue_InfoReport,
        DomainAddressSerialNumber_Read,
        DomainAddressSerialNumber_Response,
        DomainAddressSerialNumber_Write,
        FileStream_InforReport
    }
}
