/*
    Tiveria.Knx - a .Net Core base KNX library
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
using Tiveria.Knx.IP.Structures;
using Tiveria.Knx.IP.Utils;
using Tiveria.Knx.Exceptions;

namespace Tiveria.Knx.IP.ServiceTypes
{
    public class ConnectionResponse : ServiceTypeBase, IServiceType
    {
        private Hpai _endpointHPAI;
        private CRD _crd;
        private ErrorCodes _status;
        private byte _channelId;

        public Hpai EndpointHPAI { get => _endpointHPAI; }
        public CRD Crd { get => _crd; }
        public ErrorCodes Status { get => _status; }
        public byte ChannelId { get => _channelId; }

        protected ConnectionResponse()
            : base(ServiceTypeIdentifier.CONNECT_RESPONSE)
        { }

        public ConnectionResponse(ErrorCodes status)
            : base(ServiceTypeIdentifier.CONNECT_RESPONSE)
        {
            _status = status;
            _channelId = 0;
            _endpointHPAI = null;
            _crd = null;
        }

        public ConnectionResponse(byte channelId, ErrorCodes status, Hpai endpoint, CRD crd)
            : this()
        {
            _channelId = channelId;
            _status = status;
            _endpointHPAI = endpoint;
            _crd = crd;
        }

        public override void WriteToByteArray(byte[] buffer, int offset = 0)
        {
            base.WriteToByteArray(buffer, offset);
            buffer[offset] = _channelId;
            buffer[offset + 1] = (byte)_status;
            if (_status == ErrorCodes.NO_ERROR && _crd != null && _endpointHPAI != null)
            {
                _crd.WriteToByteArray(buffer, offset + 2);
                _endpointHPAI.WriteToByteArray(buffer, offset + 2 + _crd.StructureLength);
            }
        }

        public static ConnectionResponse FromBuffer(byte[] buffer, int offset = 0)
        {
            if (buffer.Length - offset < 2)
                throw BufferSizeException.TooSmall("ConnectRespopnse");
            var channelid = buffer[offset];
            var status = buffer[offset + 1];
            if (!Enum.IsDefined(typeof(ErrorCodes), status))
                throw BufferFieldException.TypeUnknown("ConnectionResponse", status);
            if(status == (byte) ErrorCodes.NO_ERROR)
            {
                var endpoint = Hpai.Parse(buffer, offset + 2);
                var crd = CRD.FromBuffer(buffer, offset + 2 + endpoint.StructureLength);
                return new ConnectionResponse(channelid, (ErrorCodes)status, endpoint, crd);
            } else
            {
                return new ConnectionResponse((ErrorCodes)status);
            }
        }
    }
}
