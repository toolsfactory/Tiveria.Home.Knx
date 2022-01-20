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
using System.Net.Sockets;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Frames;

namespace Tiveria.Home.Knx.IP.Connections
{
    /// <summary>
    /// Class for connecting via KnxNetIP Routing to the Knx infrastructure
    /// </summary>
    public class RoutingConnection : IPConnectionBase
    {
        #region public constructors
        /// <summary>
        /// Create a routing connection client that connects to the default multicast endpoint <see cref="KnxNetIPConstants.DefaultBroadcastEndpoint"/>
        /// </summary>
        /// <param name="localEndPoint">The local endpoint from which to connect.</param>
        public RoutingConnection(IPEndPoint localEndPoint)
            :this (localEndPoint, KnxNetIPConstants.DefaultBroadcastEndpoint)
        { }

        /// <summary>
        /// Create a routing connection client that connects to the multicast endpoint provided as parameter
        /// </summary>
        /// <param name="localEndPoint">The local endpoint from which to connect.</param>
        /// <param name="remoteEndpoint">The multicast group address to join</param>
        public RoutingConnection(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint) : base(remoteEndpoint)
        {
            _localEndPoint = localEndpoint;
            RemoteEndpoint = remoteEndpoint;
            _udpClient = new UdpClient() { 
                MulticastLoopback = true,
                ExclusiveAddressUse = false, 
                DontFragment = true
            };

            _packetReceiver = new UdpPacketReceiver(_udpClient, PacketReceivedDelegate, KnxFrameReceivedDelegate);
        }
        #endregion

        #region public methods
        /// <inheritdoc/>
        public override Task<bool> ConnectAsync()
        {
            return Task.Run(() =>
            {
                ConnectionState = ConnectionState.Opening;
                //_logger.Trace("ConnectAsync: Sending connection request to " + _remoteControlEndpoint + " with data " + data.ToHex());
                try
                {
                    _udpClient.Client.Bind(_localEndPoint);
                    _udpClient.JoinMulticastGroup(RemoteEndpoint.Address, _localEndPoint.Address);
                    _packetReceiver.Start();
                    ConnectionState = ConnectionState.Open;
                    return true;
                }
                catch (SocketException se)
                {
                    //_logger.Error("ConnectAsync: SocketException raised", se);
                    ConnectionState = ConnectionState.Invalid;
                    return false;
                }
            });
        }

        /// <inheritdoc/>
        public override Task DisconnectAsync()
        {
            _udpClient.Close();
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async override Task<bool> SendAsync(IKnxNetIPFrame frame)
        {
            if (ConnectionState != ConnectionState.Open)
                return false;

            var serializer = KnxNetIPFrameSerializerFactory.Instance.Create(frame.ServiceTypeIdentifier);
            var data = serializer.Serialize(frame);
            try
            {
                var bytessent = await _udpClient.SendAsync(data, data.Length, RemoteEndpoint);
                if (bytessent == 0)
                {
                    //                    _logger.Error("SendFrameAsync: Zero bytes sent");
                    return false;
                }
                else
                    return true;
            }
            catch (SocketException se)
            {
                //_logger.Error("SendFrameAsync: SocketException raised", se);
                ConnectionState = ConnectionState.Invalid;
                return false;
            }
        }

        /// <inheritdoc/>
        public async override Task<bool> SendCemiAsync(ICemiMessage message)
        {
            var frame = new RoutingIndicationFrame(message);
            return await SendAsync(frame);
        }
        #endregion

        #region private members
        private readonly UdpClient _udpClient;
        private UdpPacketReceiver _packetReceiver;
        private bool disposedValue = false;
        private readonly IPEndPoint _localEndPoint;
        #endregion

        #region private implementations
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
                }
                _udpClient.Dispose();
                disposedValue = true;
            }
        }

        protected override string GetConnectionName()
        {
            return "RoutingConnection";
        }

        #region packet and knx frame receive delegates
        private void PacketReceivedDelegate(DateTime timestamp, IPEndPoint source, IPEndPoint receiver, byte[] data)
        {
            OnDataReceived(timestamp, data);
        }

        private void KnxFrameReceivedDelegate(DateTime timestamp, IPEndPoint source, IPEndPoint receiver, IKnxNetIPFrame frame)
        {
            /*
            switch (frame.FrameHeader.ServiceTypeIdentifier)
            {
                case ServiceTypeIdentifier.RoutingIndication:
                    HandleConnectionStateResponse((ConnectionStateResponseFrame)frame);
                    break;
                case ServiceTypeIdentifier.UNKNOWN:
                    HandleUnknownServiceType((UnknownService)frame);
                    break;
                }
            */
            OnFrameReceived(timestamp, frame, true);
        }
        #endregion

        #endregion
    }
}