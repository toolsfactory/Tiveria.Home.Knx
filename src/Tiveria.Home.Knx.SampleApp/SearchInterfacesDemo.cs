﻿/*
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

using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.IP.Frames;
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

            var finder = new KnxIPInterfaceFinder();
            finder.DataReceived += Finder_DataReceived;
            finder.StartListening();
            finder.BroadcastSearchRequest();
            Thread.Sleep(1000);
            Console.WriteLine($"Interfaces found: {finder.Interfaces.Count}");
            var result = await finder.SearchForInterfacesAsync();
            Console.WriteLine(result);
            Task.Delay(1000).Wait();

            Console.WriteLine("Press <enter> to return.");
            Console.ReadLine();
        }

        private void Finder_DataReceived(object? sender, DataReceivedArgs e)
        {
            var parser = KnxNetIPFrameSerializerFactory.Instance.Create<SearchResponseFrame>();
            if (parser.TryDeserialize(e.Data, out var frame))
            {
                var sf = ((SearchResponseFrame)frame!);
                Console.WriteLine($"SearchResponse on {e.ReceivingEndpoint}: {sf.DeviceInfoDIB.FriendlyName} - {sf.ServiceEndpoint.Ip} - {sf.DeviceInfoDIB.IndividualAddress} - {BitConverter.ToString(sf.DeviceInfoDIB.MAC)}");
            }
            else
                Console.WriteLine(BitConverter.ToString(e.Data)); ;
        }
    }
}
