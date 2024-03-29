﻿/*
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
using System.Net;
using System.Net.Sockets;
using Tiveria.Common.Extensions;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.Datapoint;
using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Services;
using Tiveria.Home.Knx.Primitives;

namespace Tiveria.Home.Knx
{
    public class TunnelingMonitor
    {
        // replace the IP Address below with your specific router or tunnel interface IP Address.
        // Port should be correct assuming you have a standard setup
        private readonly IP.Connections.TunnelingConnection _connection;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<TunnelingMonitor> _logger;

        public TunnelingMonitor(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _connection = new IP.Connections.TunnelingConnectionBuilder(Program.LocalIPAddress, Program.GatewayIPAddress, Program.GatewayPort).WithLogger(_loggerFactory.CreateLogger<IP.Connections.TunnelingConnection>()).Build();
            _logger = _loggerFactory.CreateLogger<TunnelingMonitor>();
        }

        public async Task RunAsync()
        {

            ConsoleKeyInfo cki;
            // Prevent example from ending if CTL+C is pressed.
            Console.TreatControlCAsInput = true;

            _logger.LogInformation("RunAsync - Start");
            _connection.DataReceived += Con_DataReceived!;
            _connection.FrameReceived += Con_FrameReceived!;
            _connection.ConnectionStateChanged += Con_StateChanged!;
            Console.WriteLine("Hello World!");

            _connection.ConnectAsync().Wait();
            do
            {
                cki = Console.ReadKey(false);
                Console.Write(" --- You pressed ");
                if ((cki.Modifiers & ConsoleModifiers.Alt) != 0) Console.Write("ALT+");
                if ((cki.Modifiers & ConsoleModifiers.Shift) != 0) Console.Write("SHIFT+");
                if ((cki.Modifiers & ConsoleModifiers.Control) != 0) Console.Write("CTL+");
                Console.WriteLine(cki.Key.ToString());
                switch (cki.Key)
                {
                    case ConsoleKey.A: SendReadRequestAsync().Wait(); break;
                    case ConsoleKey.Escape: _connection.DisconnectAsync().Wait(); break;
                    case ConsoleKey.D0: SendWriteRequestAsync(false).Wait(); break;
                    case ConsoleKey.D1: SendWriteRequestAsync(true).Wait(); break;
                }
            } while (cki.Key != ConsoleKey.Enter);
            await _connection.DisconnectAsync();
        }


        private async Task SendWriteRequestAsync(bool on)
        {
            var data = (byte)(on ? 0x01 : 0x00);
            var apdu = new Cemi.Apdu(Cemi.ApduType.GroupValue_Write, new byte[] { data });
            var ctrl1 = new ControlField1();
            var ctrl2 = new ControlField2(groupAddress: true);
            var cemi = new Cemi.CemiLData(Cemi.MessageCode.LDATA_REQ, new List<AdditionalInformationField>(), new IndividualAddress(0, 0, 0), GroupAddress.Parse("4/0/0"), ctrl1, ctrl2, new Tpci(), apdu);
            await _connection.SendCemiAsync(cemi);
        }

        private Task SendReadRequestAsync()
        {
            Console.WriteLine("Sending read request to 22/7/0");
            return Task.CompletedTask;
            //            var cemi = Cemi.CemiLData.CreateReadRequestCemi(new IndividualAddress(1, 1, 206), new GroupAddress(22, 7, 0));
            //            await Con.SendCemiFrameAsync(cemi, true);
        }

        private Task SendReadAnswerAsync()
        {
            Console.WriteLine("Sending read answer for 29/0/0");
            var data = DPType9.DPT_TEMPERATURE.Encode(12.3);
            return Task.CompletedTask;
            //            var cemi = Cemi.CemiLData.CreateReadAnswerCemi(new IndividualAddress(1, 1, 206), new GroupAddress(29, 0, 0), data);
            //            await Con.SendCemiFrameAsync(cemi, true);
        }

        private void Con_StateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            Console.WriteLine(" == Connection state changed == " + e.ConnectionState.ToString());
        }

        private void Con_FrameReceived(object sender, FrameReceivedEventArgs e)
        {
            
            //            Console.WriteLine($"Frame received. Type: {e.Frame.ServiceType}");
            if (e.Frame.FrameHeader.ServiceTypeIdentifier == ServiceTypeIdentifier.TunnelingRequest)
            {
                var req = ((TunnelingRequestService)e.Frame.Service);
                var cemi = (CemiLData)req.CemiMessage;

                if (cemi.DestinationAddress.IsGroupAddress())
                {
                    var addr = (cemi.DestinationAddress).ToString();
                    if (cemi.Apdu != null && addr != null)
                    {
                        if (cemi.Apdu.ApduType == Cemi.ApduType.GroupValue_Write)
                        {
                            if (addr.EndsWith("/2/3") || addr.EndsWith("/2/23") || addr.EndsWith("/2/43") || addr.EndsWith("/2/63"))
                            {
                                var value = DPType5.DPT_SCALING.Decode(cemi.Apdu.Data);
                                Console.WriteLine($"++ {cemi.Apdu.ApduType} for \"{addr}\": {value}%");
                            }
                            else if (addr.EndsWith("/1/12") || addr.EndsWith("/1/22") || addr.EndsWith("/1/32") || addr.EndsWith("/1/42") || addr.EndsWith("/1/52"))
                            {
                                var value = DPType14.DPT_ELECTRIC_CURRENT.Decode(cemi.Apdu.Data);
                                Console.WriteLine($"++ {cemi.Apdu.ApduType} for \"{addr}\": {value}A");
                            }
                            else if (addr.EndsWith("/47"))
                            {
                                var value = DPType7.DPT_TIMEPERIOD_HRS.Decode(cemi.Apdu.Data);
                                Console.WriteLine($"++ {cemi.Apdu.ApduType} for \"{addr}\": {value}h");
                            }
                            else if (addr.EndsWith("5/0") || addr.EndsWith("/2/7") || addr.EndsWith("/2/9"))
                            {
                                var value = DPType9.DPT_TEMPERATURE.Decode(cemi.Apdu.Data);
                                Console.WriteLine($"++ {cemi.Apdu.ApduType} for \"{addr}\": {value}°C");
                            }
                            else if (addr.EndsWith("0/7/0"))
                            {
                                var value = DPType11.DPT_DATE.Decode(cemi.Apdu.Data);
                                Console.WriteLine($"++ {cemi.Apdu.ApduType} for \"{addr}\": {value}");
                            }
                            else if (addr.EndsWith("0/7/1"))
                            {
                                var value = DPType10.DPT_TIMEOFDAY.Decode(cemi.Apdu.Data);
                                Console.WriteLine($"++ {cemi.Apdu.ApduType} for \"{addr}\": {value}");
                            }
                            else
                            {
                                Console.WriteLine($"{cemi.Apdu.ApduType} for \"{addr}\" - ACPI DATA: {cemi.Apdu.Data.ToHex()}");
                            }
                        }
                        else if ((cemi.Apdu.ApduType == Cemi.ApduType.GroupValue_Read) && addr.EndsWith("29/0/0"))
                        {
                            SendReadAnswerAsync().Wait();
                        }
                        if (cemi.Apdu.ApduType == Cemi.ApduType.GroupValue_Response)
                        {
                            Console.WriteLine($"--{cemi.Apdu.ApduType} for \"{addr}\" - ACPI DATA: {cemi.Apdu.Data.ToHex()}");
                        }
                        else
                        {
                            //Console.WriteLine($"{cemi.Apdu.Type} for \"{addr}\" - ACPI DATA: {cemi.Apdu.Data.ToHexString()} - Payload: {cemi.Payload.ToHexString()}");
                        }
                    }
                }
            }
        }

        private void Con_DataReceived(object sender, DataReceivedArgs e)
        {
            Console.WriteLine("Received: " + BitConverter.ToString(e.Data));
        }
    }
}
