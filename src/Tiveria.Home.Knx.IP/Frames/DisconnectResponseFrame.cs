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

using Tiveria.Common.IO;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.IP.Frames
{
    /// <summary>
    /// <code>
    /// +--------+--------+
    /// | byte 1 | byte 2 |
    /// +--------+--------+
    /// | Channel|Status  |
    /// | ID     | 0x00   |
    /// +--------+--------+
    /// </code>
    /// </summary>
    public class DisconnectResponseFrame : FrameBase, IKnxNetIPFrame
    {
        public static readonly byte STRUCTURE_SIZE = 2;
        public override ServiceTypeIdentifier ServiceTypeIdentifier => ServiceTypeIdentifier.DisconnectResponse;
        public ErrorCodes Status { get; init; }
        public byte ChannelId { get; init; }

        public DisconnectResponseFrame(byte channelId, ErrorCodes status = ErrorCodes.NoError)
            : this(new FrameHeader(ServiceTypeIdentifier.DisconnectResponse, STRUCTURE_SIZE), channelId, status)
        { }

        public DisconnectResponseFrame(FrameHeader frameHeader, byte channelId, ErrorCodes status = ErrorCodes.NoError)
            : base(frameHeader, ServiceTypeIdentifier.DisconnectResponse, STRUCTURE_SIZE)
        {
            ChannelId = channelId;
            Status = status;
        }
    }
}