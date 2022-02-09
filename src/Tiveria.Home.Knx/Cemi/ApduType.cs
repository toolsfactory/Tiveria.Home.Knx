/*
    Tiveria.Home.Knx - a .Net Core base KNX library
    Copyright (c) 2018-2022 M. Geissler

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    Linking this library statically or dynamically with other modules is
    making a combined work based on this library. Thus, the terms and
    conditions of the GNU Lesser General Public License cover the whole
    combination.
*/

namespace Tiveria.Home.Knx.Cemi
{
    public static class ApduType
    {
        #region Public Apdu Type Constants
        public const int GroupValue_Read                        = 0b00_00;
        public const int GroupValue_Response                    = 0b00_01;
        public const int GroupValue_Write                       = 0b00_10;

        public const int IndividualAddress_Write                = 0b00_11;
        public const int IndividualAddress_Read                 = 0b01_00;
        public const int IndividualAddress_Response             = 0b01_01;

        public const int ADC_Read                               = 0b01_10;
        public const int ADC_Response                           = 0b01_11;

        public const int SystemNetworkParameter_Read            = 0b01_11001000;
        public const int SystemNetworkParameter_Response        = 0b01_11001001;
        public const int SystemNetworkParameter_Write           = 0b01_11001010;
        // Reserved for future use                              = 0b01_11001011

        public const int Memory_Read                            = 0b10_00;
        public const int Memory_Response                        = 0b10_01;
        public const int Memory_Write                           = 0b10_10;

        public const int UserMemory_Read                        = 0b10_11000000;
        public const int UserMemory_Response                    = 0b10_11000001;
        public const int UserMemory_Write                       = 0b10_11000010;

        public const int UserMemoryBit_Write                    = 0b10_11000100;

        public const int UserManufacturerInfo_Read              = 0b10_11000101;
        public const int UserManufacturerInfo_Response          = 0b10_11000110;

        public const int FunctionPropertyCommand                = 0b10_11000111;
        public const int FunctionPropertyState_Read             = 0b10_11001000;
        public const int FunctionPropertyState_Response         = 0b10_11001001;

        public const int DeviceDescriptor_Read                  = 0b11_00000000;
        public const int DeviceDescriptor_Response              = 0b11_01000000;  // Indentical to DeviceDescriptor Inforeport
        public const int Restart_Request                        = 0b11_10000000;  // No reponse forseen in protocol
        public const int RestartMasterReset_Request             = 0b11_10000001;  // subtype of Restart with extra flag set
        public const int RestartMasterReset_Response            = 0b11_10100001;  // subtype of Restart with extra flag set

        public const int Open_Routing_Table_Request             = 0b11_11000000;
        public const int Read_Routing_Table_Request             = 0b11_11000001;
        public const int Read_Routing_Table_Response            = 0b11_11000010;
        public const int Write_Routing_Table_Request            = 0b11_11000011;
        public const int Read_Router_Memory_Request             = 0b11_11001000;
        public const int Read_Router_Memory_Response            = 0b11_11001001;
        public const int Write_Router_Memory_Request            = 0b11_11001010;
        public const int Read_Router_Status_Request             = 0b11_11001101;
        public const int Read_Router_Status_Response            = 0b11_11001110;
        public const int Write_Router_Status_Request            = 0b11_11001111;

        public const int MemoryBit_Write                        = 0b11_11010000;

        public const int Authorize_Request                      = 0b11_11010001;
        public const int Authorize_Response                     = 0b11_11010010;
        public const int Key_Write                              = 0b11_11010011;
        public const int Key_Response                           = 0b11_11010100;

        public const int PropertyValue_Read                     = 0b11_11010101;
        public const int PropertyValue_Response                 = 0b11_11010110;
        public const int PropertyValue_Write                    = 0b11_11010111;
        public const int PropertyDescription_Read               = 0b11_11011000;
        public const int PropertyDescription_Response           = 0b11_11011001;

        public const int NetworkParameter_Read                  = 0b11_11011010;
        public const int NetworkParameter_Response              = 0b11_11011011;

