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

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Frames;
using Tiveria.Home.Knx.IP.Frames.Serializers;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.IP.Connections
{

    public class KnxNetIPServerDiscoveryAgent
    {
        #region static members
        public static readonly IPAddress KNXBROADCASTADDRESS = IPAddress.Parse("224.0.23.12");
        public static readonly ushort KNXBROADCASTPORT = 3671;
        private static IPEndPoint KNXBroadcastEndpoint = new IPEndPoint(KNXBROADCASTADDRESS, KNXBROADCASTPORT);
        #endregion

        #region private members
        private readonly List<UdpClient> _udpClients = new List<UdpClient>();
        private readonly List<UdpPacketReceiver> _udpReceivers = new List<UdpPacketReceiver>();
        private readonly List<KnxNetIPServer> _Servers = new List<KnxNetIPServer>();
        #endregion

        #region public properties
        public IReadOnlyList<KnxNetIPServer> Servers => _Servers;
        public event EventHandler<ServerRespondedEventArgs>? ServerResponded;
        #endregion

        public Task<bool> DiscoverAsync(int timeoutms = 1000)
        {
            return InternalDiscoverAsync(ipendpoint => {
                var frame = new SearchRequestFrame(new Hpai(HPAIEndpointType.IPV4_UDP, ipendpoint.Address, (ushort)ipendpoint.Port));
                return KnxNetIPFrameSerializerFactory.Instance.Create(frame.ServiceTypeIdentifier).Serialize(frame);
            }, timeoutms);
        }

        public Task<bool> SearchForInterfaceByMacAsync(byte[] macAddress, int timeoutms = 1000)
        {
            if (macAddress == null || macAddress.Length != 6)
                throw new ArgumentException(nameof(macAddress));

            return InternalDiscoverAsync(ipendpoint =>
            {
                var frame = new ExtendedSearchRequestFrame(new Hpai(HPAIEndpointType.IPV4_UDP, ipendpoint.Address, (ushort)ipendpoint.Port), new SRP(SrpType.SelectByMacAddress, macAddress));
                return KnxNetIPFrameSerializerFactory.Instance.Create(frame.ServiceTypeIdentifier).Serialize(frame);
            }, timeoutms);
        }

        public Task<bool> SearchForInterfacesInProgrammingModeAsync(int timeoutms = 1000)
        {
            return InternalDiscoverAsync(ipendpoint =>
            {
                var frame = new ExtendedSearchRequestFrame(new Hpai(HPAIEndpointType.IPV4_UDP, ipendpoint.Address, (ushort)ipendpoint.Port), new SRP(SrpType.SelectByProgrammingMode, Array.Empty<byte>()));
                return KnxNetIPFrameSerializerFactory.Instance.Create(frame.ServiceTypeIdentifier).Serialize(frame);
            }, timeoutms);
        }

        #region private methods
        private async Task<bool> InternalDiscoverAsync(Func<IPEndPoint, byte[]> frameDataGenerator, int timeoutms)
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

        private void BroadcastSearchRequest(Func<IPEndPoint, byte[]> generator)
        {
            _Servers.Clear();
            foreach (var client in _udpClients)
            {
                SendSearchMessageAsync(client, generator);
            }
        }

        private async Task SendSearchMessageAsync(UdpClient client, Func<IPEndPoint, byte[]> generator)
        {
            var endpoint = client.Client.LocalEndPoint;
            if (endpoint == null) return;

            var ipendpoint = (IPEndPoint)endpoint;
            var data = generator(ipendpoint);

            await client.SendAsync(data, data.Length, KNXBroadcastEndpoint);
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
            var _udpClient = new UdpClient(new IPEndPoint(addr, 0)); // Port 0 should trigger the system to select a free one
            _udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, IPAddress.HostToNetworkOrder(nicIndex));
            _udpClients.Add(_udpClient);
        }
        #endregion

        #region Reacting on search responses
        private void PacketReceivedDelegate(DateTime timestamp, IPEndPoint source, IPEndPoint receiver, byte[] data)
        {
            var parser = new SearchResponseFrameSerializer();
            if (parser.TryDeserialize(data, out var frame))
            {
                var sf = ((SearchResponseFrame)frame!);
                var intf = new KnxNetIPServer(new IPEndPoint(sf.ServiceEndpoint.Ip, sf.ServiceEndpoint.Port), sf.DeviceInfoDIB, sf.ServiceFamiliesDIB);
                _Servers.Add(intf);
                OnServerResponded(receiver, intf);
            }
        }

        protected void OnServerResponded(IPEndPoint receiver, KnxNetIPServer server)
        {
            ServerResponded?.Invoke(this, new ServerRespondedEventArgs(receiver, server));
        }
        #endregion
        #endregion
    }
}
