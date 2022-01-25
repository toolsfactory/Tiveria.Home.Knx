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

using System;
using System.Collections.Generic;
using System.Text;
using Tiveria.Home.Knx.Adresses;
using Tiveria.Home.Knx.Cemi;

namespace Tiveria.Home.Knx.DeviceManagement
{
    public class ManagementClient //: IManagementClient
    {
        private IKnxClient _client;

        public ManagementClient(IKnxClient client)
        {
            _client = client;
        }


        public async Task ConnectAsync()
        {
            var tpdu = new Tpci(PacketType.Control, SequenceType.UnNumbered, 0, ControlType.Connect);
            var ctrl1 = new ControlField1(MessageCode.LDATA_REQ, priority: Priority.System, broadcast: BroadcastType.Normal, ack: false);
            var ctrl2 = new ControlField2(groupAddress: false); 
            var cemi = new CemiLData(Cemi.MessageCode.LDATA_REQ, new List<AdditionalInformationField>(), new IndividualAddress(0, 0, 0), IndividualAddress.Parse("1.1.2"), ctrl1, ctrl2, tpdu);
            await _client.SendCemiAsync(cemi);
        }

        public async Task DisconnectAsync()
        {
            var tpdu = new Tpci(Cemi.PacketType.Control, Cemi.SequenceType.UnNumbered, 0, ControlType.Disconnect);
            var ctrl1 = new ControlField1(MessageCode.LDATA_REQ);
            var ctrl2 = new ControlField2(groupAddress: false);
            var cemi = new Cemi.CemiLData(Cemi.MessageCode.LDATA_REQ, new List<AdditionalInformationField>(), new IndividualAddress(0, 0, 0), IndividualAddress.Parse("1.1.2"), ctrl1, ctrl2, tpdu);
            await _client.SendCemiAsync(cemi);
        }

        public async Task ReadPropertyAsync(byte objIdx, byte propId)
        {
            var tpdu = new Apci(ApciTypes.Property_Read,new byte[] { objIdx, propId , 0x10, 0x01});
            var ctrl1 = new ControlField1(MessageCode.LDATA_REQ, priority: Priority.Low);
            var ctrl2 = new ControlField2(groupAddress: false);
            var cemi = new Cemi.CemiLData(Cemi.MessageCode.LDATA_REQ, new List<AdditionalInformationField>(), new IndividualAddress(0, 0, 0), IndividualAddress.Parse("1.1.2"), ctrl1, ctrl2, tpdu);
            await _client.SendCemiAsync(cemi);
        }
    }

    public class DeviceConnectionSession
    {
        public DeviceConnectionSession(IndividualAddress address, bool keepAlive)
        {
            Address = address;
            KeepAlive = keepAlive;
        }

        public IndividualAddress Address { get; }
        public bool KeepAlive { get; }
    }
}
