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
    public enum ApciTypes : int
    {
        None = -2,
        Unknown = -1,
// 3.1 App Layer Services on Multicast
        GroupValue_Read = 0,                          // 0b0000         0  *
        GroupValue_Response = 1,                      // 0b0001 comp    1  *
        GroupValue_Write = 2,                         // 0b0010 comp    2  *
// 3.2 App Layer Services on Broadcast
        IndividualAddress_Write = 3,                  // 0b0011         3  *
        IndividualAddress_Read = 4,                   // 0b0100         4  *
        IndividualAddress_Response = 5,               // 0b0101         5  *
        IndividualAddressSerialNumber_Read = 988,     // 0b1111 011100  15 988
        IndividualAddressSerialNumber_Response = 989, // 0b1111 011101  15 989
        IndividualAddressSerialNumber_Write= 990,     // 0b1111 011110  15 990
        NetworkParameter_Read = 986,                  // 0b1111 011010  15 986
        NetworkParameter_Response = 987,              // 0b1111 011011  15 987
        NetworkParameter_Write = 996,                 // 0b1111 100100  15 996
//        NetworkParameter_InfoReport,                // 0b1111 011011  15 987  Indentical to NetworkParameter_Response
// 3.3 App Layer Services on System Broadcast 
        DeviceDescriptor_InfoReport = 13,             // 0b1101         13
        DomainAddress_Write = 992,                    // 0b1111 100000  15 992
        DomainAddress_Read = 993,                     // 0b1111 100001  15 993
        DomainAddress_Response = 994,                 // 0b1111 100010  15 994
        DomainAddressSelective_Read = 995,            // 0b1111 100011  15 995
        DomainAddressSerialNumber_Read = 1004,        // 0b1111 101100  15 1004
        DomainAddressSerialNumber_Response = 1005,    // 0b1111 101101  15 1005
        DomainAddressSerialNumber_Write = 1006,       // 0b1111 101110  15 1006
        SystemNetworkParameter_Read = 456,            // 0b0111 001000  7  456     first 4 bits conflicting with ADC_Response
        SystemNetworkParameter_Response = 457,        // 0b0111 001001  7  457     first 4 bits conflicting with ADC_Response
        SystemNetworkParameter_Write = 458,           // 0b0111 001010  7  458     first 4 bits conflicting with ADC_Response
// 3.5 App Layer Services on P2P connection oriented
        ADC_Read = 6,                                 // 0b0110         6  *
        ADC_Response = 7,                             // 0b0111         7  *
        Memory_Read = 8,                              // 0b1000         8  *
        Memory_Response = 9,                          // 0b1001         9  *
        Memory_Write = 10,                            // 0b1010         10  *
        MemoryBit_Write = 976,                        // 0b1111 010000  15 976
        UserMemory_Read = 704,                        // 0b1011 000000  11 704
        UserMemory_Response = 705,                    // 0b1011 000001  11 705
        UserMemory_Write = 706,                       // 0b1011 000010  11 706
        UserMemoryBit_Write = 708,                    // 0b1011 000100  11 708
        UserManufacturerInfo_Read = 709,              // 0b1011 000101  11 709
        UserManufacturerInfo_Response = 710,          // 0b1011 000110  11 710
        Authorize_Request = 977,                      // 0b1111 010001  15 977
        Authorize_Response = 978,                     // 0b1111 010010  15 978
        Key_Write = 979,                              // 0b1111 010011  15 979
        Key_Response = 980,                           // 0b1111 010100  15 980
// Management 
        Property_Read = 0x03d5,
        Property_Response = 0x03d6,
        Property_Write = 0x03d7,
        PropertyDescription_Read = 0x03d8, 
        PropertyDescription_Response = 0x03d9
    } 

    public enum ApciFieldLength
    {
        Bits_4,
        Bits_10
    }

    public enum DataMode
    {
        None,
        Required
    }

    public record struct ApciTypeDetail(ApciTypes Type, byte HighBits, byte LowBits, ApciFieldLength Length, bool CanOptimize, DataMode Mode);
    }

