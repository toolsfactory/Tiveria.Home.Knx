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

using System.Net;
using System.Net.Sockets;
using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.IP.Services;
using Tiveria.Common.Logging;

namespace Tiveria.Home.Knx.IP.Connections
{
    /// <summary>
    /// Delegate definition used by HeartbeatMonitor to inform owner about a failure
    /// </summary>
    /// <param name="severe"><c>true</c> if an exception caused the failure, otherwise <c>false</c>.</param>
    /// <param name="message">Text message describing the failure</param>
    internal delegate void HeartbeatFailedDelegate(bool severe, string message);
    internal delegate void HeartbeatOkDelegate();

    /// <summary>
    /// Internal class that helps monitoring the connection state of a KNXNetIP tunneling connection
    /// </summary>
    internal class HeartbeatMonitor
    {
        #region private fields
        private readonly int _intervalMS;
        private readonly int _timeoutMS;
        private readonly int _retries;
        private readonly UdpClient _udpClient;
        private readonly IPEndPoint _remoteEndpoint;
        private readonly byte[] _rawPacket;
        private readonly AutoResetEvent _ReceivedEvent = new AutoResetEvent(false);
        private readonly CancellationTokenSource _cancelSource = new CancellationTokenSource();
//        private readonly ILogger _logger = LogFactory.GetLogger("Tiveria.Home.Knx.IP.HeartbeatMonitor");
        #endregion

        #region public events
        public HeartbeatFailedDelegate HeartbeatFailed;
        private void OnHeartbeatFailed(bool severe, string message)
        {
            HeartbeatFailed?.Invoke(severe, message);
        }

        public HeartbeatOkDelegate HeartbeatOk;
        private void OnHeartbeatOk()
        {
            HeartbeatOk?.Invoke();
        }
        #endregion

        #region constructor
        public HeartbeatMonitor(IPEndPoint remoteEndpoint, Hpai endpointHpai, byte channelId, int intervalSeconds, int timeoutSeconds, int retries)
        {
            // As this is an internal class, no validation of parameters is performed
            _udpClient = new UdpClient();
            _remoteEndpoint = remoteEndpoint;
            _intervalMS = intervalSeconds * 1000;
            _timeoutMS = timeoutSeconds * 1000;
            _retries = retries;
            var service = new ConnectionStateRequestService(channelId, endpointHpai);
            _rawPacket = new KnxNetIPFrame(service).ToBytes();
        }
        #endregion

        public void Start()
        {
//            _logger.Trace("Starting heartbeats");
            Task.Run(() => InternalTask());
        }

        public void Stop()
        {
            _cancelSource.Cancel();
        }

        public void HandleResponse(ConnectionStateResponseService response)
        {
            if (response.Status == Enums.ErrorCodes.NoError)
            {
                #if DEBUG
//                _logger.Trace($"ConnectionStateResponse OK. ChannelId: {response.ChannelId}");
                #endif
                _ReceivedEvent.Set();
            }
            else
            {
                Console.WriteLine($"ConnectionStateResponse. ChannelId: {response.ChannelId}, Status: {response.Status}");
            }
        }

        private void InternalTask()
        {
            var receivedFlag = false;
            var cancelToken = _cancelSource.Token;
            try
            {
                while (true)
                {
                    receivedFlag = false;
                    if (cancelToken.WaitHandle.WaitOne(_intervalMS))
                        break;
                    for (var i = 0; i < _retries; i++)
                    {
                        SendHeartbeat();
                        int waitResult = WaitForHearbeat();
                        receivedFlag = waitResult == 0;
                        if (waitResult == WaitHandle.WaitTimeout)
                        { }
                            //_logger.Debug("Hartbeat not received in retry " + i);
                        else
                            break;
                    }
                    if (receivedFlag)
                    {
                        OnHeartbeatOk();
                    }
                    else
                    {
                        OnHeartbeatFailed(false, "No heartbeat response within timeout and retries");
                    }
                }
            }
            catch (SocketException ex)
            {
                OnHeartbeatFailed(true, "Communication error");
            }
            catch(Exception ex)
            {
                OnHeartbeatFailed(true, "Unknown error. " + ex);
            }
        }

        private int WaitForHearbeat()
        {
            #if DEBUG
//            _logger.Trace($"Waiting heartbeat");
            #endif
            var result =  WaitHandle.WaitAny(new WaitHandle[] { _ReceivedEvent, _cancelSource.Token.WaitHandle }, _timeoutMS);
            #if DEBUG
            //_logger.Trace($"Wait result: " + result);
            #endif
            return result;
        }

        private void SendHeartbeat()
        {
            #if DEBUG
//            _logger.Trace($"Sending heartbeat now");
            #endif
            _udpClient.Send(_rawPacket, _rawPacket.Length, _remoteEndpoint);
        }
    }
}
