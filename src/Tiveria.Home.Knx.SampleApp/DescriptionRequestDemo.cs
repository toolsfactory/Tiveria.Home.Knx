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
using Tiveria.Common.Extensions;
using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.IP.Services;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx
{
    public class DescriptionRequestDemo
    {
        private UdpClient _client;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public DescriptionRequestDemo()
        {
            _client = new UdpClient(new IPEndPoint(Program.GatewayIPAddress, Program.GatewayPort));
            _cancellationTokenSource = new CancellationTokenSource();
            Listen();
        }

        public void SendDescriptionRequest()
        {
            var service = new DescriptionRequestService(new Hpai(IP.Enums.HPAIEndpointType.IPV4_UDP, System.Net.IPAddress.Parse("0.0.0.0"), 0));
            var frame = new KnxNetIPFrame(service);
            var data = frame.ToBytes();
            _client.Send(data, data.Length);
        }

        public void StopListening()
        {
            _client.Close();
            _cancellationTokenSource.Cancel();
        }

        private void Listen()
        {
            Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var data = await _client.ReceiveAsync().WithCancellation(_cancellationTokenSource.Token)
                                                           .ConfigureAwait(false);
                    HandleData(data.Buffer);
                }
            });
        }

        private void HandleData(byte[] buffer)
        {
            var ok = KnxNetIPFrame.TryParse(buffer, out var frame);
            if (ok && frame!.Service is DescriptionResponseService)
            {
                var descr = (DescriptionResponseService)frame!.Service;
                Console.WriteLine(descr);
            }
        }
    }
}
