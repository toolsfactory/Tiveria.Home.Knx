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
    public delegate void HeartbeatFailedDelegate(bool severe, string message);

    /// <summary>
    /// Delegate definition used by HeartbeatMonitor to inform owner about a successful beat
    /// </summary>
    public delegate void HeartbeatOkDelegate();

    /// <summary>
    /// Internal class that helps monitoring the connection state of a KNXNetIP tunneling connection
    /// </summary>
    public class HeartbeatMonitor
    {
        #region public static behaviour configuration
        /// <summary>
        /// Defines how often the heartbeat signal is sent
        /// </summary>
        public static ushort IntervalSeconds { get; set; } = 10;

        /// <summary>
        /// Sets the maximum time the monitor waits for a response
        /// </summary>
        public static ushort TimeoutSeconds { get; set; } = 3;

        /// <summary>
        /// Defines how many retries are done before the monitor assumes a connection loss
        /// </summary>
        public static ushort Retries { get; set; } = 3;
        #endregion public static behaviour configuration

        #region public constructor

        /// <summary>
        /// Creates a new heartbeat monitor
        /// </summary>
        /// <param name="udpClient">The udp client to use for sending messages</param>
        /// <param name="endpointHpai">The knx endpoint information to be sent with the heartbeats</param>
        /// <param name="channelId">The knx channel id to be sent with the heartbeats </param>
        /// <param name="hmOkDel">Delegate </param>
        /// <param name="hbFailDel"></param>
        public HeartbeatMonitor(IUdpClient udpClient, Hpai endpointHpai, byte channelId, HeartbeatOkDelegate hmOkDel, HeartbeatFailedDelegate hbFailDel)
        {
            // As this is an internal class, no validation of parameters is performed
            _udpClient = udpClient;
            _heartbeatFailed = hbFailDel;
            _heartbeatOk = hmOkDel;
            var service = new ConnectionStateRequestService(channelId, endpointHpai);
            _rawPacket = new KnxNetIPFrame(service).ToBytes();
        }

        #endregion

        #region public implementation
        /// <summary>
        /// Start sending heartbeats
        /// </summary>
        public void Start()
        {
            Task.Run(() => InternalTask());
        }

        /// <summary>
        /// Stop sending heartbeats
        /// </summary>
        public void Stop()
        {
            _cancelSource.Cancel();
        }

        /// <summary>
        /// called from external when a connection state response was received
        /// </summary>
        /// <param name="response">The reponse details</param>
        public void HandleResponse(ConnectionStateResponseService response)
        {
            if (response.Status == Enums.ErrorCodes.NoError)
            {
                _ReceivedEvent.Set();
            }
            else
            {
                Console.WriteLine($"ConnectionStateResponse. ChannelId: {response.ChannelId}, Status: {response.Status}");
            }
        }
        #endregion public implementation

        #region private implementation

        #region private members
        private readonly IUdpClient _udpClient;
        private readonly byte[] _rawPacket;
        private readonly AutoResetEvent _ReceivedEvent = new AutoResetEvent(false);
        private readonly CancellationTokenSource _cancelSource = new CancellationTokenSource();
        private readonly HeartbeatFailedDelegate _heartbeatFailed;
        private readonly HeartbeatOkDelegate _heartbeatOk;
        #endregion private members

        #region private delegate handlers
        private void OnHeartbeatFailed(bool severe, string message)
        {
            _heartbeatFailed?.Invoke(severe, message);
        }

        private void OnHeartbeatOk()
        {
            _heartbeatOk?.Invoke();
        }
        #endregion private delegate handlers
        
        private void InternalTask()
        {
            var receivedFlag = false;
            var cancelToken = _cancelSource.Token;
            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    receivedFlag = false;
                    if (cancelToken.WaitHandle.WaitOne(IntervalSeconds * 1000))
                        break;
                    for (var i = 0; i < Retries; i++)
                    {
                        SendHeartbeat();
                        int waitResult = WaitForHearbeat();
                        receivedFlag = waitResult == 0;
                        if (waitResult == WaitHandle.WaitTimeout)
                        { }
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
            var result =  WaitHandle.WaitAny(new WaitHandle[] { _ReceivedEvent, _cancelSource.Token.WaitHandle }, TimeoutSeconds * 1000);
            return result;
        }

        private void SendHeartbeat()
        {
            _udpClient.SendAsync(_rawPacket);
        }

        #endregion private implementation

    }
}