        public const int IndividualAddressSerialNumber_Read     = 0b11_11011100;
        public const int IndividualAddressSerialNumber_Response = 0b11_11011101;
        public const int IndividualAddressSerialNumber_Write    = 0b11_11011110;
        // Reserved for future use                              = 0b11_11011111;

        public const int DomainAddress_Write                    = 0b11_11100000;
        public const int DomainAddress_Read                     = 0b11_11100001;
        public const int DomainAddress_Response                 = 0b11_11100010;
        public const int DomainAddressSelective_Read            = 0b11_11100011;

        public const int NetworkParameter_Write                 = 0b11_11100100;

        public const int Link_Read                              = 0b11_11_100101;
        public const int Link_Response                          = 0b11_11_100110;
        public const int Link_Write                             = 0b11_11_100111;

        public const int GroupPropertyValue_Read                = 0b11_11_101000;
        public const int GroupPropertyValue_Response            = 0b11_11_101001;
        public const int GroupPropertyValue_Write               = 0b11_11_101010;
        public const int GroupPropertyValue_InfoReport          = 0b11_11_101011;

        public const int DomainAddressSerialNumber_Read         = 0b11_11_101100;
        public const int DomainAddressSerialNumber_Response     = 0b11_11_101101;
        public const int DomainAddressSerialNumber_Write        = 0b11_11_101110;

        public const int FileStream_InfoReport                  = 0b11_11_110000;
        #endregion

        #region Public helpers
        public static void Register(int value, string name, DataMode dataMode, bool canOptimize = false, int exactormin = -1, int max = -1)
        {
            var len = (value <= 0b1111) ? ApciFieldLength.Bits_4 : ApciFieldLength.Bits_10;
            var high = (byte) (len == ApciFieldLength.Bits_4 ? value >> 2 : value >> 8);
            var low  = (byte) (len == ApciFieldLength.Bits_4 ? value << 6 : value & 0xff);
            _knowApduType.Add(value, new ApduTypeDetail(name, high, low, len, dataMode, canOptimize, exactormin, max));
        }  

        public static bool IsKnown(int id) => _knowApduType.ContainsKey(id);

        public static bool RequiresData(int id) => IsKnown(id) ? _knowApduType[id].DataMode != DataMode.None : false;

        public static (DataMode Mode, int MinOrExact, int Max) GetRequiredDataDetails(int id)
            => IsKnown(id) ? (_knowApduType[id].DataMode, _knowApduType[id].ExactOrMin, _knowApduType[id].Max) : (DataMode.Unknown, -1, -1);

        public static int GetApduType((int apci4, int apci6, int len) apci) => apci switch
        {
            { apci4: 0, apci6: 0 } => ApduType.GroupValue_Read,
            { apci4: 1 } => ApduType.GroupValue_Response,
            { apci4: 2 } => ApduType.GroupValue_Write,
            { apci4: 3, apci6: 0 } => ApduType.GroupValue_Write,
            { apci4: 4, apci6: 0 } => ApduType.GroupValue_Read,
            { apci4: 5, apci6: 0 } => ApduType.GroupValue_Response,
            { apci4: 6 } => ApduType.ADC_Read,
            { apci4: 7, len: 5 } => ApduType.ADC_Response,
            { apci4: 7, apci6: 8, len: > 5 } => ApduType.SystemNetworkParameter_Read,
            { apci4: 7, apci6: 9, len: > 5 } => ApduType.SystemNetworkParameter_Response,
            { apci4: 7, apci6: 10, len: > 5 } => ApduType.SystemNetworkParameter_Write,
            { apci4: 8 } => ApduType.Memory_Read,
            { apci4: 9 } => ApduType.Memory_Response,
            { apci4: 10 } => ApduType.Memory_Write,
            _ => apci.apci4 << 6 | apci.apci6
        };

