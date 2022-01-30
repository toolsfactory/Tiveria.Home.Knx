/*
    Tiveria.Home.Knx - a .Net Core base KNX library
    Copyright (c) 2018-2022 M. Geissler

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License; or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful;
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not; write to the Free Software
    Foundation; Inc.; 59 Temple Place; Suite 330; Boston; MA  02111-1307  USA

    Linking this library statically or dynamically with other modules is
    making a combined work based on this library. Thus; the terms and
    conditions of the GNU General Public License cover the whole
    combination.
*/
    
namespace Tiveria.Home.Knx.IP.Enums
{
    /// <summary>
    /// Enum with all KNXnet/IP Servicetype Identifiers and their correct hex codes
    /// <seealso cref="EnumExtensions"/>
    /// </summary>
    public static class ServiceTypeIdentifier
    {
        #region public ServiceTypeIdentifier constants
        public const ushort SearchRequest = 0x0201;
        public const ushort SearchResponse = 0x0202;
        public const ushort DescriptionRequest = 0x0203;
        public const ushort DescriptionResponse = 0x0204;
        public const ushort ConnectRequest = 0x0205;
        public const ushort ConnectResponse = 0x0206;
        public const ushort ConnectionStateRequest = 0x0207;
        public const ushort ConnectionStateResponse = 0x0208;
        public const ushort DisconnectRequest = 0x0209;
        public const ushort DisconnectResponse = 0x020A;
        public const ushort ExtendedSearchRequest = 0x020B;
        public const ushort DeviceConfigurationRequest = 0x0310;
        public const ushort DeviceConfigurationResponse = 0x0311;
        public const ushort TunnelingRequest = 0x0420;
        public const ushort TunnelingAcknowledge = 0x0421;
        public const ushort RoutingIndication = 0x0530;
        public const ushort RoutingLostMessage = 0x0531;
        public const ushort RoutingBusy = 0x0532;
        public const ushort ObjectServer = 0xF080;
        #endregion

        #region public helpers
        public static void Register(ushort id, string name)
        { 
            _knownServiceTypeIdentifiers.Add(id, name);
        }

        public static bool IsKnown(ushort id)
        {
            return _knownServiceTypeIdentifiers.ContainsKey(id);
        }

        /// <summary>
        /// translate <see cref="ServiceTypeIdentifier"/> to a readable string
        /// </summary>
        /// <returns>the string representation of the ServiceTypeIdentifer</returns>
        public static string ToDescription(ushort id)
        {
            return IsKnown(id) ? _knownServiceTypeIdentifiers[id] : "Unknown";
        }
        #endregion

        #region internal implementation
        static ServiceTypeIdentifier()
        {
            Register(SearchRequest, "SearchRequest");
            Register(SearchResponse, "SearchResponse");
            Register(DescriptionRequest, "DescriptionRequest");
            Register(DescriptionResponse, "DescriptionResponse");
            Register(ConnectRequest, "ConnectRequest");
            Register(ConnectResponse, "ConnectResponse");
            Register(ConnectionStateRequest, "ConnectionStateRequest");
            Register(ConnectionStateResponse, "ConnectionStateResponse");
            Register(DisconnectRequest, "DisconnectRequest");
            Register(DisconnectResponse, "DisconnectResponse");
            Register(ExtendedSearchRequest, "ExtendedSearchRequest");
            Register(DeviceConfigurationRequest, "DeviceConfigurationRequest");
            Register(DeviceConfigurationResponse, "DeviceConfigurationResponse");
            Register(TunnelingRequest, "TunnelingRequest");
            Register(TunnelingAcknowledge, "TunnelingAcknowledge");
            Register(RoutingIndication, "RoutingIndication");
            Register(RoutingLostMessage, "RoutingLostMessage");
            Register(RoutingBusy, "RoutingBusy");
            Register(ObjectServer, "ObjectServer");
        }

        private static Dictionary<ushort, string> _knownServiceTypeIdentifiers = new Dictionary<ushort, string>(25);
        #endregion

    }
}