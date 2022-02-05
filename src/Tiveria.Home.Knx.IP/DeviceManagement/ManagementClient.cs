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
using Tiveria.Common;
using Tiveria.Home.Knx.BaseTypes;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.DeviceManagement;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Services;

namespace Tiveria.Home.Knx.IP.DeviceManagement
{
    public class ManagementClient : IManagementClient
    {
        #region private members
        private IKnxNetIPConnection _client;
        private int _sequenceNo = 0;
        private object _incLock = new object();
        private object _listLock = new object();
        private CancellationTokenSource _disconnectCTS = new CancellationTokenSource();
        private Dictionary<int, Apdu> _list = new Dictionary<int, Apdu>(15);
        private int[] _responseApci = new int[] {ApduType.DeviceDescriptor_Response, ApduType.Memory_Response, ApduType.PropertyValue_Response, ApduType.PropertyDescription_Response, ApduType.RestartMasterReset_Response };
        #endregion

        #region public properties
        /// <summary>
        /// State of the current connection
        /// </summary>
        public ManagementConnectionState State { get; private set; }

        /// <summary>
        /// Shows if the client is currently connected to a device
        /// </summary>
        public bool IsConnected => State == ManagementConnectionState.OpenIdle || State == ManagementConnectionState.OpenWait;
        
        /// <summary>
        /// Maximum time the client waits for a response to a request. Timout in milliseconds
        /// </summary>
        public ushort ResponseTimeout { get; set; } = KnxNetIPConstants.ResponseTimeoutMS;

        /// <summary>
        /// Priority of the messages sent to the device
        /// </summary>
        public Priority Priority { get; set; } = Priority.Normal;

        /// <summary>
        /// Individual address of the remote knx device
        /// </summary>
        public IndividualAddress RemoteAddress { get; init; }

        /// <summary>
        /// Provides the maximum supported lenght of Apdu frames. In case the device doesnt have this property, 15 is set as default.
        /// </summary>
        public int MaxApduFrameLength { get; private set; } = 15;

        /// <summary>
        /// Indicates if the devices supports extended frames.
        /// </summary>
        public bool ExtendedFramesSupported { get; private set; }
        #endregion

        #region constructor
        public ManagementClient(IKnxNetIPConnection client, IndividualAddress remoteAddress)
        {
            _client = client;
            RemoteAddress = remoteAddress;
            _client.FrameReceived += _client_FrameReceived;
        }
        #endregion

        #region public implementatation
        #region connection management
        public async Task ConnectAsync()
        {
            if (IsConnected)
                return;
            InitConnection();

            await SendControlCemiAsync(ControlType.Connect);
            State = ManagementConnectionState.OpenIdle;
            await GetMaxApduFrameLength();
        }

        public async Task DisconnectAsync()
        {
            if (!IsConnected)
                return;
            _disconnectCTS.Cancel();
            State = ManagementConnectionState.Closed;
            await SendControlCemiAsync(ControlType.Disconnect);
        }
        #endregion

        #region property & propertydescription
        public async Task<byte[]> ReadPropertyAsync(byte objIdx, byte propId)
        {
            var seq = await SendNumberedDataCemiAsync(ApduType.PropertyValue_Read, new byte[] { objIdx, propId, 0x10, 0x01 });
            var resp = await WaitForSequenceResponseAsync(ApduType.PropertyValue_Response, seq);
            return  resp.Data;
        }

        public async Task WritePropertyAsync(byte objIdx, byte propId, int startIndex, int elements, byte[] data)
        {
            Ensure.That(startIndex, nameof(startIndex)).IsInRange(0, 0x0fffff);
            Ensure.That(elements, nameof(elements)).IsInRange(1, 0x0f);
            Ensure.That(data, nameof(data)).IsNotNull();
            Ensure.That(data.Length, nameof(data)).IsGreaterThan(0);

            var apdudata = new byte[data.Length + 4];
            apdudata[0] = objIdx;
            apdudata[1] = propId;
            apdudata[2] = (byte)((elements << 4) | ((startIndex >> 8) & 0xF));
            apdudata[3] = (byte)startIndex;
            data.CopyTo(apdudata, 4);
            var seq = await SendNumberedDataCemiAsync(ApduType.PropertyValue_Write, new byte[] { objIdx, propId, 0x10, 0x01 });
            var resp = await WaitForSequenceResponseAsync(ApduType.PropertyValue_Response, seq);
            var retElements = resp.Data[2] >> 4 & 0b00001111;

            if (retElements == 0)
                throw new KnxCommunicationException("Property write forbidden");
            if(retElements != elements)
                throw new KnxCommunicationException($"Elements count missmatch. No of returned elements ({retElements}) is not equial to sent no ({elements}).");
            if (resp.Data.Length != data.Length)
                throw new KnxCommunicationException($"Data missmatch. Length of returned data ({resp.Data.Length}) is not equial to sent data ({data.Length}).");
            if (!data.SequenceEqual(resp.Data))
                throw new KnxCommunicationException($"Data missmatch. Content of returned data doesn't match sent data.");
        }

