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

using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Common.IO;
using System;
using Tiveria.Home.Knx.ObjectServer;

namespace Tiveria.Home.Knx.IP.ServiceTypes
{
    public class ObjectServerMessage : ServiceTypeBase
    {

        private readonly ConnectionHeader _connectionHeader;
        private readonly byte _mainServiceId;
        private readonly byte _subServiceId;
        private readonly byte[] _payload;
        private readonly IObjectServerService _service;

        public ConnectionHeader ConnectionHeader => _connectionHeader;
        public byte MainServiceId => _mainServiceId;
        public byte SubServiceId => _subServiceId;
        public byte[] Payload => _payload;

        public IObjectServerService Service { get => _service; }


        protected ObjectServerMessage()
            : base(ServiceTypeIdentifier.OBJECTSERVER)
        { }

        protected ObjectServerMessage(IndividualEndianessBinaryReader br)
            : this()
        {
            _connectionHeader = ConnectionHeader.Parse(br);
            _mainServiceId = br.ReadByte();
            _subServiceId = br.ReadByte();
            _payload = br.ReadBytesFull();
        }

        public ObjectServerMessage(byte mainserviceid, byte subserviceid, byte[] body)
            : this()
        {
            _connectionHeader = new ConnectionHeader(0, 0);
            _mainServiceId = mainserviceid;
            _subServiceId = subserviceid;
            _payload = (byte[])body?.Clone() ?? new byte[0];
        }

        private IObjectServerService GetServiceFromBody()
        {
            if (_payload == null || _payload.Length == 0)
                return null;

            if (_mainServiceId != 0xF0)
                return null;

            switch (_subServiceId)
            {
                case SetDatapointValueReqService.SubService:
                    return new SetDatapointValueReqService(_payload);
                case SetDatapointValueResService.SubService:
                    return new SetDatapointValueResService(_payload);
                default:
                    return null;
            }
        }

        public override void WriteToByteArray(byte[] buffer, int offset = 0)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");
            if (offset + _size > buffer.Length)
                throw new ArgumentOutOfRangeException("buffer too small");
            _connectionHeader.WriteToByteArray(buffer, offset);
            buffer[offset + _connectionHeader.Size + 0] = _mainServiceId;
            buffer[offset + _connectionHeader.Size + 1] = _mainServiceId;
            _payload.CopyTo(buffer, offset + _connectionHeader.Size + 2);
        }

        #region static methods
        /// <summary>
        /// Parses a part of a buffer and creates a CemiLData class from it
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static ObjectServerMessage Parse(byte[] buffer, int offset, int length)
        {
            return new ObjectServerMessage(new IndividualEndianessBinaryReader(buffer, offset, length));
        }

        public static bool TryParse(out ObjectServerMessage tunnelRequest, byte[] buffer, int offset, int length)
        {
            bool result = false;
            try
            {
                tunnelRequest = new ObjectServerMessage(new IndividualEndianessBinaryReader(buffer, offset, length));
                result = true;
            }
            catch
            {
                tunnelRequest = null;
                result = false;
            }
            return result;
        }
        #endregion
    }


}
