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
using Spectre.Console;

namespace Tiveria.Home.Knx
{
    public class DeviceManagementDemo
    {
        readonly IndividualAddress ADDRESS = IndividualAddress.Parse("1.1.3");
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
            Console.Clear();
            AnsiConsole.MarkupLine("[underline green]  DeviceManagement Demo  [/]");
            Console.WriteLine();
            Console.WriteLine("Begin.");

            Console.WriteLine("Connecting to IP Interface.");
            var Con = new IP.Connections.TunnelingConnectionBuilder(Program.LocalIPAddress, Program.GatewayIPAddress, Program.GatewayPort).Build();
            await Con.ConnectAsync();

            Console.WriteLine("Connectionless Client Test");
            var client = new IP.Management.ConnectionlessDeviceManagementClient(Con);
            var data = await client.ReadPropertyAsync(ADDRESS, 0, 56);
            Console.WriteLine("* Prop 0-56: " + data != null ? BitConverter.ToString(data) : "Null returned");
            Console.WriteLine("Reading property description (APDULength).");
            var propdesc = await client.ReadPropertyDescriptionAsync(ADDRESS, 0, 56);
            if (propdesc != null)
                Console.WriteLine($"* Type: {propdesc.PropertyType}, Writeable: {propdesc.WriteEnabled}, WriteLevel: {propdesc.WriteLevel}, ReadLevel: {propdesc.ReadLevel}");
            else
                Console.WriteLine("* Null returned");



            Console.WriteLine("Connecting to device.");
            var mngt = new IP.Management.TunnelingManagementClient(Con, ADDRESS);
            await mngt.ConnectAsync();

            Console.WriteLine("Reading device descriptor.");
            var desc = await mngt.ReadDeviceDescriptorAsync();
            Console.WriteLine("* Device Descriptor read: {0}", desc != null ? BitConverter.ToString(desc) : "none");

            Console.WriteLine("Reading property description (APDULength).");
            propdesc = await mngt.ReadPropertyDescriptionAsync(0, 56);
            if (propdesc != null)
                Console.WriteLine($"* Type: {propdesc.PropertyType}, Writeable: {propdesc.WriteEnabled}, WriteLevel: {propdesc.WriteLevel}, ReadLevel: {propdesc.ReadLevel}");
            else
                Console.WriteLine("* Null returned");


            Console.WriteLine("Reading property 0-56 (APDULength).");
            data = await mngt.ReadPropertyAsync(0, 56);
            Console.WriteLine("* Prop 0-56: " + data != null ? BitConverter.ToString(data) : "Null returned");

            /*
            Console.WriteLine("Reading property 5-1.");
            data = await mngt.ReadPropertyAsync(5, 1);
            Console.WriteLine("* Prop 5-1: " + data != null ? BitConverter.ToString(data) : "Null returned");

            Console.WriteLine("Reading property 6-1.");
            data = await mngt.ReadPropertyAsync(6, 1);
            Console.WriteLine("* Prop 6-1: " + data != null ? BitConverter.ToString(data) : "Null returned");

            Console.WriteLine("Reading property 0-15.");
            data = await mngt.ReadPropertyAsync(0, 15);
            Console.WriteLine("* Prop 0-15: " + data != null ? BitConverter.ToString(data) : "Null returned");
            */

            Console.WriteLine("Reading Memory.");
            var mem = await mngt.ReadMemoryAsync(0x170, 12);
            Console.WriteLine("* Memory read: {0}", mem != null ? BitConverter.ToString(mem) : "none");

            await Task.Delay(1000);

            Console.WriteLine("Disconnecting.");
            await mngt.DisconnectAsync();
            await Con.DisconnectAsync();
            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        #endregion Public Methods
    }
}
