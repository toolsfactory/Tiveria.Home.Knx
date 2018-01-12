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
    
namespace Tiveria.Knx.IP.Utils
{
    /// <summary>
    /// Enum with all KNXnet/IP Servicetype Identifiers and their correct hex codes
    /// <seealso cref="EnumExtensions"/>
    /// </summary>
    public enum ServiceTypeIdentifier : ushort
    {
        CONNECT_REQUEST = 0x0205,
        CONNECT_RESPONSE = 0x0206,
        CONNECTIONSTATE_REQUEST = 0x0207,
        CONNECTIONSTATE_RESPONSE = 0x0208,
        DISCONNECT_REQ = 0x0209,
        DISCONNECT_RES = 0x020A,
        DESCRIPTION_REQ = 0x0203,
        DESCRIPTION_RES = 0x204,
        SEARCH_REQ = 0x201,
        SEARCH_RES = 0x202,
        DEVICE_CONFIGURATION_REQ = 0x0310,
        DEVICE_CONFIGURATION_ACK = 0x0311,
        TUNNELING_REQ = 0x0420,
        TUNNELING_ACK = 0x0421,
        ROUTING_IND = 0x0530,
        ROUTING_LOST_MSG = 0x0531,
        ROUTING_BUSY = 0x0532
    }
}