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
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Common.Logging;
using Tiveria.Home.Knx.Cemi;

namespace Tiveria.Home.Knx.IP.Connections
{
    /// <summary>
    /// Base class for all ip connections.
    /// This clas provides basic shared functionalities and helper functions.
    /// </summary>
    public abstract class IPConnectionBase : IKnxNetIPConnection
    {
        #region public properties
        /// <summary>
        /// Type of the current connection
        /// </summary>
        public abstract ConnectionType ConnectionType { get; }

        /// <summary>
        /// IP Endpoint of the remote Knx device
        /// </summary>
        public IPEndPoint RemoteEndpoint { get; protected set; }

        /// <summary>
        /// Descriptive name of the connection
        /// </summary>
        public string ConnectionName => GetConnectionName();

        /// <summary>
        /// Channel id used between the two endpoints
        /// </summary>
        public byte ChannelId => _channelId;

        /// <summary>
        /// State of the connection
        /// </summary>
        public ConnectionState ConnectionState
        {
            get => _connectionState;
            set
            {
                var changed = _connectionState != value;
                _connectionState = value;
                if (changed)
                {
                    OnStateChanged(_connectionState);
                }
            }
        }
        #endregion

        #region public events
        /// <summary>
        /// Occurs when the state of the connection changes
        /// </summary>
        public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

        /// <summary>
        /// Occurs when a Knx frame was received and parsed
        /// </summary>
        public event EventHandler<FrameReceivedEventArgs>? FrameReceived;

        /// <summary>
        /// Occurs when raw data is received
        /// </summary>
        public event EventHandler<DataReceivedArgs>? DataReceived;

        /// <summary>
        /// Occurs when a connection is established
        /// </summary>
        public event EventHandler? Connected;

        /// <summary>
        /// Occurs when the connection is closed
        /// </summary>
        public event EventHandler? DisConnected;
        #endregion

        #region constructor
        protected IPConnectionBase(IPEndPoint remoteEndpoint)
        {
            //            _logger = Tiveria.Home.Knx.Utils.LogFactory.GetLogger("Tiveria.Home.Knx.IP." + ConnectionName);
            RemoteEndpoint = remoteEndpoint;
            ResetRcvSeqCounter();
            ResetSndSeqCounter();
        }

        ~IPConnectionBase()
        {
            // Dont change this code. Only change the Dispose(bool disposing) one.
            Dispose(disposing: false);
        }

        #endregion

        #region private fields
        //        protected readonly ILogger _logger;
        protected byte _channelId;
        private byte _rcvSeqCounter = 0;
        private byte _sndSeqCounter = 0;
        private object _seqLock = new object();
        private ConnectionState _connectionState = ConnectionState.Initialized;
        #endregion

        #region event handlers
        protected void OnStateChanged(ConnectionState state)
        {
            var args = new ConnectionStateChangedEventArgs(state);
            ConnectionStateChanged?.Invoke(this, args);
        }

        protected void OnFrameReceived(DateTime timestamp, IKnxNetIPFrame frame, bool handled)
        {
            FrameReceived?.Invoke(this, new FrameReceivedEventArgs(timestamp, frame, handled));
        }

        protected void OnDataReceived(DateTime timestamp, byte[] data)
        {
            DataReceived?.Invoke(this, new DataReceivedArgs(timestamp, data));
        }

        protected void OnConnected()
        {
            Connected?.Invoke(this, new EventArgs());
        }

        protected void OnDisConnected()
        {
            DisConnected?.Invoke(this, new EventArgs());
        }
        #endregion

        #region Sequence Counters
        protected void ResetRcvSeqCounter()
        {
            lock (_seqLock)
            {
                _rcvSeqCounter = 0;
            }
        }

        protected byte IncRcvSeqCounter()
        {
            byte result = 0;
            lock (_seqLock)
            {
                _rcvSeqCounter++;
                result = _rcvSeqCounter;
            }
            return result;
        }

        protected byte RcvSeqCounter => _rcvSeqCounter;

        protected void ResetSndSeqCounter()
        {
            lock (_seqLock)
            {
                _sndSeqCounter = 0;
            }
        }

        protected byte IncSndSeqCounter()
        {
            byte result = 0;
            lock (_seqLock)
            {
                _sndSeqCounter++;
                result = _sndSeqCounter;
            }
            return result;
        }

        protected byte SndSeqCounter => _sndSeqCounter;
        #endregion

        #region helper methods
        protected bool isCorrectChannelID(byte channelId, ServiceTypeIdentifier serviceType)
        {
            if (channelId == _channelId)
            {
                return true;
            }
            else
            {
//                _logger.Warn($"Wrong channelID {channelId} received for service {serviceType}. Expected: {_channelId}");
                return false;
            }
        }
        #endregion

        #region abstract base methods
        public abstract Task<bool> SendAsync(IKnxNetIPFrame frame);
        public abstract Task CloseAsync();

        public abstract Task<bool> ConnectAsync();

        protected abstract string GetConnectionName();

        public abstract Task DisconnectAsync();

        public abstract Task<bool> SendCemiAsync(ICemiMessage message);

        #region Disposable Pattern
        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #endregion
    }
}
