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

using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.Primitives;

namespace Tiveria.Home.Knx.DeviceManagement
{
    public interface ITransportLayer
    {
        public int SeqNoSend { get; }
        public int SeqNoReceive { get; }

        /// <summary>
        /// Individual address of the remote knx device
        /// </summary>
        public IndividualAddress Address { get; }
        public TransportConnectionState State { get; }

        public int MaxApduFrameLength { get; }

        /// <summary>
        /// Establishes a connection to the specific device using a TPCI connect packet
        /// </summary>
        /// <returns>The awaitable task</returns>
        Task ConnectAsync();

        /// <summary>
        /// Disconnects from the device
        /// </summary>
        /// <returns>The awaitable task</returns>
        Task DisconnectAsync();

        Task<int> SendAsync(Apdu apdu, Priority priority = Priority.Normal);

        Task BroadcastAsync(Apdu apdu, Priority priority = Priority.Normal, Boolean system = false);

        public event EventHandler? Connected;
        public event EventHandler? Disconnected;
        public event EventHandler<CemiReceivedArgs> BroadcastReceived;
        public event EventHandler<CemiReceivedArgs> GroupReceived;
        public event EventHandler<CemiReceivedArgs> IndividualReceived;
    }
}