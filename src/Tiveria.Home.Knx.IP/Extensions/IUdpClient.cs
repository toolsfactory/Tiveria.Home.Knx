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
    /// <summary>
    /// Interface for a facade of the standard <see cref="UdpClient"/>
    /// </summary>
    public interface IUdpClient : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        int Available { get; }

        /// <summary>
        /// 
        /// </summary>
        bool DontFragment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool ExclusiveAddressUse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool EnableBroadcast { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool MulticastLoopback { get; set; }

        /// <summary>
        /// 
        /// </summary>
        short Ttl { get; set; }

        /// <summary>
        /// Endpoint of the remote host
        /// </summary>
        IPEndPoint? RemoteEndpoint { get; }

        /// <summary>
        /// local endpoint. Updated after first active use so that an automatic port selection is reflected.
        /// </summary>
        IPEndPoint? LocalEndPoint { get; }

        /// <summary>
        /// Closes the connection of the client.
        /// </summary>
        void Close();

        /// <summary>
        /// Sets the default remote host
        /// </summary>
        /// <param name="endPoint"></param>
        void Connect(IPEndPoint endPoint);

        /// <summary>
        /// Returns a UDP datagram asynchronously that was sent by a remote host.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<UdpReceiveResult> ReceiveAsync();

        /// <summary>
        /// Returns a UDP datagram asynchronously that was sent by a remote host.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns a UDP datagram asynchronously that was sent by a remote host.
        /// </summary>
        /// <param name="timeoutMS">max time to receive data in milliseconds</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        ValueTask<UdpReceiveResult> ReceiveAsync(int timeoutMS, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a UDP datagram asynchronously to a remote host.
        /// </summary>
        /// <param name="datagram">An array of type Byte that specifies the UDP datagram that you intend to send represented as an array of bytes.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        public Task<int> SendAsync(byte[] datagram, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a UDP datagram asynchronously to a remote host.
        /// </summary>
        /// <param name="datagram">An array of type Byte that specifies the UDP datagram that you intend to send represented as an array of bytes.</param>
        /// <param name="bytes">The amount of bytes to be sent.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>bytes sent</returns>
        Task<int> SendAsync(byte[] datagram, int bytes, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a UDP datagram asynchronously to a remote host.
        /// </summary>
        /// <param name="datagram">An array of type Byte that specifies the UDP datagram that you intend to send represented as an array of bytes.</param>
        /// <param name="bytes">The amount of bytes to be sent.</param>
        /// <param name="timeoutMS">max time to send the data in milliseconds</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>bytes sent</returns>
        Task<int> SendAsync(byte[] datagram, int bytes, int timeoutMS, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a UDP datagram asynchronously to a remote host.
        /// </summary>
        /// <param name="datagram">An array of type Byte that specifies the UDP datagram that you intend to send represented as an array of bytes.</param>
        /// <param name="endPoint"></param>
        /// <returns>bytes sent</returns>
        Task<int> SendAsync(byte[] datagram, System.Net.IPEndPoint? endPoint);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datagram"></param>
        /// <param name="endPoint"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SendAsync(byte[] datagram, System.Net.IPEndPoint? endPoint, CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datagram"></param>
        /// <param name="endPoint"></param>
        /// <param name="timeoutMS"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SendAsync(byte[] datagram, System.Net.IPEndPoint? endPoint, int timeoutMS, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionLevel"></param>
        /// <param name="optionName"></param>
        /// <param name="optionValue"></param>
        void SetSocketOption(System.Net.Sockets.SocketOptionLevel optionLevel, System.Net.Sockets.SocketOptionName optionName, object optionValue); 
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionLevel"></param>
        /// <param name="optionName"></param>
        /// <param name="optionValue"></param>
        void SetSocketOption(System.Net.Sockets.SocketOptionLevel optionLevel, System.Net.Sockets.SocketOptionName optionName, int optionValue);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionLevel"></param>
        /// <param name="optionName"></param>
        /// <param name="optionValue"></param>
        void SetSocketOption(System.Net.Sockets.SocketOptionLevel optionLevel, System.Net.Sockets.SocketOptionName optionName, byte[] optionValue);
       
        /// <summary>
       /// 
       /// </summary>
       /// <param name="optionLevel"></param>
       /// <param name="optionName"></param>
       /// <param name="optionValue"></param>
        void SetSocketOption(System.Net.Sockets.SocketOptionLevel optionLevel, System.Net.Sockets.SocketOptionName optionName, bool optionValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multicastAddr"></param>
        /// <param name="localAddress"></param>
        void JoinMulticastGroup(System.Net.IPAddress multicastAddr, System.Net.IPAddress localAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localEP"></param>
        void BindSocket(System.Net.EndPoint localEP);
    }
}
