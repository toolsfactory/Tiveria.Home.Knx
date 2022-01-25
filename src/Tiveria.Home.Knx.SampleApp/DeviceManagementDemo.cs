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

            var mngt = new DeviceManagement.ManagementClient(Con);
            await Task.Delay(1000);
            await mngt.ConnectAsync();    
            await Task.Delay(1000);
            await mngt.ReadPropertyAsync(0, 56);
            await Task.Delay(1000);
            await mngt.DisconnectAsync();
            await Con.DisconnectAsync();
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
