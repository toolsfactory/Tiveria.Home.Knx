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

namespace Tiveria.Knx.IP
{
    public class TunnelingConnection : IPConnectionBase, IIPConnection
    {
        public static bool ResyncOnSkippedRcvSeq = true;

        private readonly IPEndPoint _localEndpoint;
        private readonly IPEndPoint _remoteControlEndpoint;
        private readonly IPAddress _remoteAddress;
        private readonly UdpClient _udpClient;
        private IPEndPoint _remoteDataEndpoint;
        private UdpPacketReceiver _packetReceiver;

        public override ConnectionType ConnectionType { get => ConnectionType.TUNNEL_CONNECTION; }

        public override IPAddress RemoteAddress { get => _remoteAddress;}

        private readonly bool _natAware = false;
        public bool NatAware { get => _natAware; }

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

        public override Task CloseAsync()
        {
            return new Task(() =>
            {
                Stop();
            });
        }

        public void Stop()
        {
            if (_packetReceiver.Running)
            {
                _packetReceiver.Stop();
            }
        }

        #region sending connection request
        public override async Task<bool> ConnectAsync()
        {
            ConnectionState = ConnectionState.Opening;
            var data  = CreateConnectionRequestFrame();
            try
            {
                _packetReceiver.Start();
                var bytessent = await _udpClient.SendAsync(data, data.Length, _remoteControlEndpoint);
                if (bytessent == 0)
                {
                    _logger.Error("ConnectAsync: Zero bytes sent");
                    ConnectionState = ConnectionState.Invalid;
                    return false;
                }
                else
                    return true;
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
            return new KnxNetIPFrame(Utils.ServiceTypeIdentifier.CONNECT_REQUEST, req).ToBytes();
        }
        #endregion

        #region handling connection response
        /// <summary>
        /// handles frames that contain a ConnectRequest message
        /// </summary>
        /// <param name="frame">The full KNXNetIP Frame</param>
        /// <param name="remoteEndpoint">The remote endpoint that sent the frame</param>
        private void HandleConnectResponse(KnxNetIPFrame frame, IPEndPoint remoteEndpoint)
        {
            var response = (ConnectionResponse)frame.ServiceType;
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

        #region handling tunneling requests
        private void HandleTunnelingRequest(KnxNetIPFrame frame)
        {
            var request = (TunnelingRequest)frame.ServiceType;
            var seq = request.ConnectionHeader.SequenceCounter;
            if (!ValidateReqSequenceCounter(seq))
                return;

            var ack = new TunnelingAcknowledgement(_channelId, seq, ErrorCodes.NO_ERROR);
            var data = (new KnxNetIPFrame(ServiceTypeIdentifier.TUNNELING_ACK, ack)).ToBytes();
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

        #region packet and knx frame receive delegates
        private void PacketReceivedDelegate(DateTime timestamp, IPEndPoint source, byte[] data)
        {
            OnDataReceived(timestamp, data);
        }

        private void KnxFrameReceivedDelegate(DateTime timestamp, IPEndPoint source, KnxNetIPFrame frame)
        {
            if (frame.ServiceType.ServiceTypeIdentifier == ServiceTypeIdentifier.CONNECT_RESPONSE)
                HandleConnectResponse(frame, source);
            if (frame.ServiceType.ServiceTypeIdentifier == ServiceTypeIdentifier.TUNNELING_REQ)
                HandleTunnelingRequest(frame);
            
            OnFrameReceived(timestamp, frame, true);
        }


        #endregion

        protected override string GetConnectionName()
        {
            return "Tunnel";
        }
    }
}
