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

using Spectre.Console;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Config;
using NLog.Targets;

namespace Tiveria.Home.Knx
{
    public class Program
    {
        public static IPAddress LocalIPAddress = IPAddress.Parse("192.168.2.107");
        public static IPAddress GatewayIPAddress = IPAddress.Parse("192.168.2.150");
        public static ushort GatewayPort = 3671;

        private static ServiceProvider? _serviceProvider;
        private static Logger? _logger;

        static void Main(string[] args)
        {
            try
            {
                Init();
                bool exit = false;
                while (!exit)
                {
                    Console.Clear();
                    AnsiConsole.MarkupLine("[underline green]  KnxNetIP Demo  [/]");
                    Console.WriteLine();

                    var menu = new EasyConsole.Menu()
                      .Add("Search KnxNetIP Routers/Tunnels", () => _serviceProvider!.GetService<SearchInterfacesDemo>()!.RunAsync().Wait())
                      .Add("Build some structures", () => _serviceProvider!.GetService<StructureBuildDemo>()!.RunAsync().Wait())
                      .Add("Tunneling Connection Tests", () => _serviceProvider!.GetService<TunnelingMonitor>()!.RunAsync().Wait())
                      .Add("Routing Connection Tests", () => _serviceProvider!.GetService<RoutingMonitor>()!.RunAsync().Wait())
                      .Add("DescriptionReq sample", () => _serviceProvider!.GetService<DescriptionRequestDemo>()!.SendDescriptionRequest())
                      .Add("Busmonitor", () => _serviceProvider!.GetService<BusmonitorDemo>()!.RunAsync().Wait())
                      .Add("DeviceManagement", () => _serviceProvider!.GetService<DeviceManagementDemo>()!.RunAsync().Wait())
                      .Add("Exit", () => exit = true);
                    menu.Display();
                }

            }
            catch (Exception ex)
            {
                // NLog: catch any exception and log it.
                _logger!.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }

        }

        private static void Init()
        {
            var config = new ConfigurationBuilder().Build();

            _logger = LogManager.Setup()
                                   .SetupExtensions(ext => ext.RegisterConfigSettings(config))
                                   .GetCurrentClassLogger();

            _serviceProvider = new ServiceCollection()
                    .AddTransient<SearchInterfacesDemo>()
                    .AddTransient<StructureBuildDemo>()
                    .AddTransient<TunnelingMonitor>()
                    .AddTransient<RoutingMonitor>()
                    .AddTransient<DescriptionRequestDemo>()
                    .AddTransient<BusmonitorDemo>()
                    .AddTransient<DeviceManagementDemo>()
                    .AddLogging(loggingBuilder =>
                    {
                        // configure Logging with NLog
                        loggingBuilder.ClearProviders();
                        loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                        loggingBuilder.AddNLog(ConfigureLogging());
                    }).BuildServiceProvider();
        }

        private static LoggingConfiguration ConfigureLogging()
        {
            var config = new LoggingConfiguration();

            var debugTarget = new DebugTarget();
            debugTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            config.AddTarget("debug", debugTarget);

            var logViewerTarget = new NLogViewerTarget();
            logViewerTarget.Address = "udp://127.0.0.1:878";
            config.AddTarget("viewer", logViewerTarget);

            var rule1 = new LoggingRule("*", NLog.LogLevel.Debug, debugTarget);
            config.LoggingRules.Add(rule1);
            var rule3 = new LoggingRule("*", NLog.LogLevel.Trace, logViewerTarget);
            config.LoggingRules.Add(rule3);

            return config;

        }

    }
}
