/*
    Tiveria.Home.Knx - a .Net Core base KNX library
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

using Tiveria.Home.Knx.Primitives;
using Tiveria.Home.Knx.Cemi;

namespace Tiveria.Home.Knx
{
    /// <summary>
    /// Baseline interface for all clients used to connect to the Knx bus
    /// </summary>
    public interface IKnxClient : IDisposable
    {
        /// <summary>
        /// Shows if the client is currently connected to the Knx bus
        /// </summary>
        bool IsConnected  => ConnectionState == KnxConnectionState.Open;

        /// <summary>
        /// The detailed connection status
        /// </summary>
        KnxConnectionState ConnectionState { get; }

        /// <summary>
        /// Human readable name of the connection
        /// </summary>
        String ConnectionName { get; }

        /// <summary>
        /// Occurs when the <see cref="ConnectionState"/> changes
        /// </summary>
        event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

        /// <summary>
        /// Occurs when data is received from the Knx bus
        /// </summary>
        event EventHandler<DataReceivedArgs>? DataReceived;

        /// <summary>
        /// Triggered when a connection to the Knx bus was established
        /// </summary>
        event EventHandler? Connected;

        /// <summary>
        /// Triggered when the connection to the Knx bus is closed
        /// </summary>
        event EventHandler? DisConnected;

        /// <summary>
        /// Asynchronously connects to the Knx bus.
        /// </summary>
        /// <returns>True if sucessfully connected, otherwise false</returns>
        Task<bool> ConnectAsync();

        /// <summary>
        /// Closes the connection to the Knx bus.
        /// </summary>
        /// <returns>The awaitable task</returns>
        Task DisconnectAsync();

        /// <summary>
        /// Sends a Cemi message to the Knx bus
        /// </summary>
        /// <param name="message">The message</param>
        /// <returns>The awaitable task</returns>
        Task SendCemiAsync(ICemiMessage message);
    }
}
