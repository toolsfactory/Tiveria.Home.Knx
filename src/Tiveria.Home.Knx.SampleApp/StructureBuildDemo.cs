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

using System.Net;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.IP.Frames;
using Tiveria.Home.Knx.IP;

namespace Tiveria.Home.Knx
{
    public class StructureBuildDemo
    {
        public async Task RunAsync()
        {
            await Task.Run(() =>
            {
                bool on = true;
                var input = (byte)(on ? 0x01 : 0x00);
                var apci = new EMI.Apci(EMI.ApciTypes.GroupValue_Write, new byte[] { input });
                Console.WriteLine("APCI: " + BitConverter.ToString(apci.ToBytes()));
                var cemi = new EMI.CemiLData(EMI.CemiMessageCode.LDATA_REQ, new IndividualAddress(0, 0, 0), GroupAddress.Parse("4/0/0"), apci.ToBytes(), EMI.Priority.System);
                Console.WriteLine("CEMI: " + BitConverter.ToString(cemi.ToBytes()));
                var con = new IP.Structures.ConnectionHeader(11, 22);
                Console.WriteLine("HEAD: " + BitConverter.ToString(con.ToBytes()));
                var frame = new TunnelingRequestFrame(con, cemi);
                var data = KnxNetIPFrameSerializerFactory.
                    Instance.Create(frame.ServiceTypeIdentifier).Serialize(frame);
                Console.WriteLine("Frame:" + BitConverter.ToString(data));

                var cf = new ConnectionRequestFrame(
                    new Hpai(HPAIEndpointType.IPV4_UDP, IPAddress.Parse("192.168.2.1"), 12233),
                    new Hpai(HPAIEndpointType.IPV4_UDP, IPAddress.Parse("192.168.2.1"), 12233),
                    new CRITunnel(TunnelingLayer.TUNNEL_LINKLAYER));
                Console.ReadLine();
            });
        }
    }
}
