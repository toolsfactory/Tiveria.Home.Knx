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


namespace Tiveria.Home.Knx.EMI
{
    /// <summary>
    /// Enumeration of all all specified types of <see cref="AdditionalInformationField"/> variants
    /// </summary>
    public enum AdditionalInfoType : byte
    {
        // 0 bytes - Reserved
        RESERVED0 = 0x00,
        // 2 bytes - Domain Address used by PL medium
        PLMEDIUM = 0x01,
        // 8 bytes - Combination of RF Info byte, KNX Serial Number/DoA and Data Link Layer Frame Number (LFN)
        RFMEDIUM = 0x02,
        // 1 bytes - Busmonitor Error Flag
        BUSMONITOR = 0x03,
        // 2 bytes - relative timestamp
        TIMESTAMP = 0x04,
        // 4 bytes - time delay until sending
        TIMEDELAY = 0x05,
        // 4 bytes - device independent timestamp used for example with L_Raw.ind or L_Busmon.ind
        TIMESTAMP_EXT = 0x06,
        // 2 bytes - Bits 7 to 4 of RF Ctrl field and block number
        BIBAT = 0x07,
        // 4 bytes - Multi frequency, call channel and fast ack id for RF
        RFMULTI = 0x08,
        // 3 bytes - length of pre- and postamble
        PREPOSTAMBLE = 0x09,
        // N*2 bytes - Status/Info for each FastAck (N)
        RFFASTACK = 0x0A,
        // N+3 bytes - Manufacturer specific data (N bytes). ManID (2 bytes). Subfunction (1 byte)
        MANUFACTURER = 0xFE,
        // 0 bytes - Reserved
        RESERVED255 = 0xFF
    }
}