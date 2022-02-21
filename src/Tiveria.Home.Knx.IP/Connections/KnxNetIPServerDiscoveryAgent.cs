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

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Services;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.IP.Connections
{
    /// <summary>
    /// Provides means to search for KnxNetIP interfaces and routers in the networks the host is connected with.
    /// (Even though not fully correct, KnxNetIP iterfaces and routers are called servers in here)
    /// </summary>
    public class KnxNetIPServerDiscoveryAgent
    {
        #region public properties
        /// <summary>
        /// List of Knx interfaces and routers that responded on which IP interface
        /// </summary>
        public IReadOnlyList<KeyValuePair<IPAddress, KnxNetIPServerDescription>> Servers => _Servers;

        /// <summary>
        /// Occurs when a SearchResponse Frame is received
        /// </summary>
        public event EventHandler<ServerRespondedEventArgs>? ServerResponded;
        #endregion

        #region public methods
        /// <summary>
        /// Trigger a search for all Knx interfaces and routers using a standard SearchRequest Frame with ServiceTypeIdentifier 0x0201
        /// </summary>
        /// <param name="timeoutms">Time after the async search is canceled</param>
        /// <returns>true in case at least one KNX interface or router was found. Otherwise false </returns>
        public Task<bool> DiscoverAsync(int timeoutms = 1000)
        {
            return InternalDiscoverAsync(ipendpoint => {
                return new SearchRequestService(new Hpai(HPAIEndpointType.IPV4_UDP, ipendpoint.Address, (ushort)ipendpoint.Port));
            }, timeoutms);
        }

        /// <summary>
        /// Trigger a search for a Knx interface or router that has the specified MAC address. The extended SearchRequest Frame with ServiceTypeIdentifier 0x020B is used
        /// </summary>
        /// <param name="macAddress">The MAC address folter</param>
        /// <param name="timeoutms">Time after the async search is canceled</param>
        /// <returns>true in case the KNX interface or router responded. Otherwise false </returns>
        public Task<bool> DiscoverByMacAsync(byte[] macAddress, int timeoutms = 1000)
        {
            return InternalDiscoverAsync(ipendpoint =>
            {
                return new ExtendedSearchRequestService(new Hpai(HPAIEndpointType.IPV4_UDP, ipendpoint.Address, (ushort)ipendpoint.Port), new SRP(SrpType.SelectByMacAddress, macAddress));
            }, timeoutms);
        }

        /// <summary>
        /// Trigger a search for all Knx interfaces and router that are currently in programming mode. The extended SearchRequest Frame with ServiceTypeIdentifier 0x020B is used
        /// </summary>
        /// <param name="timeoutms">Time after the async search is canceled</param>
        /// <returns>true in case at least one KNX interface or router was found. Otherwise false </returns>
        public Task<bool> DiscoverInProgrammingModeAsync(int timeoutms = 1000)
        {
            return InternalDiscoverAsync(ipendpoint =>
            {
                return new ExtendedSearchRequestService(new Hpai(HPAIEndpointType.IPV4_UDP, ipendpoint.Address, (ushort)ipendpoint.Port), new SRP(SrpType.SelectByProgrammingMode, Array.Empty<byte>()));
            }, timeoutms);
        }

        /// <summary>
        /// Trigger a search for all Knx interfaces and router that are currently in programming mode. The extended SearchRequest Frame with ServiceTypeIdentifier 0x020B is used
        /// </summary>
        /// <param name="service">Service description</param>
        /// <param name="timeoutms">Time after the async search is canceled</param>
        /// <returns>true in case at least one KNX interface or router was found. Otherwise false </returns>
        public Task<bool> DiscoverByServiceAsync(byte[] service, int timeoutms = 1000)
        {
            return InternalDiscoverAsync(ipendpoint =>
            {
                return new ExtendedSearchRequestService(new Hpai(HPAIEndpointType.IPV4_UDP, ipendpoint.Address, (ushort)ipendpoint.Port), new SRP(SrpType.SelectByService, service));
            }, timeoutms);
        }

        /// <summary>
        /// Trigger a search for all Knx interfaces and router that support the requested description (DIB). The extended SearchRequest Frame with ServiceTypeIdentifier 0x020B is used
        /// </summary>
        /// <param name="dibFilter">The DIB describing the filter</param>
        /// <param name="timeoutms">Time after the async search is canceled</param>
        /// <returns>true in case at least one KNX interface or router was found. Otherwise false </returns>
        public Task<bool> DiscoverByDIBFilterAsync(byte[] dibFilter, int timeoutms = 1000)
        {
            return InternalDiscoverAsync(ipendpoint =>
            {
                return new ExtendedSearchRequestService(new Hpai(HPAIEndpointType.IPV4_UDP, ipendpoint.Address, (ushort)ipendpoint.Port), new SRP(SrpType.RequestDibs, dibFilter));
            }, timeoutms);
        }
        #endregion

        #region private members
        private readonly List<IUdpClient> _udpClients = new();
        private readonly List<UdpPacketReceiver> _udpReceivers = new();
        private readonly List<KeyValuePair<IPAddress, KnxNetIPServerDescription>> _Servers = new();
        #endregion

        #region private implementations
        private async Task<bool> InternalDiscoverAsync(Func<IPEndPoint, IKnxNetIPService> frameDataGenerator, int timeoutms)
        {
            Initialize();
            foreach (var receiver in _udpReceivers)
            {
                receiver.Start();
            }

            BroadcastSearchRequest(frameDataGenerator);
            await Task.Delay(timeoutms, CancellationToken.None);

            foreach (var receiver in _udpReceivers)
            {
                receiver.Stop();
            }
            return Servers.Count > 0;
        }

        #region Sending the search requests
        private void Initialize()
        {
            _Servers.Clear();
            _udpClients.Clear();
            _udpReceivers.Clear();
            GetLocalNetworkInterfaces();
            foreach (var client in _udpClients)
            {
                var receiver = new UdpPacketReceiver(client, PacketReceivedDelegate, null);
                _udpReceivers.Add(receiver);
            }
        }

        private void BroadcastSearchRequest(Func<IPEndPoint, IKnxNetIPService> generator)
        {
            _Servers.Clear();
            foreach (var client in _udpClients)
            {
                _ = SendSearchMessageAsync(client, generator);
            }
        }

        private async Task SendSearchMessageAsync(IUdpClient client, Func<IPEndPoint, IKnxNetIPService> generator)
        {
            var endpoint = client.LocalEndPoint;
            if (endpoint == null) return;

            var ipendpoint = (IPEndPoint)endpoint;
            var service = generator(ipendpoint);
            var frame = new KnxNetIPFrame(service);
            var data = frame.ToBytes();

            await client.SendAsync(data, KnxNetIPConstants.DefaultBroadcastEndpoint);
        }
        #endregion

        #region Finding available interfaces
        private void GetLocalNetworkInterfaces()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface nic in networkInterfaces)
            {
                var props = nic.GetIPProperties();
                // try to skip all offline interfaces and the ones that dont have Multicast support -> indicator for VPN
                if (props.MulticastAddresses.Count == 0
                    || !nic.SupportsMulticast
                    || OperationalStatus.Up != nic.OperationalStatus)
                    continue;

                // and skip the ones who dont have IPv4 at all
                if (nic.Supports(NetworkInterfaceComponent.IPv4) == false)
                    continue;
                // now get ipv4 details
                IPv4InterfaceProperties p = props.GetIPv4Properties();

                var addr = nic.GetIPProperties().UnicastAddresses.Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork).First().Address;
                CreateUdpClientForIP(addr, p.Index);
            }
        }

        private void CreateUdpClientForIP(IPAddress addr, int nicIndex)
        {
            var _udpClient = UdpClientFactory.GetInstanceWithEP(new IPEndPoint(addr, 0)); // Port 0 should trigger the system to select a free one
            _udpClient.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, IPAddress.HostToNetworkOrder(nicIndex));
            _udpClients.Add(_udpClient);
        }
        #endregion

        #region Reacting on search responses
        private void PacketReceivedDelegate(DateTime timestamp, IPEndPoint source, IPEndPoint receiver, byte[] data)
        {
            if (!KnxNetIPFrame.TryParse(data, out var frame))
                return;
            if (frame!.FrameHeader.ServiceTypeIdentifier != ServiceTypeIdentifier.SearchResponse)
                return;

            var sf = ((SearchResponseService)frame!.Service);
            var intf = new KnxNetIPServerDescription(new IPEndPoint(sf.ServiceEndpoint.Ip, sf.ServiceEndpoint.Port), sf.DeviceInfoDIB, sf.ServiceFamiliesDIB);
            _Servers.Add(new KeyValuePair<IPAddress, KnxNetIPServerDescription>(receiver.Address, intf));
            OnServerResponded(receiver, intf);
        }

        protected void OnServerResponded(IPEndPoint receiver, KnxNetIPServerDescription server)
        {
            ServerResponded?.Invoke(this, new ServerRespondedEventArgs(receiver, server));
        }
        #endregion
        #endregion
    }
}