        public async Task<PropertyDescription> ReadPropertyDescriptionAsync(byte objIdx, byte propId)
        {
            var seq = await SendNumberedDataCemiAsync(ApduType.PropertyDescription_Read, new byte[] { objIdx, propId, 0x00 });
            var resp = await WaitForSequenceResponseAsync(ApduType.PropertyDescription_Response, seq);
            return new PropertyDescription(resp.Data);
        }
        #endregion

        #region Memory
        public async Task<byte[]> ReadMemoryAsync(int address, int count)
        {
            Ensure.That(address, nameof(address)).IsInRange(0, 0xffff);
            Ensure.That(count, nameof(count)).IsInRange(1, 63);
            byte[] addr = BitConverter.GetBytes(address);
            var seq = await SendNumberedDataCemiAsync(ApduType.Memory_Read, new byte[] { (byte)count, addr[1], addr[0] });
            var resp = await WaitForSequenceResponseAsync(ApduType.Memory_Response, seq);
//            if(resp.Data[0] == 0)
//                throw new KnxCommunicationException($"Data missmatch. No data returned from address 0x{address:x4}.");
            return resp.Data.AsSpan().Slice(3).ToArray();
        }

        public async Task WriteMemoryAsync(int address, byte[] data)
        {
            Ensure.That(address, nameof(address)).IsInRange(0, 0xffff);
            Ensure.That(data.Length, "data.Length").IsInRange(1, 63);
            var asdu = new byte[data.Length + 3];
            asdu[0] = (byte)data.Length;
            asdu[1] = (byte)(address >> 8);
            asdu[2] = (byte)address;
            data.CopyTo(asdu, 3);
            var seq = await SendNumberedDataCemiAsync(ApduType.Memory_Write, asdu);
            var resp = await WaitForSequenceResponseAsync(ApduType.Memory_Response, seq);
            if (resp.Data[0] != data.Length)
                throw new KnxCommunicationException($"Data missmatch. Length of returned data ({resp.Data.Length}) is not equial to sent data ({data.Length}).");
        }
        #endregion

        #region ADC
        public async Task<int> ReadADCAsync(byte channel, byte repeats)
        {
            var seq = await SendNumberedDataCemiAsync(ApduType.DeviceDescriptor_Read, new byte[] { (byte)channel, (byte)repeats });
            var resp = await WaitForSequenceResponseAsync(ApduType.ADC_Response, seq);
            return resp.Data[2] << 8 | resp.Data[3];
        }
        #endregion

        #region device descriptior
        public async Task<byte[]> ReadDeviceDescriptorAsync(byte descriptorType = 0)
        {
            var seq = await SendNumberedDataCemiAsync(ApduType.DeviceDescriptor_Read, new byte[] { descriptorType });
            var resp = await WaitForSequenceResponseAsync(ApduType.DeviceDescriptor_Response, seq);
            return resp.Data;
        }
        #endregion

        #region other
        public Task<PropertyDescription> ReadPropertyDescriptionByIndexAsync(byte objIdx, byte index)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadPropertiesAsync(byte objIdx, byte propId, int start, int count)
        {
            throw new NotImplementedException();
        }

        public async Task RestartDeviceAsync()
        {
            await SendNumberedDataCemiAsync(ApduType.Restart_Request, Array.Empty<byte>());
        }

        public Task<IndividualAddress> ReadIndividualAddressAsync()
        {
            throw new NotImplementedException();
        }

        public Task<(byte ErrorCode, int ProcessTime)> ResetDeviceAsync(EraseCode code, int channelid, bool areYouSure)
        {
            throw new NotImplementedException();
        }

        public Task<IndividualAddress> ReadAddressAsync(SerialNumber serialNumber)
        {
            throw new NotImplementedException();
        }

        public Task<IndividualAddress[]> ReadAllAddressesAsync()
        {
            throw new NotImplementedException();
        }

        public Task WriteAddressAsync(SerialNumber serialNumber, IndividualAddress address)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadDomainAddressAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<byte[]>> ReadDomainAddressesAsync(byte[] domainaddress, IndividualAddress startAddress, int range)
        {
            throw new NotImplementedException();
        }

        public Task<IList<byte[]>> ReadAllDomainAddressAsync()
        {
            throw new NotImplementedException();
        }

        public Task WriteDomainAddressAsync(byte[] domainAddress)
        {
            throw new NotImplementedException();
        }

