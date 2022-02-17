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
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Common.Logging;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.Exceptions;

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
        /// IP Endpoint of the remote Knx device
        /// </summary>
        public IPEndPoint RemoteEndpoint { get; protected set; }

        /// <summary>
        /// Descriptive name of the connection
        /// </summary>
        public string ConnectionName => GetConnectionName();

        /// <summary>
        /// State of the connection
        /// </summary>
        public KnxConnectionState ConnectionState
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

        /// <inheritdoc/>
        public event EventHandler<CemiReceivedArgs>? CemiReceived;
        #endregion

        #region constructor

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="remoteEndpoint">The remote <see cref="IPEndPoint"/> the connection talks to</param>
        protected IPConnectionBase(IPEndPoint remoteEndpoint)
        {
            RemoteEndpoint = remoteEndpoint;
        }

        /// <summary>
        /// tbd
        /// </summary>
        ~IPConnectionBase()
        {
            // Dont change this code. Only change the Dispose(bool disposing) one.
            Dispose(disposing: false);
        }

        #endregion

        #region public methods
        /// <summary>
        /// Establish a connection to the remote endpoint
        /// </summary>
        /// <exception cref="KnxCommunicationException">In case the connection could not be established, one of the <see cref="KnxCommunicationException"/> subtypes are fired</exception>
        public abstract Task ConnectAsync();

        /// <summary>
        /// Shutdown the current connection
        /// </summary>
        /// <returns></returns>
        public abstract Task DisconnectAsync();

        /// <summary>
        /// Send an <see cref="IKnxNetIPFrame"/> via the IP connection to the remote endpoint
        /// </summary>
        /// <param name="service">The KnxNetIP service to send</param>
        /// <exception cref="KnxCommunicationException">Thrown when sending the message failed</exception>
        public abstract Task SendServiceAsync(IKnxNetIPService service);

        /// <summary>
        /// Send a Cemi frame to the Knx infrastructure
        /// </summary>
        /// <param name="message">The cemi message to send</param>
        /// <exception cref="KnxCommunicationException">Thrown when sending the message failed</exception>
        public abstract Task SendCemiAsync(ICemiMessage message);
        #endregion

        #region private members
        private KnxConnectionState _connectionState = KnxConnectionState.Initialized;
        #endregion

        #region internal event handlers
        protected void OnStateChanged(KnxConnectionState state)
        {
            var args = new ConnectionStateChangedEventArgs(state);
            ConnectionStateChanged?.Invoke(this, args);
        }

        protected void OnFrameReceived(DateTime timestamp, IKnxNetIPFrame frame, bool handled)
        {
            FrameReceived?.Invoke(this, new FrameReceivedEventArgs(timestamp, frame, handled));
        }

        protected void OnCemiReceived(DateTime timestamp, ICemiMessage message)
        {
            CemiReceived?.Invoke(this, new CemiReceivedArgs(timestamp, message));
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

        #region internal implementations

        #region Disposable Pattern
        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        protected abstract string GetConnectionName();
        #endregion
    }
}
