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

using System.Collections.Concurrent;
using Tiveria.Common;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Home.Knx.IP.Connections;
using Tiveria.Home.Knx.IP.Services;
using Tiveria.Home.Knx.Management;
using Tiveria.Home.Knx.Primitives;

namespace Tiveria.Home.Knx.IP.Management
{
    /// <summary>
    /// Class that provide means to manage certain aspects of a device without establishing a direct connection to the device.
    /// </summary>
    public class ConnectionlessDeviceManagementClient : IConnectionlessDeviceManagement
    {
        #region constants
        private static readonly int[] MonitoredApduTypes = new int[] 
        { 
            ApduType.PropertyValue_Response, 
            ApduType.PropertyDescription_Response, 
            ApduType.RestartMasterReset_Response 
        };
        #endregion

        #region public properties
        /// <summary>
        /// Priority of the messages sent to the device
        /// </summary>
        public Priority Priority { get; set; } = Priority.Normal;

        /// <summary>
        /// Maximum time the client waits for a response to a request. Timout in milliseconds
        /// </summary>
        public ushort ResponseTimeout { get; set; } = KnxConstants.ResponseTimeoutMS;
        #endregion

        #region constructor
        /// <summary>
        /// Creates a new instance of the <see cref="ConnectionlessDeviceManagementClient"/> class
        /// </summary>
        /// <param name="connection">The underlying <see cref="TunnelingConnection"/> to be used</param>
        public ConnectionlessDeviceManagementClient(TunnelingConnection connection)
        {
            _connection = connection;
            connection.FrameReceived += _client_FrameReceived;
        }
        #endregion constructor

        /// <inheritdoc/>
        public async Task<byte[]> ReadPropertyAsync(IndividualAddress address, byte objIdx, byte propId)
        {
            await SendCemiAsync(address, ApduType.PropertyValue_Read, new byte[] { objIdx, propId, 0x10, 0x01 });
            var resp = await WaitForResponseAsync(ApduType.PropertyValue_Response);
            return resp.Data;
        }

        /// <inheritdoc/>
        public async Task<PropertyDescription> ReadPropertyDescriptionAsync(IndividualAddress address, byte objIdx, byte propId)
        {
            await SendCemiAsync(address, ApduType.PropertyDescription_Read, new byte[] { objIdx, propId, 0x00 });
            var resp = await WaitForResponseAsync(ApduType.PropertyDescription_Response);
            return new PropertyDescription(resp.Data);
        }

        /// <inheritdoc/>
        public async Task<(byte ErrorCode, int ProcessTime)> ResetDeviceAsync(IndividualAddress address, EraseCode code, int channelid, bool areYouSure)
        {
            await  SendCemiAsync(address, ApduType.RestartMasterReset_Request, new byte[] {(byte) code, (byte)channelid });
            var resp = await WaitForResponseAsync(ApduType.RestartMasterReset_Response);
            if (resp.Data.Length != 3)
                throw new KnxCommunicationException($"Data missmatch. Length of returned data ({resp.Data.Length}) is not equial to expected length 3.");
            return (resp.Data[0], (resp.Data[1] << 8) + resp.Data[2]);
        }

        /// <inheritdoc/>
        public Task RestartDeviceAsync(IndividualAddress address)
        {
            return SendCemiAsync(address, ApduType.Restart_Request, Array.Empty<byte>());
        }

        /// <inheritdoc/>
        public async Task WritePropertyAsync(IndividualAddress address, byte objIdx, byte propId, int startIndex, int elements, byte[] data)
        {
            Ensure.That(startIndex, nameof(startIndex)).IsInRange(0, 0x0fffff);
            Ensure.That(elements, nameof(elements)).IsInRange(1, 0x0f);
            Ensure.That(data, nameof(data)).IsNotNull();
            Ensure.That(data.Length, nameof(data)).IsGreaterThan(0);

            var apdudata = new byte[data.Length + 4];
            apdudata[0] = objIdx;
            apdudata[1] = propId;
            apdudata[2] = (byte) ((elements << 4) | ((startIndex >> 8) & 0xF));
            apdudata[3] = (byte) startIndex;
            data.CopyTo(apdudata, 4);
            await SendCemiAsync(address, ApduType.PropertyValue_Write, new byte[] { objIdx, propId, 0x10, 0x01 });
            var resp = await WaitForResponseAsync(ApduType.PropertyValue_Response);
            var retElements = resp.Data[2] >> 4 & 0b00001111;

            if (retElements == 0)
                throw new KnxCommunicationException("Property write forbidden");
            if (retElements != elements)
                throw new KnxCommunicationException($"Elements count missmatch. No of returned elements ({retElements}) is not equial to sent no ({elements}).");
            if (resp.Data.Length != data.Length)
                throw new KnxCommunicationException($"Data missmatch. Length of returned data ({resp.Data.Length}) is not equial to sent data ({data.Length}).");
            if (!data.SequenceEqual(resp.Data))
                throw new KnxCommunicationException($"Data missmatch. Content of returned data doesn't match sent data.");
        }

        #region private implementations

        #region private members
        private TunnelingConnection _connection;
        private BlockingCollection<Apdu>_apduCollection = new(25);
        #endregion

        private void _client_FrameReceived(object? sender, FrameReceivedEventArgs e)
        {
            var cemi = GetCemiLDataFromFrame(e.Frame);
            if (cemi != null && MonitoredApduTypes.Contains(cemi.Apdu.ApduType))
            {
                _apduCollection.Add(cemi.Apdu);
            }
        }

        private CemiLData? GetCemiLDataFromFrame(IKnxNetIPFrame frame)
        {
            CemiLData? cemi = (frame.Service as TunnelingRequestService)?.CemiMessage as CemiLData;
            if (cemi == null || cemi.Apdu == null || cemi.MessageCode == MessageCode.LDATA_CON) return null;
            return cemi;
        }

        private Task<Apdu> WaitForResponseAsync(int ApduType)
        {
            return Task.Run(() =>
            {
                var timeToken = new CancellationTokenSource(ResponseTimeout).Token;
                try
                {
                    foreach (var item in _apduCollection.GetConsumingEnumerable(timeToken))
                    {
                        if (item.ApduType == ApduType)
                            return item;
                    }
                }
                catch (OperationCanceledException ex)
                {
                    throw new KnxTimeoutException("no answer received in time", ex);
                }
                throw new KnxCommunicationException("No matching response/sequenceNumber combination received");
            });
        }

        private async Task SendCemiAsync(IndividualAddress remoteAddress, int ApduType, byte[] data)
        {
            var tpci = new Tpci(PacketType.Data, SequenceType.UnNumbered, 0, ControlType.None);
            var apdu = new Apdu(ApduType, data);
            var cemi = BuildRequestCemi(remoteAddress, tpci, apdu);
            await _connection.SendCemiAsync(cemi);
        }

        private CemiLData BuildRequestCemi(IndividualAddress remoteAddress, Tpci tpci, Apdu? apdu = null)
        {
            var ctrl1 = new ControlField1(priority: Priority, broadcast: BroadcastType.Normal, ack: false);
            var ctrl2 = new ControlField2(groupAddress: false);
            return new CemiLData(Cemi.MessageCode.LDATA_REQ, new List<AdditionalInformationField>(), new IndividualAddress(0, 0, 0), remoteAddress, ctrl1, ctrl2, tpci, apdu);
        }

        #endregion private implementations
    }
}
