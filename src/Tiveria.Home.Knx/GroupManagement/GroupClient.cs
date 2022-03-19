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
using Tiveria.Home.Knx.Datapoint;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Home.Knx.Primitives;

namespace Tiveria.Home.Knx.GroupManagement
{
    /// <summary>
    /// Class simplifying the work with one <see cref="GroupAddress"/>
    /// </summary>
    /// <typeparam name="T">The target type of the <see cref="GroupAddress"/></typeparam>
    public class GroupClient<T>
    {
        #region Private Fields

        private readonly GroupAddress _address;
        private readonly ManualResetEventSlim _answerEvent = new ManualResetEventSlim(false);
        private readonly IKnxConnection _client;
        private readonly DPType<T> _translator;
        private byte[]? _data;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Creates a new instance of the GroupCLient class
        /// </summary>
        /// <param name="client">The underlying connection client based on <see cref="IKnxConnection"/></param>
        /// <param name="address">The <see cref="GroupAddress"/> to work with</param>
        /// <param name="translator">A translator that is used to convert the APDU data from and to target type</param>
        public GroupClient(IKnxConnection client, GroupAddress address, DPType<T> translator)
        {
            _client = client;
            _address = address;
            _translator = translator;
            _client.CemiReceived += _client_CemiReceived;
        }

        #endregion Public Constructors



        #region Public Methods

        /// <summary>
        /// Reads the current value of the <see cref="GroupAddress"/>
        /// </summary>
        /// <returns>The value</returns>
        /// <exception cref="KnxTimeoutException">In case no answer is received</exception>
        public async Task<T?> ReadValueAsync()
        {
            var apdu = new Apdu(Cemi.ApduType.GroupValue_Read);
            var ctrl1 = new ControlField1();
            var ctrl2 = new ControlField2(groupAddress: true);
            var cemi = new CemiLData(Cemi.MessageCode.LDATA_REQ, _address, ctrl1, ctrl2, apdu);
            _answerEvent.Reset();
            await _client.SendCemiAsync(cemi);
            if (!_answerEvent.Wait(2000))
                throw new KnxTimeoutException();
            if (_data == null)
                throw new KnxCommunicationException();
            return _translator.Decode(_data);
        }

        /// <summary>
        /// Writes the specified value to the <see cref="GroupAddress"/>
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <returns>The awaitable task</returns>
        public Task WriteValueAsync(T value)
        {
            var data = _translator.Encode(value);
            var apdu = new Apdu(Cemi.ApduType.GroupValue_Write, data);
            var ctrl1 = new ControlField1();
            var ctrl2 = new ControlField2(groupAddress: true);
            var cemi = new CemiLData(Cemi.MessageCode.LDATA_REQ, _address, ctrl1, ctrl2, apdu);
            return _client.SendCemiAsync(cemi);
        }

        #endregion Public Methods

        #region Private Methods

        private void _client_CemiReceived(object? sender, CemiReceivedArgs e)
        {
            var cemi = e.Message as CemiLData;
            if (cemi == null || cemi.DestinationAddress != _address || cemi.Apdu == null || cemi.Apdu.ApduType != ApduType.GroupValue_Response)
                return;
            _data = cemi.Apdu.Data != null ? (byte[])cemi.Apdu.Data.Clone() : Array.Empty<byte>();
            _answerEvent.Set();
        }

        #endregion Private Methods
    }
}
