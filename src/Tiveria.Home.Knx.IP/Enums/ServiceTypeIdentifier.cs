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
    
namespace Tiveria.Home.Knx.IP.Enums
{
    /// <summary>
    /// Enum with all KNXnet/IP Servicetype Identifiers and their correct hex codes
    /// <seealso cref="EnumExtensions"/>
    /// </summary>
    public enum ServiceTypeIdentifier : ushort
    {
        SearchRequest = 0x0201,
        SearchResponse = 0x0202,
        DescriptionRequest = 0x0203,
        DescriptionResponse = 0x0204,
        ConnectRequest = 0x0205,
        ConnectResponse = 0x0206,
        ConnectionStateRequest = 0x0207,
        ConnectionStateResponse = 0x0208,
        DisconnectRequest = 0x0209,
        DisconnectResponse = 0x020A,
        ExtendedSearchRequest = 0x020B,
        DeviceConfigurationRequest = 0x0310,
        DeviceConfigurationResponse = 0x0311,
        TunnelingRequest = 0x0420,
        TunnelingAcknowledge = 0x0421,
        RoutingIndication = 0x0530,
        RoutingLostMessage = 0x0531,
        ROURoutingBusyING_BUSY = 0x0532,
        ObjectServer = 0xF080,
        Unknown = 0xFFFF
    }
}