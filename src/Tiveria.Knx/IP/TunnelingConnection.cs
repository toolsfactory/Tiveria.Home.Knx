/*
    Tiveria.Knx - a .Net Core base KNX library
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
using Tiveria.Common.Logging;
using Tiveria.Knx.IP.Utils;
using Tiveria.Knx.IP.ServiceTypes;
using Tiveria.Knx.IP.Structures;

namespace Tiveria.Knx.IP
{
    /// <summary>
    /// CLass allowing to connect to a KNX IP Router or Interface using Tunneling.
    /// </summary>
    public class TunnelingConnection : IPConnectionBase
    {
        public static bool ResyncOnSkippedRcvSeq = true;

        #region private fields
        private readonly object _lock = new object();
        private readonly ManualResetEvent _closeEvent = new ManualResetEvent(false);
        private readonly IPEndPoint _localEndpoint;
        private readonly IPEndPoint _remoteControlEndpoint;
        private readonly IPAddress _remoteAddress;
        private readonly UdpClient _udpClient;
        private readonly bool _natAware = false;
        private IPEndPoint _remoteDataEndpoint;
        private UdpPacketReceiver _packetReceiver;
        private HeartbeatMonitor _heartbeatMonitor;
        #endregion

        #region public properties
        public override ConnectionType ConnectionType => ConnectionType.TUNNEL_CONNECTION;
        public override IPAddress RemoteAddress => _remoteAddress;
        public bool NatAware => _natAware;
        #endregion
        
        #region constructors
        public TunnelingConnection(IPAddress remoteAddress, ushort remotePort, IPAddress localAddress, ushort localPort, bool natAware = false)
        {
            if (remoteAddress == null)
                throw new ArgumentNullException("RemoteAddress is null");
            if (localAddress == null)
                throw new ArgumentNullException("LocalAddress is null");

            _remoteAddress = remoteAddress;
            _remoteControlEndpoint = new IPEndPoint(remoteAddress, remotePort);
            _localEndpoint = new IPEndPoint(localAddress, localPort);
            _natAware = natAware;
            _udpClient = new UdpClient(_localEndpoint);
            _packetReceiver = new UdpPacketReceiver(_udpClient, PacketReceivedDelegate, KnxFrameReceivedDelegate);
        }
        #endregion

        public override async Task<bool> SendFrameAsync(KnxNetIPFrame frame)
        {
            var data = frame.ToBytes();
            try
            {
                var bytessent = await _udpClient.SendAsync(data, data.Length, _remoteControlEndpoint);
                if (bytessent == 0)
                {
                    _logger.Error("SendFrameAsync: Zero bytes sent");
                    return false;
                }
                else
                    return true;
            }
            catch (SocketException se)
            {
                _logger.Error("SendFrameAsync: SocketException raised", se);
                ConnectionState = ConnectionState.Invalid;
                return false;
            }
        }

        public Task<bool> SendCemiFrameAsync(Cemi.ICemi cemi)
        {
            var cemiLData = (Cemi.CemiLData)cemi;
            if (cemiLData == null)
                throw new ArgumentException("Only CemiLData allowed for now");
            var body = new TunnelingRequest(new Structures.ConnectionHeader(_channelId, SndSeqCounter), cemiLData);
            var frame = new KnxNetIPFrame(ServiceTypeIdentifier.TUNNELING_REQ, body.ToBytes());
            return SendFrameAsync(frame);
        }

        #region closing connection
        public override async Task CloseAsync()
        {
            await InternalCloseAsync("User triggered.", false);
        }

        private async Task InternalCloseAsync(string reason, bool external)
        {
            lock(_lock)
            {
                if (ConnectionState == ConnectionState.Closing || ConnectionState == ConnectionState.Closed)
                    return;
                ConnectionState = ConnectionState.Closing;
            }
            _logger.Info("Closing connection. " + reason);
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
            var hpai = new Structures.Hpai(Utils.HPAIEndpointType.IPV4_UDP, _localEndpoint.Address, (ushort)_localEndpoint.Port);
            var req = new DisconnectRequest(_channelId, hpai);
            return new KnxNetIPFrame(req).ToBytes();
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
            var data  = CreateConnectionRequestFrame();
            _logger.Trace("ConnectAsync: Sending connection request to " + _remoteControlEndpoint + " with data " + data.ToHexString());
            try
            {
                _packetReceiver.Start();
                var bytessent = await _udpClient.SendAsync(data, data.Length, _remoteControlEndpoint).ConfigureAwait(false);
                if (bytessent == 0)
                {
                    _logger.Error("ConnectAsync: Zero bytes sent");
                    ConnectionState = ConnectionState.Invalid;
                    return false;
                }
                else
                {
                    _logger.Trace("ConnectAsync: Connection request sucessfully sent");
                    return true;
                }
            }
            catch (SocketException se)
            {
                _logger.Error("ConnectAsync: SocketException raised", se);
                ConnectionState = ConnectionState.Invalid;
                return false;
            }
        }

        private byte[] CreateConnectionRequestFrame()
        {
            var cri = new Structures.CRITunnel(Utils.TunnelingLayer.TUNNEL_LINKLAYER);
            var hpai = new Structures.Hpai(Utils.HPAIEndpointType.IPV4_UDP, _localEndpoint.Address, (ushort)_localEndpoint.Port);
            var req = new ServiceTypes.ConnectionRequest(cri, hpai, hpai);
            return new KnxNetIPFrame(req).ToBytes();
        }
        #endregion

        #region handling connection response
        /// <summary>
        /// handles frames that contain a ConnectRequest message
        /// </summary>
        /// <param name="frame">The full KNXNetIP Frame</param>
        /// <param name="remoteEndpoint">The remote endpoint that sent the frame</param>
        private void HandleConnectResponse(ConnectionResponse response, IPEndPoint remoteEndpoint)
        {
            VerifyConnectionResponse(response, remoteEndpoint);
        }

        private bool VerifyConnectionResponse(ConnectionResponse connectionResponse, IPEndPoint remoteEndpoint)
        {
            if (connectionResponse.Status == ErrorCodes.NO_ERROR)
            {
                // to be sure, chack again that HPAI is for UDP in IPv4
                if (connectionResponse.EndpointHPAI.EndpointType == HPAIEndpointType.IPV4_UDP)
                {
                    //Everything ok. Lets fill all fields
                    var ep = connectionResponse.EndpointHPAI;
                    VerifyRemoteDataEndpointforNAT(ep, remoteEndpoint);
                    _channelId = connectionResponse.ChannelId;
                    ConnectionState = ConnectionState.Open;
                    SetupHeartbeatMonitor();
                    return true;
                }
                else
                {
                    // Endpoint type is not IPv4/UDP
                    _logger.Error("Server requested endpoint type that is not IPv4/UDP!");
                    return false;
                }
            }
            else
            {
                _logger.Error(String.Format("Server sent statuscode {0}: '{1}'", connectionResponse.Status, connectionResponse.Status.ToDescription()));
                return false;
            }
        }

        private void SetupHeartbeatMonitor()
        {
            var hpai = new Structures.Hpai(Utils.HPAIEndpointType.IPV4_UDP, _localEndpoint.Address, (ushort)_localEndpoint.Port);
            _heartbeatMonitor = new HeartbeatMonitor(_remoteControlEndpoint, hpai, _channelId, 5, 5, 2);
            _heartbeatMonitor.HeartbeatFailed = HeartbeatFailed;
            _heartbeatMonitor.HeartbeatOk = HeartbeatOk;
            _heartbeatMonitor.Start();
        }

        private void HeartbeatFailed(bool severe, string message)
        {
            if(severe)
            {
                _logger.Error("Heartbeat failed. " + message);
            }
            else
            {
                _logger.Info("Heartbeat failed. " + message);
            }
            CloseAsync();
        }

        private void HeartbeatOk()
        {
            Console.Out.WriteLineAsync("Heartbeat OK");
        }
        
        private void VerifyRemoteDataEndpointforNAT(Structures.Hpai remoteHPAI, IPEndPoint remoteEndpoint)
        {
            if (_natAware && (remoteHPAI.Ip == IPAddress.Any || remoteHPAI.Port == 0))
            {
                _remoteDataEndpoint = remoteEndpoint;
                _logger.Debug("ConnectionResponse: NAT mode, using socket endpoint " + _remoteDataEndpoint);
            }
            else
            {
                _remoteDataEndpoint = new IPEndPoint(remoteHPAI.Ip, remoteHPAI.Port);
                _logger.Debug("ConnectionResponse: using protocol endpoint " + _remoteDataEndpoint);
            }
        }
        #endregion

        #region handling disconnect request
        private void HandleDisconnectRequest(DisconnectRequest request)
        {
            SendDisconnectResponseAsync(request.ChannelId, request.ControlEndpoint);
            InternalCloseAsync("External request received.", true);
        }

        private Task SendDisconnectResponseAsync(byte channelId, Hpai controlEndpoint)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region handling disconnect response
        private void HandleDisconnectResponse(DisconnectResponse response)
        {
            if (response.Status != ErrorCodes.NO_ERROR)
                _logger.Warn($"Connection closed with status code 0x{response.Status:x2} - " + response.Status.ToDescription());
            _closeEvent.Set();
        }
        #endregion

        #region handling tunneling requests
        private void HandleTunnelingRequest(TunnelingRequest request)
        {
            var seq = request.ConnectionHeader.SequenceCounter;
            if (!ValidateReqSequenceCounter(seq))
                return;

            var ack = new TunnelingAcknowledgement(_channelId, seq, ErrorCodes.NO_ERROR);
            var data = (new KnxNetIPFrame(ack)).ToBytes();
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
            if (missed && ResyncOnSkippedRcvSeq)
            {
                _logger.Error($"tunneling request with rcv-seq '{rcvSeq}', expected '{expSeq}' -> re-sync with server (1 tunneled msg lost)");
                IncRcvSeqCounter();
                expSeq++;
            }
            IncRcvSeqCounter();
            return (rcvSeq == expSeq) || (rcvSeq == ++expSeq);
        }
        #endregion

        #region handling ConnectionState response
        private void HandleConnectionStateResponse(ConnectionStateResponse response)
        {
            if(_heartbeatMonitor != null)
            _heartbeatMonitor.HandleResponse(response);
        }
        #endregion

        #region handling unknown servicetype
        private void HandleUnknownServiceType(UnknownService serviceType)
        {
            _logger.Warn($"Unknown Servicetype: {serviceType.ServiceTypeRaw:x2}. Data: " + serviceType.FrameRaw.ToHexString());
        }
        #endregion

        #region packet and knx frame receive delegates
        private void PacketReceivedDelegate(DateTime timestamp, IPEndPoint source, byte[] data)
        {
            OnDataReceived(timestamp, data);
        }

        private void KnxFrameReceivedDelegate(DateTime timestamp, IPEndPoint source, KnxNetIPFrame frame)
        {
            #if DEBUG
            _logger.Trace($"Frame received. Type: {frame.ServiceType}. Data: " + frame.Body.ToHexString());
            #endif
            switch (frame.ServiceType.ServiceTypeIdentifier)
            {
                case ServiceTypeIdentifier.CONNECT_RESPONSE:
                    HandleConnectResponse((ConnectionResponse)frame.ServiceType, source);
                    break;
                case ServiceTypeIdentifier.DISCONNECT_REQ:
                    HandleDisconnectRequest((DisconnectRequest)frame.ServiceType);
                    break;
                case ServiceTypeIdentifier.DISCONNECT_RES:
                    HandleDisconnectResponse((DisconnectResponse)frame.ServiceType);
                    break;
                case ServiceTypeIdentifier.TUNNELING_REQ:
                    HandleTunnelingRequest((TunnelingRequest)frame.ServiceType);
                    break;
                case ServiceTypeIdentifier.CONNECTIONSTATE_RESPONSE:
                    HandleConnectionStateResponse((ConnectionStateResponse)frame.ServiceType);
                    break;
                case ServiceTypeIdentifier.UNKNOWN:
                    HandleUnknownServiceType((UnknownService)frame.ServiceType);
                    break;
            }
            OnFrameReceived(timestamp, frame, true);
        }
        #endregion

        protected override string GetConnectionName()
        {
            return "TunnelingConnection";
        }
    }
}
