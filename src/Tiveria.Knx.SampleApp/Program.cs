using System;
using System.Net;
using Tiveria.Knx;
using Tiveria.Knx.IP;
using Tiveria.Common.Extensions;
using Tiveria.Knx.IP.Utils;
using Tiveria.Knx.IP.ServiceTypes;
using Tiveria.Knx.Datapoint;
using System.Net.Sockets;
using NLog;
using NLog.Targets;
using NLog.Config;

namespace Tiveria.Knx
{
    static class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLogging();
            var remoteip = IPAddress.Parse("192.168.2.101"); // 230
            ushort port = 3671;

            var con = new Tiveria.Knx.IP.TunnelingConnection(remoteip, port, GetLocalIPAddress(), 55555);
            con.DataReceived += Con_DataReceived;
            con.FrameReceived += Con_FrameReceived;
            con.StateChanged += Con_StateChanged;
            Console.WriteLine("Hello World!");

            con.ConnectAsync().Wait();
            Console.ReadLine();
            con.CloseAsync().Wait();
            Console.ReadLine();
        }

        private static void Con_StateChanged(object sender, StateChangedEventArgs e)
        {
            Console.WriteLine(" == Connection state changed == " + e.ConnectionState.ToString());
        }

        private static void Con_FrameReceived(object sender, FrameReceivedEventArgs e)
        {
//            Console.WriteLine($"Frame received. Type: {e.Frame.ServiceType}");
            if (e.Frame.ServiceType.ServiceTypeIdentifier == ServiceTypeIdentifier.TUNNELING_REQ)
            {
                var req = (TunnelingRequest)e.Frame.ServiceType;

                if (req.CemiFrame.DestinationAddress.IsGroupAddress())
                {
                    var addr = ((GroupAddress)req.CemiFrame.DestinationAddress).ToString();
                    if (addr.EndsWith("/2/3") || addr.EndsWith("/2/23") || addr.EndsWith("/2/43") || addr.EndsWith("/2/63"))
                    { 
                        if ((req.CemiFrame.Apci.Type == Cemi.APCIType.GroupValue_Write)
                            || (req.CemiFrame.Apci.Type == Cemi.APCIType.GroupValue_Response))
                        {
                            var value = DPType5.DPT_SCALING.Decode(req.CemiFrame.Apci.Data);
                            Console.WriteLine($"++ {req.CemiFrame.Apci.Type} for \"{addr}\": {value}%");
                        }
                        else
                        {
                            Console.WriteLine($"{req.CemiFrame.Apci.Type} for \"{addr}\" - ACPI DATA: {req.CemiFrame.Apci.Data.ToHexString()} - Payload: {req.CemiFrame.Payload.ToHexString()}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{req.CemiFrame.Apci.Type} for \"{addr}\" - ACPI DATA: {req.CemiFrame.Apci.Data.ToHexString()} - Payload: {req.CemiFrame.Payload.ToHexString()}");
                    }
                }
            }
        }

        private static void Con_DataReceived(object sender, Tiveria.Knx.IP.DataReceivedArgs e)
        {
//            Console.WriteLine(e.Data.ToHexString());
        }

        public static IPAddress GetLocalIPAddress()
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

        private static void ConfigureLogging()
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
            Tiveria.Knx.Utils.LogFactory.LogManager = new NLogLogManager();

        }
    }
}
