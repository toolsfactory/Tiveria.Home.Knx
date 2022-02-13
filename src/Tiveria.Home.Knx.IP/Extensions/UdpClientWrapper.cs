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
using Tiveria.Common.Extensions;

namespace Tiveria.Home.Knx.IP.Extensions
{
    /// <inheritdoc/>
    public class UdpClientWrapper : IUdpClient
    {
        private UdpClient _client;

        /// <inheritdoc/>
        public IPEndPoint? RemoteEndpoint => _client.Client.RemoteEndPoint as IPEndPoint;

        /// <inheritdoc/>
        public IPEndPoint? LocalEndPoint => _client.Client.LocalEndPoint as IPEndPoint;

        /// <inheritdoc/>
        public int Available => _client.Available;

        /// <inheritdoc/>
        public bool DontFragment { get => _client.DontFragment; set => _client.DontFragment = value; }

        /// <inheritdoc/>
        public bool ExclusiveAddressUse { get => _client.ExclusiveAddressUse; set => _client.ExclusiveAddressUse = value; }

        /// <inheritdoc/>
        public bool EnableBroadcast { get => _client.EnableBroadcast; set => _client.EnableBroadcast = value; }

        /// <inheritdoc/>
        public bool MulticastLoopback { get => _client.MulticastLoopback; set => _client.MulticastLoopback = value; }

        /// <inheritdoc/>
        public short Ttl { get => _client.Ttl; set => _client.Ttl = value; }

        /// <summary>
        /// Creates a new wrapper around the standard <see cref="UdpClient"/>
        /// </summary>
        public UdpClientWrapper()
        {
            _client = new UdpClient();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClient"/> class and binds it to the specified local endpoint. 
        /// </summary>
        /// <param name="localEP">An <see cref="IPEndPoint"/> that represents the local endpoint to which you bind the UDP connection.</param>
        public UdpClientWrapper(IPEndPoint localEP)
        {
            _client = new UdpClient(localEP);
        }

        /// <inheritdoc/>
        public void Close()
        {
            _client.Close();
        }

        /// <inheritdoc/>
        public void Connect(IPEndPoint endPoint)
        {
            _client.Connect(endPoint);
        }

        /// <inheritdoc/>
        public ValueTask<UdpReceiveResult> ReceiveAsync(int timeoutMS, CancellationToken cancellationToken)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource(timeoutMS).Token, cancellationToken);
#if NET6_0_OR_GREATER
            return _client.ReceiveAsync(cts.Token);
#else
            return new ValueTask<UdpReceiveResult>(_client.ReceiveAsync().WithCancellation(cts.Token));
#endif
        }

        /// <inheritdoc/>
        public ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken)
        {
#if NET6_0_OR_GREATER
            return _client.ReceiveAsync(cancellationToken);
#else
            return new ValueTask<UdpReceiveResult>(_client.ReceiveAsync().WithCancellation(cancellationToken));
#endif
        }

        /// <inheritdoc/>
        public Task<UdpReceiveResult> ReceiveAsync()
        {
            return _client.ReceiveAsync();
        }

        /// <inheritdoc/>
        public Task<int> SendAsync(byte[] datagram, CancellationToken cancellationToken = default)
        {
            return SendAsync(datagram, datagram.Length, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<int> SendAsync(byte[] datagram, int bytes, CancellationToken cancellationToken = default)
        {
            return _client.SendAsync(datagram, bytes).WithCancellation(cancellationToken);
        }

        /// <inheritdoc/>
        public Task<int> SendAsync(byte[] datagram, int bytes, int timeoutMS, CancellationToken cancellationToken = default)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource(timeoutMS).Token, cancellationToken);
            return SendAsync(datagram, bytes, cts.Token);
        }

        /// <inheritdoc/>
        public Task<int> SendAsync(byte[] datagram, IPEndPoint? endPoint)
        {
            return _client.SendAsync(datagram, datagram.Length, endPoint);
        }

        /// <inheritdoc/>
        public Task<int> SendAsync(byte[] datagram, IPEndPoint? endPoint, CancellationToken cancellationToken)
        {
            return _client.SendAsync(datagram, datagram.Length, endPoint).WithCancellation(cancellationToken);
        }

        /// <inheritdoc/>
        public Task<int> SendAsync(byte[] datagram, IPEndPoint? endPoint, int timeoutMS, CancellationToken cancellationToken = default)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource(timeoutMS).Token, cancellationToken);
            return _client.SendAsync(datagram, datagram.Length, endPoint).WithCancellation(cts.Token);
        }


        /// <inheritdoc/>
        public void Dispose()
        {
            _client?.Dispose();
        }

        /// <inheritdoc/>
        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, object optionValue)
        {
            _client.Client.SetSocketOption(optionLevel, optionName, optionValue);
        }

        /// <inheritdoc/>
        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
        {
            _client.Client.SetSocketOption(optionLevel, optionName, optionValue);
        }

        /// <inheritdoc/>
        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
        {
            _client.Client.SetSocketOption(optionLevel, optionName, optionValue);
        }

        /// <inheritdoc/>
        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, bool optionValue)
        {
            _client.Client.SetSocketOption(optionLevel, optionName, optionValue);
        }

        /// <inheritdoc/>
        public void JoinMulticastGroup(IPAddress multicastAddr, IPAddress localAddress)
        {
            _client.JoinMulticastGroup(multicastAddr, localAddress);
        }

        /// <inheritdoc/>
        public void BindSocket(EndPoint localEP)
        {
            _client.Client.Bind(localEP);
        }
    }
}
