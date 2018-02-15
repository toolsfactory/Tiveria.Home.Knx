using System;
using System.Net;
using Tiveria.Knx;
using Tiveria.Knx.IP;
using Tiveria.Common.Extensions;
using Tiveria.Knx.IP.Utils;
using Tiveria.Knx.IP.ServiceTypes;
using System.Net.Sockets;
using Tiveria.Common.Logging;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleLogger.UseErrorOutputStream = false;

            var remoteip = IPAddress.Parse("192.168.2.101"); // 230
            ushort port = 3671;

            var con = new Tiveria.Knx.IP.TunnelingConnection(remoteip, port, GetLocalIPAddress(), 3671);
            con.DataReceived += Con_DataReceived;
            con.FrameReceived += Con_FrameReceived;
            con.StateChanged += Con_StateChanged;
            Console.WriteLine("Hello World!");

            con.ConnectAsync().Wait();
            Console.ReadLine();
            con.Stop();
        }

        private static void Con_StateChanged(object sender, StateChangedEventArgs e)
        {
            Console.WriteLine(" == Connection state changed == " + e.ConnectionState.ToString());
        }

        private static void Con_FrameReceived(object sender, FrameReceivedEventArgs e)
        {
            Console.WriteLine($"Frame received. Type: {e.Frame.ServiceType}");
            if(e.Frame.ServiceType.ServiceTypeIdentifier == ServiceTypeIdentifier.TUNNELING_REQ)
            {
                var req = (TunnelingRequest)e.Frame.ServiceType;
                Console.WriteLine(req.CemiFrame.ToDescription(2));
                //req.CemiFrame.
            }
        }

        private static void Con_DataReceived(object sender, Tiveria.Knx.IP.DataReceivedArgs e)
        {
            Console.WriteLine(e.Data.ToHexString());
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
    }
}
