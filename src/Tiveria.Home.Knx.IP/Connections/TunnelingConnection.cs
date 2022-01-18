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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Net.Sockets;
using Tiveria.Common.Extensions;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Frames;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.IP.Connections
{
    /// <summary>
    /// CLass allowing to connect to a KNX IP Router or Interface using Tunneling.
    /// </summary>
    public class TunnelingConnection : IPConnectionBase
    {
        #region private fields
        private readonly object _lock = new object();
        private readonly ManualResetEvent _closeEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent _ackEvent = new ManualResetEvent(false);
        private readonly IPEndPoint _localEndpoint;
        private readonly IPEndPoint _remoteControlEndpoint;
        private readonly ILogger<TunnelingConnection> _logger;
        private readonly UdpClient _udpClient;
        private IPEndPoint? _remoteDataEndpoint;
        private UdpPacketReceiver _packetReceiver;
        private HeartbeatMonitor? _heartbeatMonitor;
        private AckState _ackState = AckState.Ok;
        private readonly TunnelingConnectionConfiguration _config;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool disposedValue;
        #endregion

        #region public properties
        /// <summary>
        /// 
        /// </summary>
        public override ConnectionType ConnectionType => ConnectionType.Tunnel;
        #endregion

        #region constructors
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

            if (configuration != null)
                _config = configuration;
            else 
                _config = new TunnelingConnectionConfiguration();

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
            : this (new IPEndPoint(localAddress, localPort), new IPEndPoint(remoteAddress, remotePort), new TunnelingConnectionConfiguration() { UseBusMonitorMode = busmonitor, NatAware = natAware })
        {
            // ToDo: remove deprecated constructor
        }
        #endregion

        /// <summary>
        /// Send a generic KnxNetIPFrame (potentially raw payload)
        /// </summary>
        /// <param name="frame">The frame to send</param>
        /// <returns>true if the frame was sent successfully, otherise false</returns>
        public override async Task<bool> SendAsync(IKnxNetIPFrame frame)
        {
            if (ConnectionState != ConnectionState.Open)
                return false;

            var serializer = KnxNetIPFrameSerializerFactory.Instance.Create(frame.ServiceTypeIdentifier);
            var data = serializer.Serialize(frame);
            var timeoutCTS = new CancellationTokenSource(_config.SendTimeout);
            var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(timeoutCTS.Token, _cancellationTokenSource.Token);
            try
            {
                var bytessent = await _udpClient.SendAsync(data, data.Length).WaitAsync(linkedCTS.Token);
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

        /// <summary>
        /// Send a Cemi message to the KnxNetIP server in blocking mode <see cref="SendCemiAsync(Cemi.ICemiMessage cemi, bool blocking = true)"/>. Version & Connection headers are automatically generated.
        /// </summary>
        /// <param name="cemi">The cemi message to send</param>
        /// <returns>true if the message was sent sucessfuly, otherwise false</returns>
        public override Task<bool> SendCemiAsync(ICemiMessage message)
        {
            return SendCemiAsync(message, true);
        }

        /// <summary>
        /// Send a Cemi message to the KnxNetIP server. Version & Connection headers are automatically generated.
        /// </summary>
        /// <param name="cemi">The cemi message to send</param>
        /// <param name="blocking">If yes, the message is only sent if no other message is currently being processed. On top, the call only returns then after the Ack was received or timed out.</param>
        /// <returns>true if the message was sent sucessfuly, otherwise false</returns>
        public async Task<bool> SendCemiAsync(Cemi.ICemiMessage cemi, bool blocking = true)
        {
            var conheader = new ConnectionHeader(_channelId, SndSeqCounter);
            var frame = new TunnelingRequestFrame(conheader, cemi);
            if (blocking)
                return await DoSendCemiBlockingAsync(frame);
            else
                return await SendAsync(frame);
        }

        private Task<bool> DoSendCemiBlockingAsync(IKnxNetIPFrame frame)
        {
            var serializer = KnxNetIPFrameSerializerFactory.Instance.Create(frame.ServiceTypeIdentifier);
            var data = serializer.Serialize(frame);

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
                            ackReceived = _ackEvent.WaitOne(_config.AcknowledgeTimeout);
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

        private TunnelingRequestFrame CreateTunnelingRequestFrameFromCemi(Cemi.CemiLData cemi)
        {
            var frame = new TunnelingRequestFrame(new Structures.ConnectionHeader(_channelId, SndSeqCounter), cemi);
            return frame;
        }



        #region closing connection
        public override async Task CloseAsync()
        {
            await InternalCloseAsync("User triggered.", false);
        }

        private async Task InternalCloseAsync(string reason, bool external)
        {
            lock (_lock)
            {
                if (ConnectionState == ConnectionState.Closing || ConnectionState == ConnectionState.Closed)
                    return;
                ConnectionState = ConnectionState.Closing;
            }
            //_logger.Info("Closing connection. " + reason);
            _heartbeatMonitor?.Stop();
            if (!external)
            {
                await SendDisconnectRequestAsync().ConfigureAwait(false);
                _closeEvent.WaitOne(1000);
            }
            await Task.Run(() => Stop()).ConfigureAwait(false);
            ConnectionState = ConnectionState.Closed;
        }

        private void Stop()
        {
            if (_packetReceiver.Running)
            {
                _packetReceiver.Stop();
            }
        }

        private byte[] CreateDisconnectFrame()
        {
            var hpai = new Structures.Hpai(Enums.HPAIEndpointType.IPV4_UDP, _localEndpoint.Address, (ushort)_localEndpoint.Port);
            var frame = new DisconnectRequestFrame(_channelId, hpai);
            var data = KnxNetIPFrameSerializerFactory.
                        Instance.Create(frame.ServiceTypeIdentifier).Serialize(frame);
            return data;
        }

        private Task SendDisconnectRequestAsync()
        {
            return Task.Run(() =>
            {
                lock (_lock)
                {
                    if (ConnectionState == ConnectionState.Closed)
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
        public override async Task<bool> ConnectAsync()
        {
            ConnectionState = ConnectionState.Opening;
            var data = CreateConnectionRequestFrame();
            //_logger.Trace("ConnectAsync: Sending connection request to " + _remoteControlEndpoint + " with data " + data.ToHex());
            try
            {
                _packetReceiver.Start();
                var bytessent = await _udpClient.SendAsync(data, data.Length, _remoteControlEndpoint).ConfigureAwait(false);
                if (bytessent == 0)
                {
                    //_logger.Error("ConnectAsync: Zero bytes sent");
                    ConnectionState = ConnectionState.Invalid;
                    return false;
                }
                else
                {
                    //_logger.Trace("ConnectAsync: Connection request sucessfully sent");
                    return true;
                }
            }
            catch (SocketException se)
            {
                //_logger.Error("ConnectAsync: SocketException raised", se);
                ConnectionState = ConnectionState.Invalid;
                return false;
            }
        }

        private byte[] CreateConnectionRequestFrame()
        {
            var cri = new CRITunnel(_config.UseBusMonitorMode ? TunnelingLayer.TUNNEL_BUSMONITOR : TunnelingLayer.TUNNEL_LINKLAYER);
            var hpai = new Structures.Hpai(Enums.HPAIEndpointType.IPV4_UDP, _localEndpoint.Address, (ushort)_localEndpoint.Port);
            var frame = new ConnectionRequestFrame(hpai, hpai, cri);
            var data  = KnxNetIPFrameSerializerFactory.
                Instance.Create(frame.ServiceTypeIdentifier).Serialize(frame);
            return data;
        }
        #endregion

        #region handling connection response
        /// <summary>
        /// handles frames that contain a ConnectRequest message
        /// </summary>
        /// <param name="frame">The full KNXNetIP Frame</param>
        /// <param name="remoteEndpoint">The remote endpoint that sent the frame</param>
        private void HandleConnectResponse(ConnectionResponseFrame response, IPEndPoint remoteEndpoint)
        {
            VerifyConnectionResponse(response, remoteEndpoint);
        }

        private bool VerifyConnectionResponse(ConnectionResponseFrame connectionResponse, IPEndPoint remoteEndpoint)
        {
            if (connectionResponse.Status == ErrorCodes.NoError)
            {
                // to be sure, chack again that HPAI is for UDP in IPv4
                if (connectionResponse.DataEndpoint.EndpointType == HPAIEndpointType.IPV4_UDP)
                {
                    //Everything ok. Lets fill all fields
                    var ep = connectionResponse.DataEndpoint;
                    VerifyRemoteDataEndpointforNAT(ep, remoteEndpoint);
                    _channelId = connectionResponse.ChannelId;
                    ConnectionState = ConnectionState.Open;
                    SetupHeartbeatMonitor();
                    return true;
                }
                else
                {
                    // Endpoint type is not IPv4/UDP
                    //_logger.Error("Server requested endpoint type that is not IPv4/UDP!");
                    return false;
                }
            }
            else
            {
                //_logger.Error(String.Format("Server sent statuscode {0}: '{1}'", connectionResponse.Status, connectionResponse.Status.ToDescription()));
                return false;
            }
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
            CloseAsync();
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
        private void HandleDisconnectRequest(DisconnectRequestFrame request)
        {
            var frame = new DisconnectResponseFrame(request.ChannelId);
            var data = KnxNetIPFrameSerializerFactory.
                Instance.Create(frame.ServiceTypeIdentifier).Serialize(frame);
            _udpClient.Send(data, data.Length, _remoteDataEndpoint);

            InternalCloseAsync("External request received.", true);
        }

        #endregion

        #region handling disconnect response
        private void HandleDisconnectResponse(DisconnectResponseFrame response)
        {
            if (response.Status != ErrorCodes.NoError)
            { }
                //_logger.Warn($"Connection closed with status code " + response.Status.ToDescription());
            _closeEvent.Set();
        }
        #endregion

        #region handling tunneling requests
        private void HandleTunnelingRequest(TunnelingRequestFrame request)
        {
            var seq = request.ConnectionHeader.SequenceCounter;
            if (!ValidateReqSequenceCounter(seq))
                return;

            var frame = new TunnelingAcknowledgeFrame(request.ConnectionHeader);
            var data = KnxNetIPFrameSerializerFactory.
                Instance.Create(frame.ServiceTypeIdentifier).Serialize(frame);
            _udpClient.Send(data, data.Length, _remoteDataEndpoint);
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

        private void HandleTunnelingAck(TunnelingAcknowledgeFrame serviceType)
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
        private void HandleConnectionStateResponse(ConnectionStateResponseFrame response)
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
                    HandleConnectResponse((ConnectionResponseFrame)frame, source);
                    break;
                case ServiceTypeIdentifier.DisconnectRequest:
                    HandleDisconnectRequest((DisconnectRequestFrame)frame);
                    break;
                case ServiceTypeIdentifier.DisconnectResponse:
                    HandleDisconnectResponse((DisconnectResponseFrame)frame);
                    break;
                case ServiceTypeIdentifier.TunnelingRequest:
                    HandleTunnelingRequest((TunnelingRequestFrame)frame);
                    break;
                case ServiceTypeIdentifier.TunnelingAcknowledge:
                    HandleTunnelingAck((TunnelingAcknowledgeFrame)frame);
                    break;
                case ServiceTypeIdentifier.ConnectionStateResponse:
                    HandleConnectionStateResponse((ConnectionStateResponseFrame)frame);
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

        public override Task DisconnectAsync()
        {
            return Task.Run(() =>
            {
                _cancellationTokenSource.Cancel();
                _udpClient.Close();
            });
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

        private enum AckState
        {
            Ok,
            Pending,
            Received,
            Error
        }
    }
}