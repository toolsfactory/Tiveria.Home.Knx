﻿/*
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

using Tiveria.Common.IO;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.IP.Services.Serializers
{
    /// <summary>
    /// <code>
    /// Frame Header not shown here.
    /// +--------+--------+--------+--------+--------+--------+
    /// | byte 7 | byte 8 | byte 9 | byte 10| byte 11| byte 12|
    /// +--------+--------+--------+--------+--------+--------+
    /// | HPAI            | HPAI            | CRI Connection  |
    /// | Control Endpoint| Data Endpoint   | Request Info    |
    /// +-----------------+-----------------+-----------------+
    /// </code>
    /// </summary>
    public class ConnectionRequestServiceSerializer : ServiceSerializerBase<ConnectionRequestService>
    {
        public override ushort ServiceTypeIdentifier => Enums.ServiceTypeIdentifier.ConnectRequest;

        public override ConnectionRequestService Deserialize(BigEndianBinaryReader reader)
        {
            var dataEndpoint = Hpai.Parse(reader);
            var controlEndpoint = Hpai.Parse(reader);
            var cri = CRITunnel.Parse(reader);
            return new ConnectionRequestService(dataEndpoint, controlEndpoint, cri);
        }

        public override void Serialize(ConnectionRequestService service, BigEndianBinaryWriter writer)
        {
            service.DataEndpoint.Write(writer);
            service.ControlEndpoint.Write(writer);
            service.Cri.Write(writer);
        }
    }
}