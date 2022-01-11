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

#nullable enable

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
    public static class ApciHelper
    {
        public static readonly Dictionary<int,ApciTypeDetail> RelevantAPCITypes = new ()
        {
            { (int)ApciTypes.GroupValue_Read, new (ApciTypes.GroupValue_Read, 0b000000_00, 0b00_000000, 0, ApciFieldLength.Bits_4, false, DataMode.None) },
            { (int)ApciTypes.GroupValue_Response, new (ApciTypes.GroupValue_Response, 0b000000_00, 0b01_000000, 1, ApciFieldLength.Bits_4, true, DataMode.Required) },
            { (int)ApciTypes.GroupValue_Write, new (ApciTypes.GroupValue_Write, 0b000000_00, 0b10_000000, 2, ApciFieldLength.Bits_4, true, DataMode.Required) },
        };

        public static readonly byte DataMask6 = 0b00_111111;  //0x3f

        public static int CalculateLength(ApciTypes apciType, Span<byte> data)
        {
            if (!RelevantAPCITypes.TryGetValue((int)apciType, out var details))
                throw new NotSupportedException($"ApciType {apciType} is not supported");

            return (details.CanOptimize && (data.Length) == 1 && (data[0] <= 63)) ? 2 : data.Length + 2;
        }

        public static ApciTypeDetail GetTypeDetailsFromBytes(Span<byte> data)
        {
            var apci4 = ((data[0] & 0b000000_11) << 2) | (data[1] >> 6);
            var apci10 = ((data[0] & 0b000000_11) << 8) | data[1];

            //first check for 10 bit apci as 0b0111 high bits are present in 4bit (ADC_Read) and 10bit (SystemNetworkParameter*) types 
            if (RelevantAPCITypes.TryGetValue(apci10, out var apcitypedetail) ||
                RelevantAPCITypes.TryGetValue(apci4, out apcitypedetail))
                return apcitypedetail;

            throw new NotSupportedException($"ApciType with 4 bit id {apci4} or 10 bit id {apci10} is not supported");
        }

    }

}
