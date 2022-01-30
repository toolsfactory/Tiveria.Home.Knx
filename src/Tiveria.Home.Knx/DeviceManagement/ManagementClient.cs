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
        private int _sequenceNo = 0;

        public ManagementClient(IKnxClient client)
        {
            _client = client;
        }


        public async Task ConnectAsync()
        {
            var tpci = new Tpci(PacketType.Control, SequenceType.UnNumbered, 0, ControlType.Connect);
            var ctrl1 = new ControlField1(priority: Priority.System, broadcast: BroadcastType.Normal, ack: false);
            var ctrl2 = new ControlField2(groupAddress: false); 
            var cemi = new CemiLData(Cemi.MessageCode.LDATA_REQ, new List<AdditionalInformationField>(), new IndividualAddress(0, 0, 0), IndividualAddress.Parse("1.1.2"), ctrl1, ctrl2, tpci, null);
            await _client.SendCemiAsync(cemi);
            _sequenceNo = 0;
        }

        public async Task DisconnectAsync()
        {
            var tpci = new Tpci(Cemi.PacketType.Control, Cemi.SequenceType.UnNumbered, 0, ControlType.Disconnect);
            var ctrl1 = new ControlField1();
            var ctrl2 = new ControlField2(groupAddress: false);
            var cemi = new Cemi.CemiLData(Cemi.MessageCode.LDATA_REQ, new List<AdditionalInformationField>(), new IndividualAddress(0, 0, 0), IndividualAddress.Parse("1.1.2"), ctrl1, ctrl2, tpci, null);
            await _client.SendCemiAsync(cemi);
        }

        public async Task ReadPropertyAsync(byte objIdx, byte propId)
        {
            GetNextSequenceNumber();
            var apdu = new Apdu(ApciType.PropertyValue_Read,new byte[] { objIdx, propId , 0x10, 0x01});
            var ctrl1 = new ControlField1(priority: Priority.Low);
            var ctrl2 = new ControlField2(groupAddress: false);
            var cemi = new Cemi.CemiLData(Cemi.MessageCode.LDATA_REQ, new List<AdditionalInformationField>(), new IndividualAddress(0, 0, 0), IndividualAddress.Parse("1.1.2"), ctrl1, ctrl2, new Tpci(), apdu);
            await _client.SendCemiAsync(cemi);
        }

        private int GetNextSequenceNumber()
        {
            _sequenceNo = (_sequenceNo == 15) ? 0 : _sequenceNo + 1;
            return _sequenceNo;
        }
    }

    public class DeviceConnectionSession
    {
        public DeviceConnectionSession(IndividualAddress address, bool keepAlive)
        {
            Address = address;
            KeepAlive = keepAlive;

            _timer = new System.Timers.Timer(KnxConstants.DeviceConnectionTimeout);
            _timer.AutoReset = false;
            _timer.Elapsed += _timer_Elapsed;
        }

        public int SeqNoSend { get; private set; } = 0;
        public int SeqNoReceive { get; private set; } = 0;
        public IndividualAddress Address { get; }
        public bool KeepAlive { get; }
        public DeviceConnectionState State { get; private set; } = DeviceConnectionState.Closed;

        public void IncSeqNoSend() => SeqNoSend = ++SeqNoSend & 0x0f;
        public void IncSeqNoReceive() => SeqNoReceive = ++SeqNoReceive & 0x0f;

        private System.Timers.Timer _timer;

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// see Chapter 5.1 of 3.3.4 Transport Layer Communication in Knx Specs
    /// </summary>
    public enum DeviceConnectionState
    {
        Closed,
        OpenIdle,
        OPenWait,
        Connecting
    }
}
