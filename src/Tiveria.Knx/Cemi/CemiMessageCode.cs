/*
    Tiveria.Knx - a .Net Core base KNX library
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


namespace Tiveria.Knx.Cemi
{
    public enum CemiMessageCode : byte
    {
        // Message code for L-Data request
        LDATA_REQ = 0x11,
        // Message code for L-Data indication
        LDATA_IND = 0x29,
        // Message code for busmonitor indication
        BUSMON_IND = 0x2B,
        // Message code for L-Data confirmation
        LDATA_CON = 0x2E,
        // Message code for property reset indication
        RESET_IND = 0xF0,
        // Message code for property reset request
        RESET_REQ = 0xF1,
        // Message code for property write confirmation
        PROPWRITE_CON = 0xF5,
        // Message code for property write request
        PROPWRITE_REQ = 0xF6,
        // Message code for property info indication
        PROPINFO_IND = 0xF7,
        // Message code for property read confirmation
        PROPREAD_CON = 0xFB,
        //Message code for property read request
        PROPREAD_REQ = 0xFC
    }
}
