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
    /// Extension methods used to translate the KNXet/IP specific enums to readable strings
    /// </summary>
    public static class EnumExtensions 
    {
        /// <summary>
        /// translate <see cref="HPAIEndpointType"/> to a readable string
        /// </summary>
        /// <returns>the string representation of the enum value</returns>
        public static String ToDescription(this HPAIEndpointType hpaiEndpointType)
        {
            switch(hpaiEndpointType)
            {
                case HPAIEndpointType.IPV4_TCP:
                    return "IPv4 / TCP";
                case HPAIEndpointType.IPV4_UDP:
                    return "IPv4 / UDP";
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// translate <see cref="TunnelingLayer"/> to a readable string
        /// </summary>
        /// <returns>the string representation of the enum value</returns>
        public static String ToDescription(this TunnelingLayer tunnelingLayer)
        {
            switch (tunnelingLayer)
            {
                case TunnelingLayer.TUNNEL_BUSMONITOR:
                    return "Busmonitor";
                case TunnelingLayer.TUNNEL_LINKLAYER:
                    return "Linklayer";
                case TunnelingLayer.TUNNEL_RAW:
                    return "RAW";
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// translate <see cref="ConnectionType"/> to a readable string
        /// </summary>
        /// <returns>the string representation of the enum value</returns>
        public static String ToDescription(this ConnectionType connectionType)
        {
            switch (connectionType)
            {
                case ConnectionType.DeviceManagement:
                    return "Device Management";
                case ConnectionType.RemConf:
                    return "Remote Configuration";
                case ConnectionType.RemLog:
                    return "Remote Logging";
                case ConnectionType.Tunnel:
                    return "Tunneling";
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// translate <see cref="ServiceTypeIdentifier"/> to a readable string
        /// </summary>
        /// <returns>the string representation of the enum value</returns>
        public static String ToDescription(this ServiceTypeIdentifier serviceTypeIdentifier)
        {
            switch (serviceTypeIdentifier)
            {
                case ServiceTypeIdentifier.ConnectRequest:
                    return "Connect.req";
                case ServiceTypeIdentifier.ConnectResponse:
                    return "Connect.res";
                case ServiceTypeIdentifier.ConnectionStateRequest:
                    return "Connectionstate.req";
                case ServiceTypeIdentifier.ConnectionStateResponse:
                    return "Connectionstate.res";
                case ServiceTypeIdentifier.DisconnectRequest:
                    return "Disconnect.req";
                case ServiceTypeIdentifier.DisconnectResponse:
                    return "Disconnect.res";
                case ServiceTypeIdentifier.DescriptionRequest:
                    return "Description.req";
                case ServiceTypeIdentifier.DescriptionResponse:
                    return "Description.res";
                case ServiceTypeIdentifier.SearchRequest:
                    return "Search.req";
                case ServiceTypeIdentifier.SearchResponse:
                    return "Search.res";
                case ServiceTypeIdentifier.DeviceConfigurationRequest:
                    return "DeviceConfiguration.req";
                case ServiceTypeIdentifier.DeviceConfigurationResponse:
                    return "DeviceConfiguration.ack";
                case ServiceTypeIdentifier.TunnelingRequest:
                    return "Tunneling.req";
                case ServiceTypeIdentifier.TunnelingAcknowledge:
                    return "Tunneling.ack";
                case ServiceTypeIdentifier.RoutingIndication:
                    return "Routing.ind";
                case ServiceTypeIdentifier.RoutingLostMessage:
                    return "RoutingLost.msg";
                case ServiceTypeIdentifier.ROURoutingBusyING_BUSY:
                    return "RoutingBusy.ind";
                default:
                    return "Unknown";
            }
        }

        public static string ToDescription(this ErrorCodes ec)
        {
            switch (ec)
            {
                case ErrorCodes.NoError: return "Success";
                // common error codes
                case ErrorCodes.HostProtocolType:
                    return "Host protocol not supported";
                case ErrorCodes.VersionNotSupported:
                    return "Protocol version not supported";
                case ErrorCodes.SequenceNumber:
                    return "Sequence number out of order";
                // connect response error codes
                case ErrorCodes.ConnectionType:
                    return "Connection type not supported by server";
                case ErrorCodes.ConnectionOption:
                    return "One or more connection options not supported by server";
                case ErrorCodes.NoMoreConnections:
                    return "Server cannot accept new connections. Concurrency maximum reached.";
                case ErrorCodes.TunnelingLayer:
                    return "Requested tunneling layer not supported by server";
                // connection state response error codes
                case ErrorCodes.ConnectionId:
                    return "No active connection with specified ID found by server";
                case ErrorCodes.DataConnection:
                    return "Error in data connection";
                case ErrorCodes.KnxConnection:
                    return "Error in KNX connection";
                default:
                    return "Unknown";
            }
        }
    }
}