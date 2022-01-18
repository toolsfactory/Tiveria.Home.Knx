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

using Tiveria.Common.IO;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.IP.Frames.Serializers
{
    /// <summary>
    /// <code>
    /// Frame Header not shown here.
    /// +--------+--------+--------+--------+--------+--------+
    /// | byte 7 | byte 8 | byte 9 | byte 10| byte 11| byte 12|
    /// +--------+--------+--------+--------+--------+--------+
    /// | Channel|Status  | HPAI            | CRD Connection  |
    /// | Id     |        | Data Endpoint   | Response Data   |
    /// +-----------------+-----------------+-----------------+
    /// </code>
    /// </summary>
    public class ConnectionResponseFrameSerializer : FrameSerializerBase<ConnectionResponseFrame>
    {
        public override ServiceTypeIdentifier ServiceTypeIdentifier => ServiceTypeIdentifier.ConnectResponse;

        public override ConnectionResponseFrame Deserialize(BigEndianBinaryReader reader)
        {
            var header = FrameHeader.Parse(reader);
            var channelId = reader.ReadByte();
            var status = reader.ReadByteEnum<ErrorCodes>("ConnectionResponse.Status");
            var endpoint = Hpai.Parse(reader);
            var crd = CRDTunnel.Parse(reader);
            return new ConnectionResponseFrame(channelId, status, endpoint, crd);
        }

        public override void Serialize(ConnectionResponseFrame frame, BigEndianBinaryWriter writer)
        {
            frame.FrameHeader.Write(writer);
            writer.Write(frame.ChannelId);
            writer.Write((byte)frame.Status);
            frame.DataEndpoint.Write(writer);
            frame.Crd.Write(writer);
        }
    }

}