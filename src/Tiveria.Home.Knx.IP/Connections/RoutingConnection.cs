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
using System.Net.Sockets;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Services;

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
        /// <param name="localEndpoint">The local endpoint from which to connect.</param>
        /// <param name="remoteEndpoint">The multicast group address to join</param>
        public RoutingConnection(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint) : base(remoteEndpoint)
        {
            _localEndPoint = localEndpoint;
            RemoteEndpoint = remoteEndpoint;
            _udpClient = UdpClientFactory.GetInstance();
            _udpClient.MulticastLoopback = true;
            _udpClient.ExclusiveAddressUse = false;
            _udpClient.DontFragment = true;            

            _packetReceiver = new UdpPacketReceiver(_udpClient, PacketReceivedDelegate, KnxFrameReceivedDelegate);
        }
        #endregion

        #region public methods
        /// <inheritdoc/>
        public override Task ConnectAsync()
        {
            return Task.Run(() =>
            {
                ConnectionState = KnxConnectionState.Opening;
                //_logger.Trace("ConnectAsync: Sending connection request to " + _remoteControlEndpoint + " with data " + data.ToHex());
                try
                {
                    _udpClient.BindSocket(_localEndPoint);
                    _udpClient.JoinMulticastGroup(RemoteEndpoint.Address, _localEndPoint.Address);
                    _packetReceiver.Start();
                    ConnectionState = KnxConnectionState.Open;
                    return;
                }
                catch 
                {
                    //_logger.Error("ConnectAsync: SocketException raised", se);
                    ConnectionState = KnxConnectionState.Invalid;
                    throw;
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
        public async override Task SendServiceAsync(IKnxNetIPService service)
        {
            if (ConnectionState != KnxConnectionState.Open)
                throw new KnxCommunicationException("Connection is not open");

            var frame = new KnxNetIPFrame(service);
            var data = frame.ToBytes();
            try
            {
                var bytessent = await _udpClient.SendAsync(data, RemoteEndpoint);
                if (bytessent == 0)
                    throw new KnxCommunicationException("sending data failed");
            }
            catch (SocketException se)
            {
                ConnectionState = KnxConnectionState.Invalid;
                throw new KnxCommunicationException("sending data failed", se);
            }
        }

        /// <inheritdoc/>
        public override Task SendCemiAsync(ICemiMessage message)
        {
            var service = new RoutingIndicationService(message);
            return SendServiceAsync(service);
        }
        #endregion

        #region private members
        private readonly IUdpClient _udpClient;
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
            OnFrameReceived(timestamp, frame, true);
            if (frame is RoutingIndicationService)
                OnCemiReceived(DateTime.UtcNow, (frame as RoutingIndicationService)!.CemiMessage);
        }
        #endregion

        #endregion
    }
}