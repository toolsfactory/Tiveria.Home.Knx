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
using System.Net.Sockets;
using Tiveria.Home.Knx.Adresses;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.IP.Services;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx
{
    public class DeviceManagementDemo
    {
        private IP.Connections.TunnelingConnection? Con;

        public async Task RunAsync()
        {
            Con = new IP.Connections.TunnelingConnection(Program.GatewayIPAddress, Program.GatewayPort, GetLocalIPAddress(), 55555);
            await Con.ConnectAsync();

            var tpdu = new Tpci(Cemi.PacketType.Control, Cemi.SequenceType.UnNumbered, 0, ControlType.Connect);
            var ctrl1 = new ControlField1(MessageCode.LDATA_REQ);
            var ctrl2 = new ControlField2();
            var cemi = new Cemi.CemiLData(Cemi.MessageCode.LDATA_REQ, new List<AdditionalInformationField>(), new IndividualAddress(0, 0, 0), GroupAddress.Parse("4/0/0"), ctrl1, ctrl2, tpdu);

            await Con.SendCemiAsync(cemi);
            await Task.Delay(1000);

            tpdu = new Tpci(Cemi.PacketType.Control, Cemi.SequenceType.UnNumbered, 0, ControlType.Disconnect);
            cemi = new Cemi.CemiLData(Cemi.MessageCode.LDATA_REQ, new List<AdditionalInformationField>(), new IndividualAddress(0, 0, 0), GroupAddress.Parse("4/0/0"), ctrl1, ctrl2, tpdu);
            await Con.SendCemiAsync(cemi);
        }

        public IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

    }
}
