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
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.ObjectServer.Services.Serializers
{
    public class SetDatapointValueReqServiceSerializer : IObjectServerServiceSerializer<SetDatapointValueReqService>
    {
        public byte MainService => 0xF0;

        public byte SubService => 0x06;

        IObjectServerService IObjectServerServiceSerializer.Deserialize(BigEndianBinaryReader reader)
        {
            return DoDeserialize(reader);
        }

        public SetDatapointValueReqService Deserialize(BigEndianBinaryReader reader)
        {
            return DoDeserialize(reader);
        }

        private SetDatapointValueReqService DoDeserialize(BigEndianBinaryReader reader)
        {
            var mainSvc = reader.ReadByte();
            var subSvc = reader.ReadByte();
            if (mainSvc != MainService || subSvc != SubService)
                throw new KnxBufferFieldException("Main- or Subservice not correct");
            var startDP = reader.ReadUInt16();
            var numOfDP = reader.ReadUInt16();
            if (numOfDP == 0 && reader.Available > 0)
                KnxBufferSizeException.TooBig("SetDatapointReqService");
            if (numOfDP > 0 && reader.Available == 0)
                KnxBufferSizeException.TooSmall("SetDatapointReqService");

            var dplist = new List<DataPoint>();
            for (int i = 0; i < numOfDP; i++)
            {
                ushort dpid = reader.ReadUInt16();
                byte dpcmd = reader.ReadByte();
                byte dplen = reader.ReadByte();
                byte[] dpdata = reader.ReadBytes(dplen);
                dplist.Add(new DataPoint(dpid, dpcmd, dplen, dpdata));
            }
            return new SetDatapointValueReqService(startDP, dplist);
        }

        public void Serialize(SetDatapointValueReqService service, BigEndianBinaryWriter writer)
        {
            writer.Write(SetDatapointValueReqService.MainService);
            writer.Write(SetDatapointValueReqService.SubService);
            writer.Write(service.StartDataPoint);
            writer.Write((ushort) service.NumberOfDataPoints);
            foreach (var dp in service.DataPoints)
            {
                writer.Write(dp.ID);
                writer.Write(dp.Command);
                writer.Write(dp.Length);
                writer.Write(dp.Value);
            }
        }

        public void Serialize(IObjectServerService service, BigEndianBinaryWriter writer)
        {
            Serialize((SetDatapointValueReqService) service, writer);
        }
    }
}
