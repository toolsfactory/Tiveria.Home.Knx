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
using Tiveria.Home.Knx.IP.Extensions;

namespace Tiveria.Home.Knx.IP.Connections
{
    /// <summary>
    /// CLass allowing to connect to a KNX IP Router or Interface using Tunneling.
    /// </summary>
    public class TunnelingConnection : IPConnectionBase
    {
        #region public constants
        /// <summary>
        /// Time in milliseconds the system waits for a disconnect repsonse from the knx interface
        /// </summary>
        public const int DisconnectConfirmationTimeoutMS = 5000;
        #endregion

        #region public properties

        /// <summary>
        /// Channel id used between the two endpoints
        /// </summary>
        public byte ChannelId { get; private set; }

        /// <summary>
        /// Type of the current connection
        /// </summary>
        public ConnectionType ConnectionType => ConnectionType.Tunnel;

        /// <summary>
        /// Shows the counter used for the next packet to be sent
        /// </summary>
        public byte SendSequenceCounter => _cntManager.SndSeqCounter;

        /// <summary>
        /// Shows the counter of the last received packet
        /// </summary>
        public byte ReceiveSequenceCounter => _cntManager.RcvSeqCounter;
        #endregion

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
            _logger = logger ?? NullLogger<TunnelingConnection>.Instance;
            _config = configuration ?? new TunnelingConnectionConfiguration();
            _udpClient = UdpClientFactory.GetInstanceWithEP(_localEndpoint);
            _packetReceiver = new UdpPacketReceiver(_udpClient, PacketReceivedDelegate, KnxFrameReceivedDelegate);
        }
        #endregion