        public static (int offset, int mask) GetASDUMaskAndOffset((int apci, int len) x) => x switch
        {
            { apci: ApduType.GroupValue_Response, len: <= 2 } => (1, CompressedDataMask),
            { apci: ApduType.GroupValue_Write, len: <= 2 } => (1, CompressedDataMask),
            { apci: ApduType.ADC_Read } => (1, CompressedDataMask),
            { apci: ApduType.ADC_Response } => (1, CompressedDataMask),
            { apci: ApduType.Memory_Read } => (1, CompressedDataMask),
            { apci: ApduType.Memory_Response } => (1, CompressedDataMask),
            { apci: ApduType.Memory_Write } => (1, CompressedDataMask),
            { apci: ApduType.DeviceDescriptor_Read } => (1, CompressedDataMask),
            { apci: ApduType.DeviceDescriptor_Response } => (1, CompressedDataMask),
            { apci: ApduType.IndividualAddress_Read } => (1, CompressedDataMask),
            { apci: ApduType.IndividualAddress_Response } => (1, CompressedDataMask),
            _ => (2, 0b_11111111)
        };

        public static string ToString(int id) => IsKnown(id) ? _knowApduType[id].Name : "Unknown";
        #endregion

        #region internal helpers
        static ApduType()
        {
            Register(GroupValue_Read, "GroupValue_Read", DataMode.None);
            Register(GroupValue_Response, "GroupValue_Response", DataMode.MinMax, true, 1, 14);
            Register(GroupValue_Write, "GroupValue_Write", DataMode.MinMax, true, 1, 14);
            Register(IndividualAddress_Write, "IndividualAddress_Write", DataMode.Exact, false, 2);
            Register(IndividualAddress_Read, "IndividualAddress_Read", DataMode.None);
            Register(IndividualAddress_Response, "IndividualAddress_Response", DataMode.None);
            Register(ADC_Read, "ADC_Read", DataMode.Exact, true, 2);
            Register(ADC_Response, "ADC_Response", DataMode.Exact, true, 4);
            Register(SystemNetworkParameter_Read, "SystemNetworkParameter_Read", DataMode.Min, false, 4);
            Register(SystemNetworkParameter_Response, "SystemNetworkParameter_Response", DataMode.Min, false, 6);
            Register(SystemNetworkParameter_Write, "SystemNetworkParameter_Write", DataMode.Min, false, 4);
            Register(Memory_Read, "Memory_Read", DataMode.Exact, true, 3);
            Register(Memory_Response, "Memory_Response", DataMode.Min, true, 3);
            Register(Memory_Write, "Memory_Write", DataMode.Min, true, 4);
            Register(UserMemory_Read, "UserMemory_Read", DataMode.Exact, false, 3);
            Register(UserMemory_Response, "UserMemory_Response", DataMode.Min, false, 3);
            Register(UserMemory_Write, "UserMemory_Write", DataMode.Min, false, 3);
            Register(UserMemoryBit_Write, "UserMemoryBit_Write", DataMode.Min, false, 5);
            Register(UserManufacturerInfo_Read, "UserManufacturerInfo_Read", DataMode.None);
            Register(UserManufacturerInfo_Response, "UserManufacturerInfo_Response", DataMode.Exact, false, 3);
            Register(FunctionPropertyCommand, "FunctionPropertyCommand", DataMode.Min, false, 3);
            Register(FunctionPropertyState_Read, "FunctionPropertyState_Read", DataMode.Min, false, 3);
            Register(FunctionPropertyState_Response, "FunctionPropertyState_Response", DataMode.Min, false, 3);
            Register(DeviceDescriptor_Read, "DeviceDescriptor_Read", DataMode.Exact, true, 1);
            Register(DeviceDescriptor_Response, "DeviceDescriptor_Response", DataMode.Min, true, 2);
            Register(Restart_Request, "Restart_Request", DataMode.None);
            Register(RestartMasterReset_Request, "RestartMasterReset_Request", DataMode.Exact, false, 2);
            Register(RestartMasterReset_Response, "RestartMasterReset_Response", DataMode.Exact, false, 3);
            Register(Open_Routing_Table_Request, "Open_Routing_Table_Request", DataMode.None);
            Register(Read_Routing_Table_Request, "Read_Routing_Table_Request", DataMode.None);
            Register(Read_Routing_Table_Response, "Read_Routing_Table_Response", DataMode.None);
            Register(Write_Routing_Table_Request, "Write_Routing_Table_Request", DataMode.None);
            Register(Read_Router_Memory_Request, "Read_Router_Memory_Request", DataMode.None);
            Register(Read_Router_Memory_Response, "Read_Router_Memory_Response", DataMode.None);
            Register(Write_Router_Memory_Request, "Write_Router_Memory_Request", DataMode.None);
            Register(Read_Router_Status_Request, "Read_Router_Status_Request", DataMode.None);
            Register(Read_Router_Status_Response, "Read_Router_Status_Response", DataMode.None);
            Register(Write_Router_Status_Request, "Write_Router_Status_Request", DataMode.None);
            Register(MemoryBit_Write, "MemoryBit_Write", DataMode.Min, false, 6);
            Register(Authorize_Request, "Authorize_Request", DataMode.Exact, false, 5);
            Register(Authorize_Response, "Authorize_Response", DataMode.Exact, false, 1);
            Register(Key_Write, "Key_Write", DataMode.Exact, false, 5);
            Register(Key_Response, "Key_Response", DataMode.Exact, false, 1);
            Register(PropertyValue_Read, "PropertyValue_Read", DataMode.Exact, false, 4);
            Register(PropertyValue_Response, "PropertyValue_Response", DataMode.Min, false, 4);
            Register(PropertyValue_Write, "PropertyValue_Write", DataMode.Min, false, 4);
            Register(PropertyDescription_Read, "PropertyDescription_Read", DataMode.Exact, false, 3);
            Register(PropertyDescription_Response, "PropertyDescription_Response", DataMode.Exact, false, 7);
            Register(NetworkParameter_Read, "NetworkParameter_Read", DataMode.Min, false, 4);
            Register(NetworkParameter_Response, "NetworkParameter_Response", DataMode.MinMax, false, 4, 14);
            Register(IndividualAddressSerialNumber_Read, "IndividualAddressSerialNumber_Read", DataMode.Exact, false, 6);
            Register(IndividualAddressSerialNumber_Response, "IndividualAddressSerialNumber_Response", DataMode.Exact, false, 10);
            Register(IndividualAddressSerialNumber_Write, "IndividualAddressSerialNumber_Write", DataMode.Exact, false, 12);
            Register(DomainAddress_Write, "DomainAddress_Write", DataMode.MinMax, false, 2, 6);
            Register(DomainAddress_Read, "DomainAddress_Read", DataMode.None);
            Register(DomainAddress_Response, "DomainAddress_Response", DataMode.MinMax, false, 2, 6);
            Register(DomainAddressSelective_Read, "DomainAddressSelective_Read", DataMode.Min, false, 1);
            Register(NetworkParameter_Write, "NetworkParameter_Write", DataMode.Min, false, 4);
            Register(Link_Read, "Link_Read", DataMode.Exact, false, 2);
            Register(Link_Response, "Link_Response", DataMode.MinMax, false, 2, 14);
            Register(Link_Write, "Link_Write", DataMode.Exact, false, 4);
            Register(GroupPropertyValue_Read, "GroupPropertyValue_Read", DataMode.None);
            Register(GroupPropertyValue_Response, "GroupPropertyValue_Response", DataMode.None);
            Register(GroupPropertyValue_Write, "GroupPropertyValue_Write", DataMode.None);
            Register(GroupPropertyValue_InfoReport, "GroupPropertyValue_InfoReport", DataMode.None);
            Register(DomainAddressSerialNumber_Read, "DomainAddressSerialNumber_Read", DataMode.Exact, false, 6);
            Register(DomainAddressSerialNumber_Response, "DomainAddressSerialNumber_Response", DataMode.Exact, false, 10);
            Register(DomainAddressSerialNumber_Write, "DomainAddressSerialNumber_Write", DataMode.Exact, false, 12);
            Register(FileStream_InfoReport, "FileStream_InfoReport", DataMode.Min, false, 1);
        }

        private const int CompressedDataMask = 0b00_111111;

        private static Dictionary<int, ApduTypeDetail> _knowApduType = new Dictionary<int, ApduTypeDetail>(50);

        #endregion
    }

    public enum ApciFieldLength
    {
        Bits_4,
        Bits_10
    }

    public enum DataMode
    {
        None,
        Exact,
        Min,
        MinMax,
        Unknown
    }

    public record struct ApduTypeDetail(string Name, byte HighBits, byte LowBits, ApciFieldLength Length, DataMode DataMode, bool CanOptimize, int ExactOrMin, int Max);

}
