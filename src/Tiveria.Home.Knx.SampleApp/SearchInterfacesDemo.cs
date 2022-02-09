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

using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.IP.Services;
using Tiveria.Home.Knx.IP.Connections;
using Spectre.Console;

namespace Tiveria.Home.Knx
{
    public class SearchInterfacesDemo
    {
        public async Task RunAsync()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[underline red]  Search Interfaces Demo  [/]");
            Console.WriteLine();

            var finder = new KnxNetIPServerDiscoveryAgent();
            finder.ServerResponded += Finder_ServerResponded;
            await finder.DiscoverAsync();
            Console.WriteLine($"Interfaces found: {finder.Servers.Count}");
            await finder.DiscoverInProgrammingModeAsync();
            Console.WriteLine($"Interfaces programming: {finder.Servers.Count}");
            await finder.DiscoverByMacAsync(new byte[] {0xdc, 0xa6, 0x32, 0xb7, 0x47, 0x1a});
            Console.WriteLine($"Interfaces MAC: {finder.Servers.Count}");
            Task.Delay(1000).Wait();

            Console.WriteLine("Press <enter> to return.");
            Console.ReadLine();
        }

        private void Finder_ServerResponded(object? sender, ServerRespondedEventArgs e)
        {
            Console.WriteLine($"SearchResponse on {e.ReceivingEndpoint}: {e.Server.DeviceInfoDIB.FriendlyName} - {e.Server.ServiceEndpoint.Address} - {e.Server.DeviceInfoDIB.IndividualAddress} - {BitConverter.ToString(e.Server.DeviceInfoDIB.MAC)}");
            foreach(var family in e.Server.ServiceFamiliesDIB.ServiceFamilies)
            {
                Console.WriteLine($"    Family: {family.Family:x2} Version:{family.Version:x2}");
            }

        }
    }
}
