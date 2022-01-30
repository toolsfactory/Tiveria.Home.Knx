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
using Tiveria.Home.Knx.Adresses;
using Tiveria.Home.Knx.IP;
using Tiveria.Common.Extensions;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.Datapoint;
using System.Net.Sockets;
using NLog;
using NLog.Config;
using NLog.Targets;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.IP.Services;

namespace Tiveria.Home.Knx
{
    public class RoutingMonitor
    {
        // replace the IP Address below with your specific router or tunnel interface IP Address.
        // Port should be correct assuming you have a standard setup

        private IP.Connections.RoutingConnection? Con;

        public async Task RunAsync()
         {

            ConsoleKeyInfo cki;
            // Prevent example from ending if CTL+C is pressed.
            Console.TreatControlCAsInput = true;
            ConfigureLogging();


            Con = new IP.Connections.RoutingConnection(new IPEndPoint(IPAddress.Parse("192.168.2.107"), KnxNetIPConstants.DefaultPort)) ;
            Con.DataReceived += Con_DataReceived;
            Con.FrameReceived += Con_FrameReceived;
            Con.ConnectionStateChanged += Con_StateChanged;

            Con.ConnectAsync().Wait();

            do
            {
                cki = Console.ReadKey(false);
                switch (cki.Key)
                {
                    case ConsoleKey.A: SendReadRequestAsync().Wait(); break;
                    case ConsoleKey.D0: SendWriteRequestAsync(false).Wait(); break;
                    case ConsoleKey.D1: SendWriteRequestAsync(true).Wait(); break;
                }

            } while (cki.Key != ConsoleKey.Escape);
            await Con.DisconnectAsync();
        }


        private async Task SendWriteRequestAsync(bool on)
        {
            var data = (byte)(on ? 0x01 : 0x00);
            var tpdu = new Apdu(ApciType.GroupValue_Write, new byte[] { data });
//            var ctrl1 = new ControlField1(MessageCode.LDATA_IND, 0xbc);
            var ctrl1 = new ControlField1(priority: Priority.Normal, ack: false, repeat: false, confirm: ConfirmType.NoError);
            var ctrl2 = new ControlField2();
            var cemi = new Cemi.CemiLData(Cemi.MessageCode.LDATA_IND, new List<AdditionalInformationField>(), new IndividualAddress(0, 0, 1), GroupAddress.Parse("4/0/0"), ctrl1, ctrl2, default(Tpci), tpdu);
            await Con.SendCemiAsync(cemi);
        }

        private async Task SendReadRequestAsync()
        {
            Console.WriteLine("Sending read request to 22/7/0");
//            var cemi = Cemi.CemiLData.CreateReadRequestCemi(new IndividualAddress(1, 1, 206), new GroupAddress(22, 7, 0));
            //await Con.SendCemiFrameAsync(cemi, true);
        }

        private async Task SendReadAnswerAsync()
        {
            Console.WriteLine("Sending read answer for 29/0/0");
            var data = DPType9.DPT_TEMPERATURE.Encode(12.3);
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
            if (e.Frame.FrameHeader.ServiceTypeIdentifier == ServiceTypeIdentifier.RoutingIndication)
            {
                var req = ((RoutingIndicationService)e.Frame.Service);
                var cemi = (CemiLData)req.CemiMessage;

                if (cemi.DestinationAddress.IsGroupAddress())
                {
                    var addr = (cemi.DestinationAddress).ToString();
                    if (cemi.Apdu != null)
                    {
                        if (cemi.Apdu.Type == ApciType.GroupValue_Write)
                        {
                            if (addr.EndsWith("/2/3") || addr.EndsWith("/2/23") || addr.EndsWith("/2/43") || addr.EndsWith("/2/63"))
                            {
                                var value = DPType5.DPT_SCALING.Decode(cemi.Apdu.Data);
                                Console.WriteLine($"++ {cemi.Apdu.Type} for \"{addr}\": {value}%");
                            }
                            else if (addr.EndsWith("/1/12") || addr.EndsWith("/1/22") || addr.EndsWith("/1/32") || addr.EndsWith("/1/42") || addr.EndsWith("/1/52"))
                            {
                                var value = DPType14.DPT_ELECTRIC_CURRENT.Decode(cemi.Apdu.Data);
                                Console.WriteLine($"++ {cemi.Apdu.Type} for \"{addr}\": {value}A");
                            }
                            else if (addr.EndsWith("/47"))
                            {
                                var value = DPType7.DPT_TIMEPERIOD_HRS.Decode(cemi.Apdu.Data);
                                Console.WriteLine($"++ {cemi.Apdu.Type} for \"{addr}\": {value}h");
                            }
                            else if (addr.EndsWith("5/0") || addr.EndsWith("/2/7") || addr.EndsWith("/2/9"))
                            {
                                var value = DPType9.DPT_TEMPERATURE.Decode(cemi.Apdu.Data);
                                Console.WriteLine($"++ {cemi.Apdu.Type} for \"{addr}\": {value}°C");
                            }
                            else if (addr.EndsWith("0/7/0"))
                            {
                                var value = DPType11.DPT_DATE.Decode(cemi.Apdu.Data);
                                Console.WriteLine($"++ {cemi.Apdu.Type} for \"{addr}\": {value}");
                            }
                            else if (addr.EndsWith("0/7/1"))
                            {
                                var value = DPType10.DPT_TIMEOFDAY.Decode(cemi.Apdu.Data);
                                Console.WriteLine($"++ {cemi.Apdu.Type} for \"{addr}\": {value}");
                            }
                            else
                            {
                                Console.WriteLine($"{cemi.Apdu.Type} for \"{addr}\" - ACPI DATA: {cemi.Apdu.Data.ToHex()}");
                            }
                        }
                        else if ((cemi.Apdu.Type == ApciType.GroupValue_Read) && addr.EndsWith("29/0/0"))
                        {
                            SendReadAnswerAsync();
                        }
                        if (cemi.Apdu.Type == ApciType.GroupValue_Response)
                        {
                            Console.WriteLine($"--{cemi.Apdu.Type} for \"{addr}\" - ACPI DATA: {cemi.Apdu.Data.ToHex()}");
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
//            Console.WriteLine(e.Data.ToHexString());
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

        private void ConfigureLogging()
        {
            var config = new LoggingConfiguration();

            // Step 2. Create targets, configure and add them to the configuration 
            var consoleTarget = new ColoredConsoleTarget();
            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            config.AddTarget("console", consoleTarget);

            var fileTarget = new FileTarget();
            fileTarget.FileName = "${basedir}/file.txt";
            fileTarget.Layout = "${message}";
            config.AddTarget("file", fileTarget);

            var logViewerTarget = new NLogViewerTarget();
            logViewerTarget.Address = "tcp://127.0.0.1:878";
            config.AddTarget("viewer", logViewerTarget);

            // Step 3. Define rules
            var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);
            var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);
            var rule3 = new LoggingRule("*", LogLevel.Trace, logViewerTarget);
            config.LoggingRules.Add(rule3);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;
//            Tiveria.Home.Knx.Utils.LogFactory.LogManager = new NLogLogManager();

        }
    }
}
