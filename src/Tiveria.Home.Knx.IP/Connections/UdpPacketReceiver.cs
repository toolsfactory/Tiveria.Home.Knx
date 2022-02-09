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
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.IP.Connections
{
    public class UdpPacketReceiver
    {
//        private readonly Tiveria.Common.Logging.ILogger _logger = Tiveria.Home.Knx.Utils.LogFactory.GetLogger("Tiveria.Home.Knx.UdpPacketReceiver");
        private readonly UdpClient _client;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly PacketReceivedDelegate? _packetReceived;
        private readonly KnxFrameReceivedDelegate? _knxFrameReceived;
        private bool _running;

        public bool Running => _running;
        public PacketReceivedDelegate? PacketReceived => _packetReceived;
        public KnxFrameReceivedDelegate? KnxFrameReceived => _knxFrameReceived;

        public UdpPacketReceiver(UdpClient client, PacketReceivedDelegate packetReceived, KnxFrameReceivedDelegate? knxFrameReceived)
        {
            _client = client;
            _packetReceived = packetReceived;
            _knxFrameReceived = knxFrameReceived;
            _cancellationTokenSource = new CancellationTokenSource();
            _running = false;
        }

        private void OnPacketReceived(IPEndPoint source, IPEndPoint receiver, byte[] data)
        {
            PacketReceived?.Invoke(DateTime.Now, source, receiver, data);
        }

        private void OnKnxFrameReceived(IPEndPoint source, IPEndPoint receiver, IKnxNetIPFrame frame)
        {
            KnxFrameReceived?.Invoke(DateTime.Now, source, receiver, frame);
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
            if (!_running)
                return;
            _client.Close();
            _cancellationTokenSource.Cancel();
            _running=false;
        }

        private void StartListen()
        {
            Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        var receivedResults = await _client.ReceiveAsync()
                                                           .WithCancellation(_cancellationTokenSource.Token)
                                                           .ConfigureAwait(false);
                        OnPacketReceived(receivedResults.RemoteEndPoint, (IPEndPoint)_client.Client.LocalEndPoint, receivedResults.Buffer);
                        TryParseKnxFrame(receivedResults.RemoteEndPoint, (IPEndPoint)_client.Client.LocalEndPoint, receivedResults.Buffer);
                    }
                    catch (OperationCanceledException ex)
                    { }
                }
                _running = false;
            });
        }

        private void TryParseKnxFrame(IPEndPoint remoteEndPoint,IPEndPoint receiver, byte[] buffer)
        {
            if (!KnxNetIPFrame.TryParse(buffer, out var frame))
            {
                // ToDo: Add errorhandling for frames that could not be parsed
                return;
            }
            OnKnxFrameReceived(remoteEndPoint, receiver, frame!);
        }
    }
}
