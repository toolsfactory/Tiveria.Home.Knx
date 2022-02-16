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

namespace Tiveria.Home.Knx.IP.Connections
{
    /// <summary>
    /// Configuration options for all kinds of KnxNetIP connections
    /// </summary>
    public record IPConnectionConfiguration
    {
        /// <summary>
        /// Enable/Disable automatic resynchronization of sequence number
        /// </summary>
        public bool ResyncSequenceNumbers { get; set; } = false;

        /// <summary>
        /// Switch to control the NAT awarenes of the connection initiation
        /// </summary>
        public bool NatAware { get; set; } = false;

        /// <summary>
        /// Defines how often a failed send should be repeated
        /// </summary>
        public ushort SendRepeats { get; set; } = 3;

        /// <summary>
        /// Timeout in milliseconds after the achnowledge is deemed to not happen anymore
        /// </summary>
        public ushort AcknowledgeTimeout { get; set; } = 500;

        /// <summary>
        /// Maximum time in milliseconds a send can take before it is considered as failed
        /// </summary>
        public ushort SendTimeout { get; set; } = 500;
    }
}
