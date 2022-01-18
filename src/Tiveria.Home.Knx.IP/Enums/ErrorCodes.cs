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

namespace Tiveria.Home.Knx.IP.Enums
{
    public enum ErrorCodes : byte
    {
        /// <summary>
        /// Operation successfull.
        /// </summary>
        NoError = 0x00,

        /// <summary>
        ///  The requested host protocol type is not supported.
        /// </summary>
        HostProtocolType = 0x01,

        /// <summary>
        /// The requested  host protocol version not supported.
        /// </summary>
        VersionNotSupported = 0x02,

        /// <summary>
        /// The sequence number is out of order.
        /// </summary>
        SequenceNumber = 0x04,

        /// <summary>
        /// The server could not find an active data connection with specified ID.
        /// </summary>
        ConnectionId = 0x21,

        /// <summary>
        /// The server does not support the requested connection type.
        /// </summary>
        ConnectionType = 0x22,

        /// <summary>
        /// The server does not support the requested connection options.
        /// </summary>
        ConnectionOption = 0x23,

        /// <summary>
        /// The server could not accept a new connection, maximum reached.
        /// </summary>
        NoMoreConnections = 0x24,

        /// <summary>
        /// The server detected an error concerning the data connection with the specified ID.
        /// </summary>
        DataConnection = 0x26,

        /// <summary>
        /// The server detected an error concerning the KNX subsystem connection with the specified ID.
        /// </summary>
        KnxConnection = 0x27,

        /// <summary>
        /// The server does not support the requested tunneling layer.
        /// </summary>
        TunnelingLayer = 0x29
    }
}
