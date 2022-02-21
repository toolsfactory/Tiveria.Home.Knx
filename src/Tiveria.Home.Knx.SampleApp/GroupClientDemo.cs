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

using Microsoft.Extensions.Logging;
using Spectre.Console;
using Tiveria.Home.Knx.Datapoint;
using Tiveria.Home.Knx.GroupManagement;
using Tiveria.Home.Knx.Primitives;

namespace Tiveria.Home.Knx
{
    public class GroupClientDemo
    {
        private readonly IP.Connections.TunnelingConnection _connection;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<GroupClientDemo> _logger;
        private readonly GroupClient<bool> _switchClient;
        private readonly GroupClient<bool> _statusClient;

        public GroupClientDemo(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _connection = new IP.Connections.TunnelingConnectionBuilder(Program.LocalIPAddress, Program.GatewayIPAddress, Program.GatewayPort).WithLogger(_loggerFactory.CreateLogger<IP.Connections.TunnelingConnection>()).Build();
            _logger = _loggerFactory.CreateLogger<GroupClientDemo>();
            _switchClient = new GroupClient<bool>(_connection, GroupAddress.Parse("4/0/0"), DPType1.DPT_SWITCH);
            _statusClient = new GroupClient<bool>(_connection, GroupAddress.Parse("4/0/1"), DPType1.DPT_SWITCH);
        }

        public async Task RunAsync()
        {
            ConsoleKeyInfo cki;
            // Prevent example from ending if CTL+C is pressed.
            Console.TreatControlCAsInput = true;

            Console.Clear();
            AnsiConsole.MarkupLine("[underline red]  Search Interfaces Demo  [/]");
            Console.WriteLine();

            _connection.ConnectAsync().Wait();
            do
            {
                cki = Console.ReadKey(false);
                Console.Write(" --- You pressed ");
                Console.WriteLine(cki.Key.ToString());
                switch (cki.Key)
                {
                    case ConsoleKey.A:
                        var status = await _statusClient.ReadValueAsync();
                        Console.WriteLine("Switch status: " + status); 
                        break;
                    case ConsoleKey.Escape: _connection.DisconnectAsync().Wait(); break;
                    case ConsoleKey.D0: _switchClient.WriteValueAsync(false).Wait(); break;
                    case ConsoleKey.D1: _switchClient.WriteValueAsync(true).Wait(); break;
                }
            } while (cki.Key != ConsoleKey.Enter);
            await _connection.DisconnectAsync();
        }

    }
}
