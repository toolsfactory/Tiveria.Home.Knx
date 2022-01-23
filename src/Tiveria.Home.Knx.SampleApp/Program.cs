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

using Spectre.Console;
using System.Net;
using Tiveria.Home.Knx.IP;

namespace Tiveria.Home.Knx
{
    public class Program
    {
        public static IPAddress GatewayIPAddress = IPAddress.Parse("192.168.2.154");
        public static ushort GatewayPort = 3671;

        static void Main(string[] args)
        {
            bool exit = false;

            KnxNetIPFrameSerializerFactory.Instance.Initialize();
            while (!exit)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[underline green]  KnxNetIP Demo  [/]");
                Console.WriteLine();

                var menu = new EasyConsole.Menu()
                  .Add("Search KnxNetIP Routers/Tunnels", () => new SearchInterfacesDemo().RunAsync().Wait())
                  .Add("Build some structures", () => new StructureBuildDemo().RunAsync().Wait())
                  .Add("Tunneling Connection Tests", () => new TunnelingMonitor().RunAsync().Wait())
                  .Add("Routing Connection Tests", () => new RoutingMonitor().RunAsync().Wait())
                  .Add("DescriptionReq sample", () => new DescriptionRequestDemo().SendDescriptionRequest())
                  .Add("Busmonitor", () => new BusmonitorDemo().RunAsync().Wait())
                  .Add("DeviceManagement", () => new DeviceManagementDemo().RunAsync().Wait())
                  .Add("Exit", () => exit = true);
                menu.Display();
            }
        }
    }
}
