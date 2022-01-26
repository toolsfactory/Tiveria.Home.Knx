/*
    Tiveria.Home.Knx - a .Net Core base KNX library
    Copyright (c) 2018 M. Geissler

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    Linking this library statically or dynamically with other modules is
    making a combined work based on this library. Thus, the terms and
    conditions of the GNU General Public License cover the whole
    combination.
*/

namespace Tiveria.Home.Knx.Cemi
{
    public static class ApciTypes
    {
        public const int GroupValue_Read                        = 0b0000;
        public const int GroupValue_Response                    = 0b0001;
        public const int GroupValue_Write                       = 0b0010;

        public const int IndividualAddress_Write                = 0b0011;
        public const int IndividualAddress_Read                 = 0b0100;
        public const int IndividualAddress_Response             = 0b0101;

        public const int ADC_Read                               = 0b0110;
        public const int ADC_Response                           = 0b0111;

        public const int SystemNetworkParameter_Read            = 0b0111_001000;
        public const int SystemNetworkParameter_Response        = 0b0111_001001;
        public const int SystemNetworkParameter_Write           = 0b0111_001010;
        // Reserved for future use                              = 0b0111_001011

        public const int Memory_Read                            = 0b1000;
        public const int Memory_Response                        = 0b1001;
        public const int Memory_Write                           = 0b1010;

        public const int UserMemory_Read                        = 0b1011_000000;
        public const int UserMemory_Response                    = 0b1011_000001;
        public const int UserMemory_Write                       = 0b1011_000010;

        public const int UserMemoryBit_Write                    = 0b1011_000100;

        public const int UserManufacturerInfo_Read              = 0b1011_000101;
        public const int UserManufacturerInfo_Response          = 0b1011_000110;

        public const int FunctionPropertyCommand                = 0b1011_000111;
        public const int FunctionPropertyState_Read             = 0b1011_001000;
        public const int FunctionPropertyState_Write            = 0b1011_001001;

        public const int DeviceDescriptor_Read                  = 0b1100_000000;
        public const int DeviceDescriptor_Response              = 0b1101_000000;  // Indentical to DeviceDescriptor Inforeport
        public const int Restart_Request                        = 0b1110_000000;  // No reponse forseen in protocol
        public const int RestartMasterReset_Request             = 0b1110_000001;  // subtype of Restart with extra flag set
        public const int RestartMasterReset_Response            = 0b1110_100001;  // subtype of Restart with extra flag set

        public const int Open_Routing_Table_Request             = 0b1111_000000;
        public const int Read_Routing_Table_Request             = 0b1111_000001;
        public const int Read_Routing_Table_Response            = 0b1111_000010;
        public const int Write_Routing_Table_Request            = 0b1111_000011;
        public const int Read_Router_Memory_Request             = 0b1111_001000;
        public const int Read_Router_Memory_Response            = 0b1111_001001;
        public const int Write_Router_Memory_Request            = 0b1111_001010;
        public const int Read_Router_Status_Request             = 0b1111_001101;
        public const int Read_Router_Status_Response            = 0b1111_001110;
        public const int Write_Router_Status_Request            = 0b1111_001111;

        public const int MemoryBit_Write                        = 0b1111_010000;

        public const int Authorize_Request                      = 0b1111_010001;
        public const int Authorize_Response                     = 0b1111_010010;
        public const int Key_Write                              = 0b1111_010011;
        public const int Key_Response                           = 0b1111_010100;

        public const int PropertyValue_Read                     = 0b1111_010101;
        public const int PropertyValue_Response                 = 0b1111_010110;
        public const int PropertyValue_Write                    = 0b1111_010111;
        public const int PropertyDescription_Read               = 0b1111_011000;
        public const int PropertyDescription_Response           = 0b1111_011001;

        public const int NetworkParameter_Read                  = 0b1111_011010;
        public const int NetworkParameter_Response              = 0b1111_011011;

