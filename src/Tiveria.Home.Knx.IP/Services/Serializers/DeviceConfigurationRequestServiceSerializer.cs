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

using Tiveria.Common.IO;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.Cemi.Serializers;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.IP.Services.Serializers
{
    public class DeviceConfigurationRequestServiceSerializer : ServiceSerializerBase<DeviceConfigurationRequestService>
    {
        public override ushort ServiceTypeIdentifier => Enums.ServiceTypeIdentifier.DeviceConfigurationRequest;

        public override DeviceConfigurationRequestService Deserialize(BigEndianBinaryReader reader)
        {
            var conheader = ConnectionHeader.Parse(reader);
            var cemi = new CemiLDataSerializer().Deserialize(reader, -1);
            return new DeviceConfigurationRequestService(conheader, cemi);
        }

        public override void Serialize(DeviceConfigurationRequestService service, BigEndianBinaryWriter writer)
        {
            service.ConnectionHeader.Write(writer);
            new CemiLDataSerializer().Serialize((CemiLData)service.CemiMessage, writer);
        }
    }
}