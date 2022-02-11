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

namespace Tiveria.Home.Knx.IP.Enums
{
    /// <summary>
    /// Extension methods used to translate the KNXet/IP specific enums to readable strings
    /// </summary>
    public static class EnumExtensions 
    {
        /// <summary>
        /// translate <see cref="HPAIEndpointType"/> to a readable string
        /// </summary>
        /// <returns>the string representation of the enum value</returns>
        public static String ToDescription(this HPAIEndpointType hpaiEndpointType)
            => hpaiEndpointType switch
            {
                HPAIEndpointType.IPV4_TCP => "IPv4 / TCP",
                HPAIEndpointType.IPV4_UDP => "IPv4 / UDP",
                _                         => "Unknown"
            };

        /// <summary>
        /// translate <see cref="TunnelingLayer"/> to a readable string
        /// </summary>
        /// <returns>the string representation of the enum value</returns>
        public static String ToDescription(this TunnelingLayer tunnelingLayer) 
            => tunnelingLayer switch
            {
                TunnelingLayer.TUNNEL_BUSMONITOR => "Busmonitor",
                TunnelingLayer.TUNNEL_LINKLAYER  => "Linklayer",
                TunnelingLayer.TUNNEL_RAW        => "RAW",
                _                                => "Unknown"
            };

        /// <summary>
        /// translate <see cref="ConnectionType"/> to a readable string
        /// </summary>
        /// <returns>the string representation of the enum value</returns>
        public static String ToDescription(this ConnectionType connectionType)
            => connectionType switch
            {
                ConnectionType.DeviceManagement => "Device Management",
                ConnectionType.RemConf          => "Remote Configuration",
                ConnectionType.RemLog           => "Remote Logging",
                ConnectionType.Tunnel           => "Tunneling",
                _                               => "Unknown"
            };

        public static string ToDescription(this ErrorCodes ec)
            => ec switch
            {
                ErrorCodes.NoError             => "Success",
                ErrorCodes.HostProtocolType    => "Host protocol not supported",
                ErrorCodes.VersionNotSupported => "Protocol version not supported",
                ErrorCodes.SequenceNumber      => "Sequence number out of order",
                ErrorCodes.ConnectionType      => "Connection type not supported by server",
                ErrorCodes.ConnectionOption    => "One or more connection options not supported by server",
                ErrorCodes.NoMoreConnections   => "Server cannot accept new connections. Concurrency maximum reached.",
                ErrorCodes.TunnelingLayer      => "Requested tunneling layer not supported by server",
                ErrorCodes.ConnectionId        => "No active connection with specified ID found by server",
                ErrorCodes.DataConnection      => "Error in data connection",
                ErrorCodes.KnxConnection       => "Error in KNX connection",
                _                              => "Unknown"
            };
    }
}