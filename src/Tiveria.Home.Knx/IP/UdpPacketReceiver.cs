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
    
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tiveria.Common.Extensions;

namespace Tiveria.Home.Knx.IP
{
    public class UdpPacketReceiver : IUdpPacketReceiver
    {
        private readonly Tiveria.Common.Logging.ILogger _logger = Tiveria.Home.Knx.Utils.LogFactory.GetLogger("Tiveria.Home.Knx.UdpPacketReceiver");
        private readonly UdpClient _client;
        private readonly CancellationToken _cancellationToken;
        private readonly PacketReceivedDelegate _packetReceived;
        private readonly KnxFrameReceivedDelegate _knxFrameReceived;
        private bool _running;

        public bool Running => _running;
        public PacketReceivedDelegate PacketReceived => _packetReceived;
        public KnxFrameReceivedDelegate KnxFrameReceived => _knxFrameReceived;

        public UdpPacketReceiver(UdpClient client, PacketReceivedDelegate packetReceived, KnxFrameReceivedDelegate knxFrameReceived)
        {
            _client = client;
            _packetReceived = packetReceived;
            _knxFrameReceived = knxFrameReceived;
            _cancellationToken = new CancellationToken();
            _running = false;
        }

        private void OnPacketReceived(IPEndPoint source, byte[] data)
        {
            PacketReceived?.Invoke(DateTime.Now, source, data);
        }

        private void OnKnxFrameReceived(IPEndPoint source, KnxNetIPFrame frame)
        {
            KnxFrameReceived?.Invoke(DateTime.Now, source, frame);
        }

        public bool Start()
        {
            if (_running)
                return false;
            StartListen();
            return _running = true;
        }

        public void Stop()
        {
            _client.Close();
            _cancellationToken.ThrowIfCancellationRequested();
        }

        private void StartListen()
        {
            Task.Run(async () =>
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var receivedResults = await _client.ReceiveAsync()
                                                           .WithCancellation(_cancellationToken)
                                                           .ConfigureAwait(false);
                        OnPacketReceived(receivedResults.RemoteEndPoint, receivedResults.Buffer);
                        TryParseKnxFrame(receivedResults.RemoteEndPoint, receivedResults.Buffer);
                    }
                    catch (OperationCanceledException ex)
                    { }
                }
                _running = false;
            });
        }

        private void TryParseKnxFrame(IPEndPoint remoteEndPoint, byte[] buffer)
        {
            try
            {
                var frame = KnxNetIPFrame.Parse(buffer);
                OnKnxFrameReceived(remoteEndPoint, frame);
            }
            catch (Exceptions.BufferException be)
            {
                _logger.Warn("Invalid frame received", be);
            }
            catch (ArgumentException ae)
            {
                _logger.Warn("Invalid frame received", ae);
            }
        }
    }
}
