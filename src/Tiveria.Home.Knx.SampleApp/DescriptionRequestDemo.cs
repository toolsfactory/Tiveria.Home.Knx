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

using System.Net.Sockets;
using Tiveria.Common.Extensions;
using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.IP.Frames;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx
{
    public class DescriptionRequestDemo
    {
        private UdpClient _client;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public DescriptionRequestDemo(string ip, ushort port)
        {
            _client = new UdpClient(ip, port);
            _cancellationTokenSource = new CancellationTokenSource();
            Listen();
        }

        public void SendDescriptionRequest()
        {
            var frame = new DescriptionRequestFrame(new Hpai(IP.Enums.HPAIEndpointType.IPV4_UDP, System.Net.IPAddress.Parse("0.0.0.0"), 0));
            var data = KnxNetIPFrameSerializerFactory.Instance.Create(frame.ServiceTypeIdentifier).Serialize(frame);
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
            try
            {
                var reader = new Common.IO.BigEndianBinaryReader(buffer);
                var header = FrameHeader.Parse(reader);
                Console.WriteLine(header);
                var parser = KnxNetIPFrameSerializerFactory.Instance.Create(header.ServiceTypeIdentifier);
                reader.Seek(0);
                var frame = parser.Deserialize(reader);
                Console.WriteLine(frame.ToString());
            }
            catch (Exceptions.BufferException be)
            {
            }
            catch (ArgumentException ae)
            {
            }
        }
    }
}