        public Task WriteDomainAddressAsync(SerialNumber serialNumber, byte[] domainAddress)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region private implementations
        private async Task GetMaxApduFrameLength()
        {
            try
            {
                var len = await ReadPropertyAsync(0, 56);
                MaxApduFrameLength = len[0] < 15 ? 15 : len[0];
                ExtendedFramesSupported = MaxApduFrameLength > 15;
            }
            catch
            {
                MaxApduFrameLength = 15;
            }
        }

        private int GetNextSequenceNumber()
        {
            lock (_incLock)
            {
                _sequenceNo = (_sequenceNo == 15) ? 0 : _sequenceNo + 1;
            }
            return _sequenceNo;
        }

        private void InitConnection()
        {
            // first call to GetNextSequenceNumber returns 0;
            _sequenceNo = 15;
            State = ManagementConnectionState.Closed;
            if (_disconnectCTS.IsCancellationRequested)
                _disconnectCTS = new CancellationTokenSource();
        }

        private async Task SendControlCemiAsync(ControlType ctrlType)
        {
            var cts = new CancellationTokenSource(2000);
            var tpci = new Tpci(Cemi.PacketType.Control, Cemi.SequenceType.UnNumbered, 0, ctrlType);
            var cemi = BuildRequestCemi(tpci, null);
            await _client.SendCemiAsync(cemi);
        }

        private async Task<int> SendNumberedDataCemiAsync(int ApduType, byte[]data)
        {
            if (!IsConnected)
                throw new KnxConnectionException("Client not connected to device!");
            var seq = GetNextSequenceNumber();
            var tpci = new Tpci(PacketType.Data, SequenceType.Numbered, (byte)seq, ControlType.None);
            var apdu = new Apdu(ApduType, data);
            var cemi = BuildRequestCemi(tpci, apdu);
            await _client.SendCemiAsync(cemi);
            return seq;
        }

        private CemiLData BuildRequestCemi(Tpci tpci, Apdu? apdu = null)
        {
            var ctrl1 = new ControlField1(priority: Priority, broadcast: BroadcastType.Normal, ack: false);
            var ctrl2 = new ControlField2(groupAddress: false);
            return new CemiLData(Cemi.MessageCode.LDATA_REQ, new List<AdditionalInformationField>(), new IndividualAddress(0, 0, 0), RemoteAddress, ctrl1, ctrl2, tpci, apdu);
        }

        private void SendTpciAckFrameAsync(byte sequenceNumber)
        {
            var tpci = new Tpci(PacketType.Control, SequenceType.Numbered, sequenceNumber, ControlType.Ack);
            var cemi = BuildRequestCemi(tpci, null);
            _client.SendCemiAsync(cemi);
        }

        private void _client_FrameReceived(object? sender, FrameReceivedEventArgs e)
        {
            if (e.Frame.Service.ServiceTypeIdentifier != ServiceTypeIdentifier.TunnelingRequest)
                return;
            var svc = (TunnelingRequestService)e.Frame.Service;
            var cemi = (CemiLData)svc.CemiMessage;
            if (svc == null || cemi == null) return;
            if (cemi.Apdu == null || cemi.MessageCode == MessageCode.LDATA_CON) return;
            if (_responseApci.Contains(cemi.Apdu.ApduType))
            {
                lock (_listLock)
                {
                    if (_list.ContainsKey(cemi.Tpci.SequenceNumber))
                        _list[cemi.Tpci.SequenceNumber] = cemi.Apdu;
                    else
                        _list.Add(cemi.Tpci.SequenceNumber, cemi.Apdu);
                }
                SendTpciAckFrameAsync(cemi.Tpci.SequenceNumber);
            }
        }

        private async Task<Apdu> WaitForSequenceResponseAsync(int ApduType, int sequenceNo)
        {
            var timeToken = new CancellationTokenSource(ResponseTimeout).Token;
            var cts = CancellationTokenSource.CreateLinkedTokenSource(_disconnectCTS.Token, timeToken);
            while (!_list.ContainsKey(sequenceNo) && !cts.Token.IsCancellationRequested)
                await Task.Delay(10);
            if (timeToken.IsCancellationRequested)
                throw new KnxTimeoutException("no answer received in time");

            Apdu apdu;
            lock (_listLock)
            {
                if (_list.ContainsKey(sequenceNo))
                {
                    apdu = _list[sequenceNo];
                    if (apdu.ApduType == ApduType)
                        _list.Remove(sequenceNo);
                    else
                        throw new KnxCommunicationException("Non matching response/sequenceNumber combination received");
                }
                else
                    throw new KnxCommunicationException("No matching response/sequenceNumber combination received");
            }
            return apdu;
        }
        #endregion
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
