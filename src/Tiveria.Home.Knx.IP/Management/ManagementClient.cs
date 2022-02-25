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

using Tiveria.Home.Knx.Cemi;
using Tiveria.Home.Knx.Management;
using Tiveria.Home.Knx.IP.Connections;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Services;
using Tiveria.Home.Knx.Primitives;

namespace Tiveria.Home.Knx.IP.Management
{
    /// <summary>
    /// Implementation of the <see cref="IConnectedDeviceManagement"/> interface for tunneling based IP connections.
    /// </summary>
    public class TunnelingManagementClient : ManagementClientBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TunnelingManagementClient"/> class
        /// </summary>
        /// <param name="connection">The underlying <see cref="TunnelingConnection"/> to be used</param>
        /// <param name="remoteAddress">The Knx <see cref="IndividualAddress"/> of the device to connect to</param>
        public TunnelingManagementClient(IKnxNetIPConnection connection, IndividualAddress remoteAddress) 
            : base(connection, remoteAddress)
        {
            connection.FrameReceived += _client_FrameReceived;
        }

        private void _client_FrameReceived(object? sender, FrameReceivedEventArgs e)
        {
            if (e.Frame.Service.ServiceTypeIdentifier != ServiceTypeIdentifier.TunnelingRequest)
                return;
            var svc = (TunnelingRequestService)e.Frame.Service;
            var cemi = (CemiLData)svc.CemiMessage;
            if (svc == null || cemi == null) return;
            if (cemi!.Apdu == null || cemi.MessageCode == MessageCode.LDATA_CON) return;
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
    }
}