        public const int IndividualAddressSerialNumber_Read     = 0b1111_011100;
        public const int IndividualAddressSerialNumber_Response = 0b1111_011101;
        public const int IndividualAddressSerialNumber_Write    = 0b1111_011110;
        // Reserved for future use                              = 0b1111_011111;

        public const int DomainAddress_Write                    = 0b1111_100000;
        public const int DomainAddress_Read                     = 0b1111_100001;
        public const int DomainAddress_Response                 = 0b1111_100010;
        public const int DomainAddressSelective_Read            = 0b1111_100011;

        public const int NetworkParameter_Write                 = 0b1111_100100;

        public const int Link_Read                              = 0b1111_100101;
        public const int Link_Response                          = 0b1111_100110;
        public const int Link_Write                             = 0b1111_100111;

        public const int GroupPropertyValue_Read                = 0b1111_101000;
        public const int GroupPropertyValue_Response            = 0b1111_101001;
        public const int GroupPropertyValue_Write               = 0b1111_101010;
        public const int GroupPropertyValue_InfoReport          = 0b1111_101011;

        public const int DomainAddressSerialNumber_Read         = 0b1111_101100;
        public const int DomainAddressSerialNumber_Response     = 0b1111_101101;
        public const int DomainAddressSerialNumber_Write        = 0b1111_101110;

        public const int FileStream_InfoReport                  = 0b1111_110000;

