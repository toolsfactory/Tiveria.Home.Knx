using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tiveria.Common.Extensions;

namespace Tiveria.Knx.IP
{
    public class UdpPacketReceiver : IUdpPacketReceiver
    {
        private readonly Tiveria.Common.Logging.ILogger _logger = Tiveria.Knx.Utils.LogFactory.GetLogger("Tiveria.Knx.UdpPacketReceiver");
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
