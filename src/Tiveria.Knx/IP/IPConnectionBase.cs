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
using System.Threading;
using System.Threading.Tasks;
using Tiveria.Knx.IP.Utils;
using Tiveria.Common.Logging;

namespace Tiveria.Knx.IP
{
    public abstract class IPConnectionBase : IIPConnection
    {
        protected readonly ILogger _logger;

        public abstract ConnectionType ConnectionType { get; }
        public abstract IPAddress RemoteAddress { get; }
        public string ConnectionName { get => GetConnectionName(); }

        protected abstract string GetConnectionName();

        private ConnectionState _connectionState = ConnectionState.Initialized;
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

        protected byte _channelId;
        public byte ChannelId { get => _channelId; }

        #region Events
        public event EventHandler<StateChangedEventArgs> StateChanged;
        protected void OnStateChanged(ConnectionState state)
        {
            var args = new StateChangedEventArgs(state);
            StateChanged?.Invoke(this, args);
        }

        public event EventHandler<FrameReceivedEventArgs> FrameReceived;
        protected void OnFrameReceived(DateTime timestamp, KnxNetIPFrame frame, bool handled)
        {
            FrameReceived?.Invoke(this, new FrameReceivedEventArgs(timestamp, frame, handled));
        }

        public event EventHandler<DataReceivedArgs> DataReceived;
        protected void OnDataReceived(DateTime timestamp, byte[] data)
        {
            DataReceived?.Invoke(this, new DataReceivedArgs(timestamp, data));
        }

        public event EventHandler Connected;
        protected void OnConnected()
        {
            Connected?.Invoke(this, new EventArgs());
        }

        public event EventHandler DisConnected;
        protected void OnDisConnected()
        {
            DisConnected?.Invoke(this, new EventArgs());
        }
        #endregion

        protected IPConnectionBase()
        {
            _logger = Tiveria.Knx.Utils.LogFactory.GetLogger("Tiveria.Knx.Connection." + ConnectionName);
            ResetRcvSeqCounter();
            ResetSndSeqCounter();
        }

        #region Sequence Counters
        private byte _rcvSeqCounter = 0;
        private byte _sndSeqCounter = 0;
        private object _seqLock = new object();

        protected void ResetRcvSeqCounter()
        {
            lock (_seqLock)
            {
                _rcvSeqCounter = 0;
            }
        }

        protected long IncRcvSeqCounter()
        {
            int result = 0;
            lock (_seqLock)
            {
                _rcvSeqCounter++;
                result = _rcvSeqCounter;
            }
            return result;
        }

        protected long RcvSeqCounter => _rcvSeqCounter; 

        protected void ResetSndSeqCounter()
        {
            lock (_seqLock)
            {
                _sndSeqCounter = 0;
            }
        }

        protected long IncSndSeqCounter()
        {
            long result = 0;
            lock (_seqLock)
            {
                _sndSeqCounter++;
                result = _sndSeqCounter;
            }
            return result;
        }

        protected long SndSeqCounter => _sndSeqCounter; 
        #endregion

        public abstract Task<bool> SendFrameAsync(KnxNetIPFrame frame);

        public abstract Task CloseAsync();

        public abstract Task<bool> ConnectAsync();
    }
}