        static ApciTypes()
        {
        Register(GroupValue_Read, "GroupValue_Read", false);
        Register(GroupValue_Response, "GroupValue_Response", true, true);
        Register(GroupValue_Write, "GroupValue_Write", true, true);
        Register(IndividualAddress_Write, "IndividualAddress_Write", true);
        Register(IndividualAddress_Read, "IndividualAddress_Read", false);
        Register(IndividualAddress_Response, "IndividualAddress_Response", false);
        Register(ADC_Read, "ADC_Read", true);
        Register(ADC_Response, "ADC_Response", true);
        Register(SystemNetworkParameter_Read, "SystemNetworkParameter_Read", true);
        Register(SystemNetworkParameter_Response, "SystemNetworkParameter_Response", true);
        Register(SystemNetworkParameter_Write, "SystemNetworkParameter_Write", true);
        Register(Memory_Read, "Memory_Read", true);
        Register(Memory_Response, "Memory_Response", true);
        Register(Memory_Write, "Memory_Write", true);
        Register(UserMemory_Read, "UserMemory_Read", true);
        Register(UserMemory_Response, "UserMemory_Response", true);
        Register(UserMemory_Write, "UserMemory_Write", true);
        Register(UserMemoryBit_Write, "UserMemoryBit_Write", true);
        Register(UserManufacturerInfo_Read, "UserManufacturerInfo_Read", false);
        Register(UserManufacturerInfo_Response, "UserManufacturerInfo_Response", true);
        Register(FunctionPropertyCommand, "FunctionPropertyCommand", true);
        Register(FunctionPropertyState_Read, "FunctionPropertyState_Read", true);
        Register(FunctionPropertyState_Write, "FunctionPropertyState_Write", true);
        Register(DeviceDescriptor_Read, "DeviceDescriptor_Read", true, true);
        Register(DeviceDescriptor_Response, "DeviceDescriptor_Response", true);
        Register(Restart_Request, "Restart_Request", false);
        Register(RestartMasterReset_Request, "RestartMasterReset_Request", true);
        Register(RestartMasterReset_Response, "RestartMasterReset_Response", true);
        Register(Open_Routing_Table_Request, "Open_Routing_Table_Request", false);
        Register(Read_Routing_Table_Request, "Read_Routing_Table_Request", false);
        Register(Read_Routing_Table_Response, "Read_Routing_Table_Response", false);
        Register(Write_Routing_Table_Request, "Write_Routing_Table_Request", false);
        Register(Read_Router_Memory_Request, "Read_Router_Memory_Request", false);
        Register(Read_Router_Memory_Response, "Read_Router_Memory_Response", false);
        Register(Write_Router_Memory_Request, "Write_Router_Memory_Request", false);
        Register(Read_Router_Status_Request, "Read_Router_Status_Request", false);
        Register(Read_Router_Status_Response, "Read_Router_Status_Response", false);
        Register(Write_Router_Status_Request, "Write_Router_Status_Request", false);
        Register(MemoryBit_Write, "MemoryBit_Write", true);
        Register(Authorize_Request, "Authorize_Request", true);
        Register(Authorize_Response, "Authorize_Response", true);
        Register(Key_Write, "Key_Write", true);
        Register(Key_Response, "Key_Response", true);
        Register(PropertyValue_Read, "PropertyValue_Read", true);
        Register(PropertyValue_Response, "PropertyValue_Response", true);
        Register(PropertyValue_Write, "PropertyValue_Write", false);
        Register(PropertyDescription_Read, "PropertyDescription_Read", true);
        Register(PropertyDescription_Response, "PropertyDescription_Response", true);
        Register(NetworkParameter_Read, "NetworkParameter_Read", true);
        Register(NetworkParameter_Response, "NetworkParameter_Response", true);
        Register(IndividualAddressSerialNumber_Read, "IndividualAddressSerialNumber_Read", true);
        Register(IndividualAddressSerialNumber_Response, "IndividualAddressSerialNumber_Response", true);
        Register(IndividualAddressSerialNumber_Write, "IndividualAddressSerialNumber_Write", true);
        Register(DomainAddress_Write, "DomainAddress_Write", true);
        Register(DomainAddress_Read, "DomainAddress_Read", false);
        Register(DomainAddress_Response, "DomainAddress_Response", true);
        Register(DomainAddressSelective_Read, "DomainAddressSelective_Read", true);
        Register(NetworkParameter_Write, "NetworkParameter_Write", true);
        Register(Link_Read, "Link_Read", true);
        Register(Link_Response, "Link_Response", true);
        Register(Link_Write, "Link_Write", true);
        Register(GroupPropertyValue_Read, "GroupPropertyValue_Read", false);
        Register(GroupPropertyValue_Response, "GroupPropertyValue_Response", false);
        Register(GroupPropertyValue_Write, "GroupPropertyValue_Write", false);
        Register(GroupPropertyValue_InfoReport, "GroupPropertyValue_InfoReport", false);
        Register(DomainAddressSerialNumber_Read, "DomainAddressSerialNumber_Read", true);
        Register(DomainAddressSerialNumber_Response, "DomainAddressSerialNumber_Response", true);
        Register(DomainAddressSerialNumber_Write, "DomainAddressSerialNumber_Write", true);
        Register(FileStream_InfoReport, "FileStream_InfoReport", true);
    }

    public static void Register(int value, string name, bool dataRequired, bool canOptimize = false)
        {
            var len = (value <= 0b1111) ? ApciFieldLength.Bits_4 : ApciFieldLength.Bits_10;
            var high = (byte) (len == ApciFieldLength.Bits_4 ? value >> 2 : value >> 8);
            var low  = (byte) (len == ApciFieldLength.Bits_4 ? value << 6 : value & 0xff);
            _knowApciTypes.Add(value, new ApciTypeDetail2(name, high, low, len, dataRequired, canOptimize));
        }  

        public static bool IsKnown(int id) => _knowApciTypes.ContainsKey(id);
        
        public static ApciTypeDetail2? GetDetails(int id)
        {
            return IsKnown(id) ? _knowApciTypes[id] : null;
        }
        public static string ToString(int id) => IsKnown(id) ? _knowApciTypes[id].Name : "Unknown";

        private static Dictionary<int, ApciTypeDetail2> _knowApciTypes = new Dictionary<int, ApciTypeDetail2>(50);
    }

    public enum ApciFieldLength
    {
        Bits_4,
        Bits_10
    }

    public record struct ApciTypeDetail2(string Name, byte HighBits, byte LowBits, ApciFieldLength Length, bool DataRequired, bool CanOptimize);

}
