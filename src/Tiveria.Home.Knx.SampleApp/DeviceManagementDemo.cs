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
using System.Net.Sockets;
using Tiveria.Home.Knx.Primitives;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.IP.Services;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx
{
    public class DeviceManagementDemo
    {
        #region Private Fields

        private IP.Connections.TunnelingConnection? Con;

        #endregion Private Fields

        #region Public Methods

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

        public async Task RunAsync()
        {
            Con = new IP.Connections.TunnelingConnection(Program.GatewayIPAddress, Program.GatewayPort, GetLocalIPAddress(), 55555);
            await Con.ConnectAsync();

            var mngt = new IP.DeviceManagement.TunnelingManagementClient(Con, IndividualAddress.Parse("1.1.3"));
            await mngt.ConnectAsync();
            await mngt.ReadDeviceDescriptorAsync();
            await mngt.ReadPropertyAsync(5, 1);
            await mngt.ReadPropertyAsync(6, 1);
            await mngt.ReadPropertyAsync(0, 15);
            var mem = await mngt.ReadMemoryAsync(0x170, 12);
            Console.WriteLine("Memory read: {0}", mem != null ? BitConverter.ToString(mem) : "none");

            var data = await mngt.ReadPropertyAsync(0, 56);
            if (data != null)
                Console.WriteLine("zz: " + BitConverter.ToString(data));
            else
                Console.WriteLine("zz: Null returned");

            var propdesc = await mngt.ReadPropertyDescriptionAsync(0, 56);
            if (propdesc != null)
                Console.WriteLine($"Type: {propdesc.PropertyType}, Writeable: {propdesc.WriteEnabled}, WriteLevel: {propdesc.WriteLevel}, ReadLevel: {propdesc.ReadLevel}");
            else
                Console.WriteLine("**: Null returned");

            await Task.Delay(1000);
            await mngt.DisconnectAsync();
            await Con.DisconnectAsync();
            Console.ReadLine();
        }

        #endregion Public Methods
    }
}
