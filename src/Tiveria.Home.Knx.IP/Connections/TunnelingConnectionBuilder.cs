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
using System.Net;

namespace Tiveria.Home.Knx.IP.Connections
{
    /// <summary>
    /// Builder to simplify creating <see cref="TunnelingConnection"/> instances.
    /// </summary>
    public class TunnelingConnectionBuilder
    {
        #region private members
        private TunnelingConnectionConfiguration _config = new();
        private IPEndPoint _localEndPoint;
        private IPEndPoint _remoteEndPoint;
        private ILogger<TunnelingConnection>? _logger;
        #endregion

        #region public constructor
        /// <summary>
        /// Initializes the builder with the absolutely required parameters for a <see cref="TunnelingConnection"/>.
        /// </summary>
        /// <param name="localEndPoint">IP endpoint from which the host initiates the connection</param>
        /// <param name="remoteEndPoint">remote IP endpoint to connect to</param>
        public TunnelingConnectionBuilder(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint)
        {
            _localEndPoint = localEndPoint;
            _remoteEndPoint = remoteEndPoint;
        }

        /// <summary>
        /// Initializes the builder with the absolutely required parameters for a <see cref="TunnelingConnection"/>.
        /// </summary>
        /// <param name="localIp">IP Address of the local host</param>
        /// <param name="localPort">Port to be used to communicate from</param>
        /// <param name="remoteIp">IP Address of the remote KnxNetIp Interface/Router</param>
        /// <param name="remotePort">Port on the remote endpoint to communicate with <see cref="KnxNetIPConstants.DefaultPort"/></param>
        public TunnelingConnectionBuilder(IPAddress localIp, ushort localPort, IPAddress remoteIp, ushort remotePort = KnxNetIPConstants.DefaultPort)
        {
            _localEndPoint = new IPEndPoint(localIp, localPort);
            _remoteEndPoint = new IPEndPoint(remoteIp, remotePort);
        }

        /// <summary>
        /// Initializes the builder with the absolutely required parameters for a <see cref="TunnelingConnection"/>. LocalPort is set to 0 - so the UdpClient uses the next free port.
        /// </summary>
        /// <param name="localIp">IP Address of the local host</param>
        /// <param name="remoteIp">IP Address of the remote KnxNetIp Interface/Router</param>
        /// <param name="remotePort">Port on the remote endpoint to communicate with <see cref="KnxNetIPConstants.DefaultPort"/></param>
        public TunnelingConnectionBuilder(IPAddress localIp, IPAddress remoteIp, ushort remotePort = KnxNetIPConstants.DefaultPort)
            : this (localIp, 0 , remoteIp, remotePort)
        {}
        #endregion


        #region public methods
        /// <summary>
        /// Set the send timeout
        /// </summary>
        /// <param name="timeoutms">Timeout in ms</param>
        /// <returns>The builder</returns>
        public TunnelingConnectionBuilder WithSendTimeout(ushort timeoutms)
        {
            _config.SendTimeout = timeoutms;
            return this;
        }

        /// <summary>
        /// The maximum time the client waits for an acknowledge response
        /// </summary>
        /// <param name="timeoutms">Timeout in ms</param>
        /// <returns>The builder</returns>
        public TunnelingConnectionBuilder WithAcknowledgeTimeout(ushort timeoutms)
        {
            _config.AcknowledgeTimeout = timeoutms;
            return this;
        }

        /// <summary>
        /// The number of retries performed for a send operation.
        /// </summary>
        /// <param name="repeats">Repeats limitted to 10.</param>
        /// <returns>The builder</returns>
        public TunnelingConnectionBuilder WithSendRepeats(ushort repeats)
        {
            _config.SendRepeats = (ushort) (repeats > 10 ? 10 : repeats);
            return this;
        }

        /// <summary>
        /// Sets if the connection should act NAT aware
        /// </summary>
        /// <param name="active">Awareness on=true or off=false</param>
        /// <returns>The builder</returns>
        public TunnelingConnectionBuilder WithNatAwareness(bool active)
        {
            _config.NatAware = active;
            return this;
        }

        /// <summary>
        /// Switches the automatic resynchronisation of sequence numbers on or off
        /// </summary>
        /// <param name="active">Sync on=true or off=false</param>
        /// <returns>The builder</returns>
        public TunnelingConnectionBuilder WithResyncSequenceNumbers(bool active)
        {
            _config.ResyncSequenceNumbers = active;
            return this;
        }

        /// <summary>
        /// Configures the connection to use the BusMonitor mode of KnxNetIP
        /// </summary>
        /// <returns>The builder</returns>
        public TunnelingConnectionBuilder WithBusMonitorMode()
        {
            _config.UseBusMonitorMode = true;
            return this;
        }

        /// <summary>
        /// Configures the connection to use the normal mode instead of BusMonitor mode
        /// </summary>
        /// <returns>The builder</returns>
        public TunnelingConnectionBuilder WithNormalMode()
        {
            _config.UseBusMonitorMode = false;
            return this;
        }

        /// <summary>
        /// Configures the logger used. In case not set, internally a <see cref="NullLogger"/> is used.
        /// </summary>
        /// <param name="logger">the logger to use</param>
        /// <returns>The builder</returns>
        public TunnelingConnectionBuilder WithLogger(ILogger<TunnelingConnection> logger)
        {
            _logger = logger;
            return this;
        }

        /// <summary>
        /// Builds the TunnelingConnection instance based on the previously provided configurations
        /// </summary>
        /// <returns>the TunnelingConnection instance</returns>
        public TunnelingConnection Build()
        {
            return new TunnelingConnection(_localEndPoint, _remoteEndPoint, _config, _logger);
        }
        #endregion
    }
}
