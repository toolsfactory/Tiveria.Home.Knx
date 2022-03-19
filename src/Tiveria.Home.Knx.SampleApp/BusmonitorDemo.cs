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
using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Services;
using Tiveria.Home.Knx.IP.Connections;

namespace Tiveria.Home.Knx
{
    public class BusmonitorDemo
    {
        private IPAddress _gatewayIPAddress;
        private ushort _gatewayPort;

        private IP.Connections.TunnelingConnection Con = new TunnelingConnectionBuilder(Program.LocalIPAddress, Program.GatewayIPAddress, Program.GatewayPort).Build();
        public BusmonitorDemo()
        {
            _gatewayIPAddress = Program.GatewayIPAddress;
            _gatewayPort = Program.GatewayPort; ;
        }
        // replace the IP Address below with your specific router or tunnel interface IP Address.
        // Port should be correct assuming you have a standard setup

        public async Task RunAsync()
        {

            ConsoleKeyInfo cki;
            // Prevent example from ending if CTL+C is pressed.
            Console.TreatControlCAsInput = true;

            Con.DataReceived += Con_DataReceived;
            Con.FrameReceived += Con_FrameReceived;
            Con.ConnectionStateChanged += Con_StateChanged;
            Console.WriteLine("Hello World!");

            await Con.ConnectAsync();
            do
            {
                cki = Console.ReadKey(false);
                Console.Write(" --- You pressed ");
                Console.WriteLine(cki.Key.ToString());
            } while (cki.Key != ConsoleKey.Escape);
            Con.DisconnectAsync().Wait();
        }


        private void Con_StateChanged(object? sender, ConnectionStateChangedEventArgs e)
        {
            Console.WriteLine(" == Connection state changed == " + e.ConnectionState.ToString());
        }

        private void Con_FrameReceived(object? sender, FrameReceivedEventArgs e)
        {
            //            Console.WriteLine($"Frame received. Type: {e.Frame.ServiceType}");
            if (e.Frame.FrameHeader.ServiceTypeIdentifier == ServiceTypeIdentifier.TunnelingRequest)
            {
                var req = ((TunnelingRequestService)e.Frame);
                //var cemi = (CemiLData)req.CemiMessage;
            }
        }

        private void Con_DataReceived(object? sender, DataReceivedArgs e)
        {
             Console.WriteLine(BitConverter.ToString(e.Data));
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
