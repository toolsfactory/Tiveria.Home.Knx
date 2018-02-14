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

using System;

namespace Tiveria.Knx.IP.Utils
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
                case ConnectionType.DEVICE_MGMT_CONNECTION:
                    return "Device Management";
                case ConnectionType.OBJSVR_CONNECTION:
                    return "Objectserver";
                case ConnectionType.REMCONF_CONNECTION:
                    return "Remote Configuration";
                case ConnectionType.REMLOG_CONNECTION:
                    return "Remote Logging";
                case ConnectionType.TUNNEL_CONNECTION:
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
                case ServiceTypeIdentifier.CONNECT_REQUEST:
                    return "Connect.req";
                case ServiceTypeIdentifier.CONNECT_RESPONSE:
                    return "Connect.res";
                case ServiceTypeIdentifier.CONNECTIONSTATE_REQUEST:
                    return "Connectionstate.req";
                case ServiceTypeIdentifier.CONNECTIONSTATE_RESPONSE:
                    return "Connectionstate.res";
                case ServiceTypeIdentifier.DISCONNECT_REQ:
                    return "Disconnect.req";
                case ServiceTypeIdentifier.DISCONNECT_RES:
                    return "Disconnect.res";
                case ServiceTypeIdentifier.DESCRIPTION_REQ:
                    return "Description.req";
                case ServiceTypeIdentifier.DESCRIPTION_RES:
                    return "Description.res";
                case ServiceTypeIdentifier.SEARCH_REQ:
                    return "Search.req";
                case ServiceTypeIdentifier.SEARCH_RES:
                    return "Search.res";
                case ServiceTypeIdentifier.DEVICE_CONFIGURATION_REQ:
                    return "DeviceConfiguration.req";
                case ServiceTypeIdentifier.DEVICE_CONFIGURATION_ACK:
                    return "DeviceConfiguration.ack";
                case ServiceTypeIdentifier.TUNNELING_REQ:
                    return "Tunneling.req";
                case ServiceTypeIdentifier.TUNNELING_ACK:
                    return "Tunneling.ack";
                case ServiceTypeIdentifier.ROUTING_IND:
                    return "Routing.ind";
                case ServiceTypeIdentifier.ROUTING_LOST_MSG:
                    return "RoutingLost.msg";
                case ServiceTypeIdentifier.ROUTING_BUSY:
                    return "RoutingBusy.ind";
                default:
                    return "Unknown";
            }
        }

        public static string ToDescription(this ErrorCodes ec)
        {
            switch (ec)
            {
                case ErrorCodes.NO_ERROR: return "Success";
                // common error codes
                case ErrorCodes.HOST_PROTOCOL_TYPE:
                    return "Host protocol not supported";
                case ErrorCodes.VERSION_NOT_SUPPORTED:
                    return "Protocol version not supported";
                case ErrorCodes.SEQUENCE_NUMBER:
                    return "Sequence number out of order";
                // connect response error codes
                case ErrorCodes.CONNECTION_TYPE:
                    return "Connection type not supported by server";
                case ErrorCodes.CONNECTION_OPTION:
                    return "One or more connection options not supported by server";
                case ErrorCodes.NO_MORE_CONNECTIONS:
                    return "Server cannot accept new connections. Concurrency maximum reached.";
                case ErrorCodes.TUNNELING_LAYER:
                    return "Requested tunneling layer not supported by server";
                // connection state response error codes
                case ErrorCodes.CONNECTION_ID:
                    return "No active connection with specified ID found by server";
                case ErrorCodes.DATA_CONNECTION:
                    return "Error in data connection";
                case ErrorCodes.KNX_CONNECTION:
                    return "Error in KNX connection";
                default:
                    return "Unknown";
            }
        }
    }
}