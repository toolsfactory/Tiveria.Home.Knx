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
using Tiveria.Home.Knx.Cemi;

namespace Tiveria.Home.Knx
{
    /// <summary>
    /// Provides data for the <see cref="IKnxConnection.ConnectionStateChanged"/>
    /// </summary>
    public class ConnectionStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Provides the new state of the connection
        /// </summary>
        public KnxConnectionState ConnectionState { get; init; }

        /// <summary>
        /// Creates a new instance of the <see cref="ConnectionStateChangedEventArgs"/> class
        /// </summary>
        /// <param name="state">The new state</param>
        public ConnectionStateChangedEventArgs(KnxConnectionState state)
        {
            ConnectionState = state;
        }
    }

    /// <summary>
    /// Provides access to data received via a knx bus connection
    /// </summary>
    public class DataReceivedArgs : EventArgs
    {
        /// <summary>
        /// The data received
        /// </summary>
        public byte[] Data { get; init; }
        /// <summary>
        /// Gets the date/time when the data was received
        /// </summary>
        public DateTime Timestamp { get; init; }
        /// <summary>
        /// Creates a new instance of the event data args class
        /// </summary>
        /// <param name="timestamp">The Date/Time when the data was received</param>
        /// <param name="data">THe actual data</param>
        public DataReceivedArgs(DateTime timestamp, byte[] data)
        {
            Timestamp = timestamp;
            Data = data;
        }
    }

    /// <summary>
    /// Provides access to parsed Cemi Data as soon as such a packet was received
    /// </summary>
    public class CemiReceivedArgs : EventArgs
    {
        /// <summary>
        /// The received data as parsed cemi message
        /// </summary>
        public ICemiMessage Message { get; init; }

        /// <summary>
        /// Gets the date/time when the data was received
        /// </summary>
        public DateTime Timestamp { get; init; }

        /// <summary>
        /// Creates a new instance of the event data args class
        /// </summary>
        /// <param name="timestamp">The Date/Time when the data was received</param>
        /// <param name="message">The actual cemi message</param>
        public CemiReceivedArgs(DateTime timestamp, ICemiMessage message)
        {
            Timestamp = timestamp;
            Message = message;
        }
    }
}