        #region public methods
        /// <inheritdoc/>
        public override async Task ConnectAsync()
        {
            await InternalConnectAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async override Task DisconnectAsync()
        {
            await InternalCloseAsync("User triggered.", false);
        }


        /// <inheritdoc/>
        public override async Task SendServiceAsync(IKnxNetIPService service)
        {
            CheckConnectionAndAckStates();

            var frame = new KnxNetIPFrame(service);
            var data = frame.ToBytes();
            var timeoutCTS = new CancellationTokenSource(_config.SendTimeout);
            var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(timeoutCTS.Token, _closingCancelationTokenSource.Token);
            try
            {
                if(0 == await _udpClient.SendAsync(data, data.Length).WithCancellation(linkedCTS.Token))
                    throw new KnxCommunicationException("sending data failed");
            }
            catch (SocketException se)
            {
                ConnectionState = KnxConnectionState.Invalid;
                throw new KnxCommunicationException("sending data failed", se);
            }
        }

        /// <summary>
        /// Send a Cemi message to the KnxNetIP server in blocking mode. Version and Connection headers are automatically generated.
        /// </summary>
        /// <param name="cemi">The cemi message to send</param>
        /// <exception cref="KnxCommunicationException">Thrown when sending the message failed</exception>
        public async override Task SendCemiAsync(Cemi.ICemiMessage cemi)
        {
            await InternalSendCemiAsync(cemi).ConfigureAwait(false);
        }

        #endregion

        #region private fields
        private readonly object _lock = new object();
        private readonly ManualResetEvent _closeEvent = new ManualResetEvent(false);
        private readonly ManualResetEventSlim _ackEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEvent _connectEvent = new ManualResetEvent(false);
        private IPEndPoint _localEndpoint;
        private readonly ILogger<TunnelingConnection> _logger;
        private readonly IUdpClient _udpClient;
        private UdpPacketReceiver _packetReceiver;
        private HeartbeatMonitor? _heartbeatMonitor;
        private AckState _ackState = AckState.Ok;
        private readonly TunnelingConnectionConfiguration _config;
        private readonly CancellationTokenSource _closingCancelationTokenSource = new CancellationTokenSource();
        private bool disposedValue = false;
        private SequenceCountersManager _cntManager = new SequenceCountersManager();
        #endregion

        #region private implementations

        #region internal open connection
        private async Task InternalConnectAsync()
        {
            _logger.TraceBeginFunc();
            InitializeConnection();
            try
            {
                _packetReceiver.Start();
                if (0 == await _udpClient.SendAsync(CreateConnectionRequestFrame()))
                    throw new KnxConnectionException("Could not send connect request");
                if (!WaitForConnectEvent())
                {
                    ConnectionState = KnxConnectionState.Invalid;
                    _logger.LogWarning("Connect event not received");
                } else
                {
                    SetupHeartbeatMonitor();
                    _localEndpoint = (IPEndPoint) _udpClient.LocalEndPoint!;
                }
                _logger.TraceEndFunc();
            }
            catch (Exception ex)
            {
                ConnectionState = KnxConnectionState.Invalid;
                _logger.LogTunnelingConnectionFailed(ex);
                throw;
            }
        }

       private void InitializeConnection()
        {
            if (ConnectionState != KnxConnectionState.Initialized)
                throw new KnxCommunicationException("Connection is not in the correct state to be opened");
            ConnectionState = KnxConnectionState.Opening;
            _connectEvent.Reset();
            _udpClient.Connect(RemoteEndpoint);
        }         
        
        private byte[] CreateConnectionRequestFrame()
        {
            var cri     = new CRITunnel(_config.UseBusMonitorMode ? TunnelingLayer.TUNNEL_BUSMONITOR : TunnelingLayer.TUNNEL_LINKLAYER);
            var hpai    = new Structures.Hpai(Enums.HPAIEndpointType.IPV4_UDP, _localEndpoint.Address, (ushort)_localEndpoint.Port);
            var service = new ConnectionRequestService(hpai, hpai, cri);
            var frame   = new KnxNetIPFrame(service);
            return frame.ToBytes();
        }

        private bool WaitForConnectEvent()
        {
            return _connectEvent.WaitOne(_config.ConnectTimeout);
        }

        private void HandleConnectResponse(ConnectionResponseService response, IPEndPoint remoteEndpoint)
        {
            // to be sure, chack again that HPAI is for UDP in IPv4
            if ((response.Status == ErrorCodes.NoError) &&
                (response.DataEndpoint.EndpointType == HPAIEndpointType.IPV4_UDP))
            {
                ChannelId = response.ChannelId;
                ConnectionState = KnxConnectionState.Open;
            }
            else
            {
                ChannelId = 0;
                ConnectionState = KnxConnectionState.Invalid;
            }
            _connectEvent.Set();
        }

        private void SetupHeartbeatMonitor()
        {
            var hpai = new Hpai(Enums.HPAIEndpointType.IPV4_UDP, _localEndpoint.Address, (ushort)_localEndpoint.Port);
            _heartbeatMonitor = new HeartbeatMonitor(_udpClient, hpai, ChannelId, HeartbeatOk, HeartbeatFailed);
            _heartbeatMonitor.Start();
        }

        #endregion

        #region internal close connection
        private async Task InternalCloseAsync(string reason, bool external)
        {
            lock (_lock)
            {
                if (ConnectionState == KnxConnectionState.Closing || ConnectionState == KnxConnectionState.Closed)
                    return;
                ConnectionState = KnxConnectionState.Closing;
            }
            _closingCancelationTokenSource.Cancel();
            _heartbeatMonitor?.Stop();
            if (!external)
            {
                await SendDisconnectRequestAsync();
                _closeEvent.WaitOne(DisconnectConfirmationTimeoutMS);
            }
            ConnectionState = KnxConnectionState.Closed;
            _packetReceiver.Stop();
            _udpClient.Close();
        }

        private byte[] CreateDisconnectFrame()
        {
            var hpai = new Hpai(Enums.HPAIEndpointType.IPV4_UDP, _localEndpoint.Address, (ushort)_localEndpoint.Port);
            var service = new DisconnectRequestService(ChannelId, hpai);
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
                    _udpClient.SendAsync(frame).Wait();
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

        #region handling disconnect response
        private void HandleDisconnectResponse(DisconnectResponseService response)
        {
            if (response.Status != ErrorCodes.NoError)
                _logger.LogWarning($"Connection closed with status code " + response.Status.ToDescription());
            _closeEvent.Set();
        }
        #endregion

        #endregion

        #region handling heartbeats
        private void HeartbeatFailed(bool severe, string message)
        {
            _ = InternalCloseAsync("Hartbeat failed - " + message, false);
        }

        private void HeartbeatOk()
        {
        }
        #endregion handling heartbeats

        #region sending in blocked mode

        private Task InternalSendCemiAsync(ICemiMessage message)
        {
            CheckConnectionAndAckStates();

            return Task.Run(() =>
            {
                var data = CreateFrameData(message);
                InitAckReceiving();
                try
                {
                    for (var i = 0; i < _config.SendRepeats; i++)
                    {
                        if (SendDataAndWaitForAck(data))
                        {
                            _cntManager.IncSndSeqCounter();
                            return;
                        }
                    }
                    HandleAckNotReceived();
                }
                catch (SocketException se)
                {
                    ConnectionState = KnxConnectionState.Invalid;
                    throw new KnxCommunicationException("sending data failed", se);
                }
            });
        }

        private void CheckConnectionAndAckStates()
        {
            if (ConnectionState != KnxConnectionState.Open)
                throw new KnxCommunicationException("Connection is not open");
            if (_ackState == AckState.Pending)
                throw new KnxCommunicationException("Cannot send another Cemi message or frame when previous one not yet acknowledged");
        }

        private byte[] CreateFrameData(ICemiMessage message)
        {
            var conheader = new ConnectionHeader(ChannelId, SendSequenceCounter);
            var service = new TunnelingRequestService(conheader, message);
            var data = new KnxNetIPFrame(service).ToBytes();
            return data;
        }

        private void InitAckReceiving()
        {
            _ackEvent.Reset();
            _ackState = AckState.Pending;
        }

        private bool SendDataAndWaitForAck(byte[] data)
        {
            _udpClient.SendAsync(data, data.Length);
            return _ackEvent.Wait(_config.AcknowledgeTimeout, _closingCancelationTokenSource.Token);

        }

        private void HandleAckNotReceived()
        {
            _ackState = AckState.Error;
            InternalCloseAsync("Ack not received", false).Wait();
        }
        private void HandleTunnelingAck(TunnelingAcknowledgeService serviceType)
        {
            if (_ackState != AckState.Pending)
                return;
            if (serviceType.ConnectionHeader.ChannelId != ChannelId)
                return;
            if (SendSequenceCounter != serviceType.ConnectionHeader.SequenceCounter)
                return;
            _ackState = AckState.Received;
            _ackEvent.Set();
        }

        #endregion sending in blocked mode

        #region handling disconnect request
        private void HandleDisconnectRequest(DisconnectRequestService request)
        {
            var service = new DisconnectResponseService(request.ChannelId);
            var frame = new KnxNetIPFrame(service);
            var data = frame.ToBytes();
            _udpClient.SendAsync(data).Wait();
            _ = InternalCloseAsync("External request received.", true);
        }

        #endregion

        #region handling tunneling requests
        private void HandleTunnelingRequest(TunnelingRequestService request)
        {
            var seq = request.ConnectionHeader.SequenceCounter;
            if (!_cntManager.ValidateReqSequenceCounter(seq, _config.ResyncSequenceNumbers))
                return;

            SendTunnelingAcknowledge(request);
            OnCemiReceived(DateTime.UtcNow, request.CemiMessage);
        }

        private void SendTunnelingAcknowledge(TunnelingRequestService request)
        {
            var service = new TunnelingAcknowledgeService(request.ConnectionHeader);
            var frame = new KnxNetIPFrame(service);
            var data = frame.ToBytes();
            _udpClient.SendAsync(data);
        }

        #endregion

        #region handling ConnectionState response
        private void HandleConnectionStateResponse(ConnectionStateResponseService response)
        {
            if (_heartbeatMonitor != null)
                _heartbeatMonitor.HandleResponse(response);
        }
        #endregion

        #region handling unknown servicetype request
        private void HandleUnknownServiceType(RawService serviceType)
        {
            _logger.LogWarning($"Unknown Servicetype: {serviceType.ServiceTypeIdentifier:x2}. Data: " + BitConverter.ToString(serviceType.Payload));
        }
        #endregion

        #region packet and knx frame receive delegates
        private void PacketReceivedDelegate(DateTime timestamp, IPEndPoint source, IPEndPoint receiver, byte[] data)
        {
            OnDataReceived(timestamp, data);
        }

        private void KnxFrameReceivedDelegate(DateTime timestamp, IPEndPoint source, IPEndPoint receiver, IKnxNetIPFrame frame)
        {
            HandleFrameTypes(source, frame);
            OnFrameReceived(timestamp, frame, true);
        }

        private void HandleFrameTypes(IPEndPoint source, IKnxNetIPFrame frame)
        {
            switch (frame.FrameHeader.ServiceTypeIdentifier)
            {
                case ServiceTypeIdentifier.ConnectResponse:
                    HandleConnectResponse((ConnectionResponseService) frame.Service, source);
                    break;
                case ServiceTypeIdentifier.DisconnectRequest:
                    HandleDisconnectRequest((DisconnectRequestService) frame.Service);
                    break;
                case ServiceTypeIdentifier.DisconnectResponse:
                    HandleDisconnectResponse((DisconnectResponseService) frame.Service);
                    break;
                case ServiceTypeIdentifier.TunnelingRequest:
                    HandleTunnelingRequest((TunnelingRequestService) frame.Service);
                    break;
                case ServiceTypeIdentifier.TunnelingAcknowledge:
                    HandleTunnelingAck((TunnelingAcknowledgeService) frame.Service);
                    break;
                case ServiceTypeIdentifier.ConnectionStateResponse:
                    HandleConnectionStateResponse((ConnectionStateResponseService) frame.Service);
                    break;
                default:
                    HandleUnknownServiceType((RawService) frame.Service);
                    break;
            }
        }

        #endregion

        #region others

        /// <inheritdoc/>
        protected override string GetConnectionName()
        {
            return "TunnelingConnection - " + (_config.UseBusMonitorMode ? "BusMonitoring" : "Standard");
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _udpClient.Dispose();
                }
                disposedValue = true;
            }
        }
        #endregion others

        #endregion private implementations

        private enum AckState
        {
            Ok,
            Pending,
            Received,
            Error
        }
    }
}