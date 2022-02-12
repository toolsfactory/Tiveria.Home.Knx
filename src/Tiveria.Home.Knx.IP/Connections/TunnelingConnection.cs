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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Net.Sockets;
using Tiveria.Common.Extensions;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Services;
using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.IP.Connections
{
    /// <summary>
    /// CLass allowing to connect to a KNX IP Router or Interface using Tunneling.
    /// </summary>
    public class TunnelingConnection : IPConnectionBase
    {
        #region public constructors
        /// <summary>
        /// Create a new tunneling connection client
        /// </summary>
        /// <param name="localEndpoint">The IP endpoint on the local host</param>
        /// <param name="remoteEndpoint">The IP endpoint to connect to</param>
        /// <param name="configuration">Additional configuration parameters</param>
        /// <param name="logger">Instance of a logger object</param>
        public TunnelingConnection(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint, TunnelingConnectionConfiguration? configuration = null, ILogger<TunnelingConnection>? logger = null)
            : base(remoteEndpoint)
        {
            _localEndpoint = localEndpoint;
            _remoteControlEndpoint = remoteEndpoint;
            _logger = logger ?? NullLogger<TunnelingConnection>.Instance;
            _config = configuration ?? new TunnelingConnectionConfiguration();
            _udpClient = new UdpClient(_localEndpoint);
            _packetReceiver = new UdpPacketReceiver(_udpClient, PacketReceivedDelegate, KnxFrameReceivedDelegate);
        }

        /// <summary>
        /// DEPRECATED
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="localAddress"></param>
        /// <param name="localPort"></param>
        /// <param name="busmonitor"></param>
        /// <param name="natAware"></param>
        [Obsolete]
        public TunnelingConnection(IPAddress remoteAddress, ushort remotePort, IPAddress localAddress, ushort localPort, bool busmonitor = false, bool natAware = false)
            : this(new IPEndPoint(localAddress, localPort), new IPEndPoint(remoteAddress, remotePort), new TunnelingConnectionConfiguration() { UseBusMonitorMode = busmonitor, NatAware = natAware })
        {
            // ToDo: remove deprecated constructor
        }
        #endregion

        #region public properties
        /// <summary>
        /// Type of the current connection
        /// </summary>
        public ConnectionType ConnectionType => ConnectionType.Tunnel;
        #endregion

        #region public methods
        /// <inheritdoc/>
        public override async Task<bool> ConnectAsync()
        {
            _connectEvent.Reset();
            ConnectionState = KnxConnectionState.Opening;
            var data = CreateConnectionRequestFrame();
            try
            {
                _packetReceiver.Start();
                var bytessent = await _udpClient.SendAsync(data, data.Length, _remoteControlEndpoint).ConfigureAwait(false);
                if (bytessent == 0)
                {
                    ConnectionState = KnxConnectionState.Invalid;
                    return false;
                }
                _localEndpoint = (IPEndPoint) _udpClient.Client.LocalEndPoint!;
                var result = _connectEvent.WaitOne(_config.ConnectTimeout);
                if (!result) ConnectionState = KnxConnectionState.Invalid;
                return result;
            }
            catch
            {
                ConnectionState = KnxConnectionState.Invalid;
                return false;
            }
        }

        /// <inheritdoc/>
        public async override Task DisconnectAsync()
        {
            await InternalCloseAsync("User triggered.", false);
        }


        /// <summary>
        /// Send a generic IKnxNetIPService (potentially raw payload)
        /// </summary>
        /// <param name="frame">The frame to send</param>
        /// <exception cref="KnxCommunicationException">Thrown when sending the message failed</exception>
        public override async Task SendAsync(IKnxNetIPService service)
        {
            if (ConnectionState != KnxConnectionState.Open)
                throw new KnxCommunicationException("Connection is not open");

            var frame = new KnxNetIPFrame(service);
            var data = frame.ToBytes();
            var timeoutCTS = new CancellationTokenSource(_config.SendTimeout);
            var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(timeoutCTS.Token, _cancellationTokenSource.Token);
            try
            {
                var bytessent = await _udpClient.SendAsync(data, data.Length).WithCancellation(linkedCTS.Token);
                if (bytessent == 0)
                    throw new KnxCommunicationException("sending data failed");
            }
            catch (SocketException se)
            {
                ConnectionState = KnxConnectionState.Invalid;
                throw new KnxCommunicationException("sending data failed", se);
            }
        }

        /// <summary>
        /// Send a Cemi message to the KnxNetIP server in blocking mode <see cref="SendCemiAsync(Cemi.ICemiMessage cemi, bool blocking = true)"/>. Version & Connection headers are automatically generated.
        /// </summary>
        /// <param name="cemi">The cemi message to send</param>
        /// <exception cref="KnxCommunicationException">Thrown when sending the message failed</exception>
        public override Task SendCemiAsync(ICemiMessage message)
        {
            return SendCemiAsync(message, true);
        }

        /// <summary>
        /// Send a Cemi message to the KnxNetIP server. Version & Connection headers are automatically generated.
        /// </summary>
        /// <param name="cemi">The cemi message to send</param>
        /// <param name="blocking">If yes, the message is only sent if no other message is currently being processed. On top, the call only returns then after the Ack was received or timed out.</param>
        /// <exception cref="KnxCommunicationException">Thrown when sending the message failed</exception>
        public async Task SendCemiAsync(Cemi.ICemiMessage cemi, bool blocking = true)
        {
            var conheader = new ConnectionHeader(_channelId, SndSeqCounter);
            var service = new TunnelingRequestService(conheader, cemi);
            if (blocking)
                await DoSendServiceBlockingAsync(service);
            else
                await SendAsync(service);
        }
        #endregion

        #region private fields
        private readonly object _lock = new object();
        private readonly ManualResetEvent _closeEvent = new ManualResetEvent(false);
        private readonly ManualResetEventSlim _ackEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEvent _connectEvent = new ManualResetEvent(false);
        private IPEndPoint _localEndpoint;
        private readonly IPEndPoint _remoteControlEndpoint;
        private readonly ILogger<TunnelingConnection> _logger;
        private readonly UdpClient _udpClient;
        private IPEndPoint? _remoteDataEndpoint;
        private UdpPacketReceiver _packetReceiver;
        private HeartbeatMonitor? _heartbeatMonitor;
        private AckState _ackState = AckState.Ok;
        private readonly TunnelingConnectionConfiguration _config;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool disposedValue = false;
        #endregion

        #region private implementations
        private Task<bool> DoSendServiceBlockingAsync(IKnxNetIPService service)
        {
            var data = new KnxNetIPFrame(service).ToBytes();

            return Task.Run(() =>
            {
                try
                {
                    lock (_lock)
                    {
                        if (_ackState == AckState.Pending)
                            return false;
                        bool ackReceived = false;
                        InitAckReceiving();
                        for (var i = 0; i < _config.SendRepeats; i++)
                        {
                            _udpClient.SendAsync(data, data.Length, _remoteControlEndpoint);
                            ackReceived = _ackEvent.Wait(_config.AcknowledgeTimeout, _cancellationTokenSource.Token);
                            if (ackReceived)
                                break;
                        }
                        if (!ackReceived)
                            HandleAckNotReceived();
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    IncSndSeqCounter();
                }
            });
        }

        private void InitAckReceiving()
        {
            _ackEvent.Reset();
            _ackState = AckState.Pending;
        }

        private void HandleAckNotReceived()
        {
            _ackState = AckState.Error;
            InternalCloseAsync("Ack not received", false).Wait();
        }

        #region closing connection
        private async Task InternalCloseAsync(string reason, bool external)
        {
            lock (_lock)
            {
                if (ConnectionState == KnxConnectionState.Closing || ConnectionState == KnxConnectionState.Closed)
                    return;
                ConnectionState = KnxConnectionState.Closing;
            }
            //_logger.Info("Closing connection. " + reason);
            _heartbeatMonitor?.Stop();
            if (!external)
            {
                await SendDisconnectRequestAsync().ConfigureAwait(false);
                _closeEvent.WaitOne(1000);
            }
            await Task.Run(() => _packetReceiver.Stop()).ConfigureAwait(false);
            ConnectionState = KnxConnectionState.Closed;
            _cancellationTokenSource.Cancel();
            _udpClient.Close();
        }

        private byte[] CreateDisconnectFrame()
        {
            var hpai = new Hpai(Enums.HPAIEndpointType.IPV4_UDP, _localEndpoint.Address, (ushort)_localEndpoint.Port);
            var service = new DisconnectRequestService(_channelId, hpai);
            var frame = new KnxNetIPFrame(service);
            return frame.ToBytes();
        }

        private Task SendDisconnectRequestAsync()
        {
            return Task.Run(() =>
            {
                lock (_lock)
                {
                    if (ConnectionState == KnxConnectionState.Closed)
                        return;
                    var frame = CreateDisconnectFrame();
                    _udpClient.Send(frame, frame.Length, _remoteControlEndpoint);
                    var remaining = 1000 * 10;
                    while (remaining > 0)
                    {
                        if (_closeEvent.WaitOne(500))
                            break;
                        remaining -= 500;
                    }
                }
            });
        }
        #endregion

        #region sending connection request
        private byte[] CreateConnectionRequestFrame()
        {
            var cri     = new CRITunnel(_config.UseBusMonitorMode ? TunnelingLayer.TUNNEL_BUSMONITOR : TunnelingLayer.TUNNEL_LINKLAYER);
            var hpai    = new Structures.Hpai(Enums.HPAIEndpointType.IPV4_UDP, _localEndpoint.Address, (ushort)_localEndpoint.Port);
            var service = new ConnectionRequestService(hpai, hpai, cri);
            var frame   = new KnxNetIPFrame(service);
            return frame.ToBytes();
        }
        #endregion

        #region handling connection response
        /// <summary>
        /// handles frames that contain a ConnectRequest message
        /// </summary>
        /// <param name="frame">The full KNXNetIP Frame</param>
        /// <param name="remoteEndpoint">The remote endpoint that sent the frame</param>
        private void HandleConnectResponse(ConnectionResponseService response, IPEndPoint remoteEndpoint)
        {
            // to be sure, chack again that HPAI is for UDP in IPv4
            if ((response.Status == ErrorCodes.NoError) &&
                (response.DataEndpoint.EndpointType == HPAIEndpointType.IPV4_UDP))
            {
                //Everything ok. Lets fill all fields
                var ep = response.DataEndpoint;
                VerifyRemoteDataEndpointforNAT(ep, remoteEndpoint);
                _channelId = response.ChannelId;
                ConnectionState = KnxConnectionState.Open;
                SetupHeartbeatMonitor();
            }
            else
            {
                _channelId = 0;
                ConnectionState = KnxConnectionState.Invalid;
            }
            _connectEvent.Set();
        }

        private void SetupHeartbeatMonitor()
        {
            var hpai = new Structures.Hpai(Enums.HPAIEndpointType.IPV4_UDP, _localEndpoint.Address, (ushort)_localEndpoint.Port);
            _heartbeatMonitor = new HeartbeatMonitor(_remoteControlEndpoint, hpai, _channelId, 5, 5, 2);
            _heartbeatMonitor.HeartbeatFailed = HeartbeatFailed;
            _heartbeatMonitor.HeartbeatOk = HeartbeatOk;
            _heartbeatMonitor.Start();
        }

        private void HeartbeatFailed(bool severe, string message)
        {
            if (severe)
            {
                //_logger.Error("Heartbeat failed. " + message);
            }
            else
            {
                //_logger.Info("Heartbeat failed. " + message);
            }
            _ = InternalCloseAsync("Hartbeat failed - " + message, false);
        }

        private void HeartbeatOk()
        {
        }

        private void VerifyRemoteDataEndpointforNAT(Structures.Hpai remoteHPAI, IPEndPoint remoteEndpoint)
        {
            if (_config.NatAware && (remoteHPAI.Ip == IPAddress.Any || remoteHPAI.Port == 0))
            {
                _remoteDataEndpoint = remoteEndpoint;
                //_logger.Debug("ConnectionResponse: NAT mode, using socket endpoint " + _remoteDataEndpoint);
            }
            else
            {
                _remoteDataEndpoint = new IPEndPoint(remoteHPAI.Ip, remoteHPAI.Port);
                //_logger.Debug("ConnectionResponse: using protocol endpoint " + _remoteDataEndpoint);
            }
        }
        #endregion

        #region handling disconnect request
        private void HandleDisconnectRequest(DisconnectRequestService request)
        {
            var service = new DisconnectResponseService(request.ChannelId);
            var frame = new KnxNetIPFrame(service);
            var data = frame.ToBytes();
            _udpClient.Send(data, data.Length, _remoteDataEndpoint);

            _ = InternalCloseAsync("External request received.", true);
        }

        #endregion

        #region handling disconnect response
        private void HandleDisconnectResponse(DisconnectResponseService response)
        {
            if (response.Status != ErrorCodes.NoError)
            { }
                //_logger.Warn($"Connection closed with status code " + response.Status.ToDescription());
            _closeEvent.Set();
        }
        #endregion

        #region handling tunneling requests
        private void HandleTunnelingRequest(TunnelingRequestService request)
        {
            var seq = request.ConnectionHeader.SequenceCounter;
            if (!ValidateReqSequenceCounter(seq))
                return;

            var service = new TunnelingAcknowledgeService(request.ConnectionHeader);
            var frame = new KnxNetIPFrame(service);
            var data = frame.ToBytes();
            _udpClient.Send(data, data.Length, _remoteDataEndpoint);
            OnCemiReceived(DateTime.UtcNow, request.CemiMessage) ;
        }

        private bool ValidateReqSequenceCounter(byte rcvSeq)
        {
            // copied from Calimero
            // Workaround for missed request problem (not part of the knxnet/ip tunneling spec):
            // we missed a single request, hence, the receive sequence is one behind. If the remote
            // endpoint didn't terminate the connection, but continues to send requests, this workaround
            // re-syncs with the sequence of the sender.
            var expSeq = RcvSeqCounter;
            var missed = (expSeq - 1 == expSeq);
            if (missed && _config.ResyncSequenceNumbers)
            {
                //_logger.Error($"tunneling request with rcv-seq '{rcvSeq}', expected '{expSeq}' -> re-sync with server (1 tunneled msg lost)");
                IncRcvSeqCounter();
                expSeq++;
            }
            IncRcvSeqCounter();
            return (rcvSeq == expSeq) || (rcvSeq == ++expSeq);
        }

        private void HandleTunnelingAck(TunnelingAcknowledgeService serviceType)
        {
            if (_ackState != AckState.Pending)
                return;
            if (serviceType.ConnectionHeader.ChannelId != _channelId)
                return;
            if (SndSeqCounter != serviceType.ConnectionHeader.SequenceCounter)
                return;
            _ackState = AckState.Received;
            _ackEvent.Set();
        }

        #endregion

        #region handling ConnectionState response
        private void HandleConnectionStateResponse(ConnectionStateResponseService response)
        {
            if (_heartbeatMonitor != null)
                _heartbeatMonitor.HandleResponse(response);
        }
        #endregion

        #region handling unknown servicetype
/*
        private void HandleUnknownServiceType(UnknownService serviceType)
        {
            _logger.Warn($"Unknown Servicetype: {serviceType.ServiceTypeRaw:x2}. Data: " + serviceType.FrameRaw.ToHex());
        }
*/
        #endregion

        #region packet and knx frame receive delegates
        private void PacketReceivedDelegate(DateTime timestamp, IPEndPoint source, IPEndPoint receiver, byte[] data)
        {
            OnDataReceived(timestamp, data);
        }

        private void KnxFrameReceivedDelegate(DateTime timestamp, IPEndPoint source, IPEndPoint receiver, IKnxNetIPFrame frame)
        {
            switch (frame.FrameHeader.ServiceTypeIdentifier)
            {
                case ServiceTypeIdentifier.ConnectResponse:
                    HandleConnectResponse((ConnectionResponseService)frame.Service, source);
                    break;
                case ServiceTypeIdentifier.DisconnectRequest:
                    HandleDisconnectRequest((DisconnectRequestService)frame.Service);
                    break;
                case ServiceTypeIdentifier.DisconnectResponse:
                    HandleDisconnectResponse((DisconnectResponseService)frame.Service);
                    break;
                case ServiceTypeIdentifier.TunnelingRequest:
                    HandleTunnelingRequest((TunnelingRequestService)frame.Service);
                    break;
                case ServiceTypeIdentifier.TunnelingAcknowledge:
                    HandleTunnelingAck((TunnelingAcknowledgeService)frame.Service);
                    break;
                case ServiceTypeIdentifier.ConnectionStateResponse:
                    HandleConnectionStateResponse((ConnectionStateResponseService)frame.Service);
                    break;
                    /*
                case ServiceTypeIdentifier.UNKNOWN:
                    HandleUnknownServiceType((UnknownService)frame);
                    break;
                    */
            }
            OnFrameReceived(timestamp, frame, true);
        }
        #endregion

        protected override string GetConnectionName()
        {
            return "TunnelingConnection - " + (_config.UseBusMonitorMode ? "BusMonitoring" : "Standard");
        }

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
        #endregion

        private enum AckState
        {
            Ok,
            Pending,
            Received,
            Error
        }
    }
}