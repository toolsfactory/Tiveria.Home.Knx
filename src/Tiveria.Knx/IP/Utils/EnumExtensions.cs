using System;

namespace Tiveria.Knx.IP.Utils
{
    public static class EnumExtensions 
    {
        public static String  ToString(this HPAIEndpointType hpaiEndpointType)
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

        public static String ToString(this ConnectionType connectionType)
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

        public static String ToString(this ServiceTypeIdentifier serviceTypeIdentifier)
        {
            switch (serviceTypeIdentifier)
            {
                case ServiceTypeIdentifier.CONNECT_REQUEST:
                    return "connect.req";
                case ServiceTypeIdentifier.CONNECT_RESPONSE:
                    return "connect.res";
                case ServiceTypeIdentifier.CONNECTIONSTATE_REQUEST:
                    return "connectionstate.req";
                case ServiceTypeIdentifier.CONNECTIONSTATE_RESPONSE:
                    return "connectionstate.res";
                case ServiceTypeIdentifier.DISCONNECT_REQ:
                    return "disconnect.req";
                case ServiceTypeIdentifier.DISCONNECT_RES:
                    return "disconnect.res";
                case ServiceTypeIdentifier.DESCRIPTION_REQ:
                    return "description.req";
                case ServiceTypeIdentifier.DESCRIPTION_RES:
                    return "description.res";
                case ServiceTypeIdentifier.SEARCH_REQ:
                    return "search.req";
                case ServiceTypeIdentifier.SEARCH_RES:
                    return "search.res";
                case ServiceTypeIdentifier.DEVICE_CONFIGURATION_REQ:
                    return "device-configuration.req";
                case ServiceTypeIdentifier.DEVICE_CONFIGURATION_ACK:
                    return "device-configuration.ack";
                case ServiceTypeIdentifier.TUNNELING_REQ:
                    return "tunneling.req";
                case ServiceTypeIdentifier.TUNNELING_ACK:
                    return "tunneling.ack";
                case ServiceTypeIdentifier.ROUTING_IND:
                    return "routing.ind";
                case ServiceTypeIdentifier.ROUTING_LOST_MSG:
                    return "routing-lost.msg";
                case ServiceTypeIdentifier.ROUTING_BUSY:
                    return "routing-busy.ind";
                default:
                    return "unknown service";
            }
        }
    }
}