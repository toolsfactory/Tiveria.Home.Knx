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
using Tiveria.Home.Knx.IP.ServiceTypes;
using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Common.Extensions;

namespace Tiveria.Home.Knx.IP
{
    public class KnxNetIPFrame
    {
        #region private fields
        private byte[] _body;
        private FrameHeader _header;
        private IServiceType _serviceType;
        #endregion

        #region public properties
        public byte[] Body { get => _body; }
        public FrameHeader Header { get => _header; }
        public IServiceType ServiceType { get => _serviceType; }
        #endregion

        #region Constructors
        public KnxNetIPFrame(ServiceTypeIdentifier servicetypeidentifier, byte[] body)
        {
            if (body == null)
                throw new ArgumentNullException("body is null");
            if (body.Length < 1)
                throw BufferFieldException.TooSmall("ServiceType/Body", 1, 0);
            if (body.Length > 26)
                throw new ArgumentException("body too big");

            _header = new FrameHeader(servicetypeidentifier, (ushort)body.Length);
            _body = body;
            _serviceType = GetServiceTypeFromBody();
        }

        protected KnxNetIPFrame(FrameHeader header, byte[] body)
        {
            if (body == null)
                throw new ArgumentNullException("body is null");
            if (body.Length != (header.TotalLength - header.Size))
                throw new ArgumentException("body size doesn't fit to size specified in header");

            _header = header;
            _body = body;
            _serviceType = GetServiceTypeFromBody();
        }

        public KnxNetIPFrame(IServiceType serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType is null");

            _body = serviceType.ToBytes();
            _header = new FrameHeader(serviceType.ServiceTypeIdentifier, (ushort)_body.Length);
            _serviceType = serviceType;
        }
        #endregion

        public byte[] ToBytes()
        {
            var data = new byte[_header.TotalLength];
            _header.WriteToByteArray(data, 0);
            _body.CopyTo(data, _header.Size);
            return data;
        }

        private IServiceType GetServiceTypeFromBody()
        {
            if (_body == null || _body.Length == 0)
                return null;
            switch (Header.ServiceTypeIdentifier)
            {
                case ServiceTypeIdentifier.CONNECT_REQUEST:
                    return ConnectionRequest.Parse(_body, 0);
                case ServiceTypeIdentifier.CONNECT_RESPONSE:
                    return ConnectionResponse.Parse(_body, 0);
                case ServiceTypeIdentifier.TUNNELING_REQ:
                    return TunnelingRequest.Parse(_body, 0, _body.Length);
                case ServiceTypeIdentifier.DISCONNECT_REQ:
                    return DisconnectRequest.Parse(_body, 0);
                case ServiceTypeIdentifier.DISCONNECT_RES:
                    return DisconnectResponse.Parse(_body, 0);
                case ServiceTypeIdentifier.CONNECTIONSTATE_RESPONSE:
                    return ConnectionStateResponse.Parse(_body, 0);
                case ServiceTypeIdentifier.TUNNELING_ACK:
                    return TunnelingAcknowledgement.Parse(_body, 0);
                case ServiceTypeIdentifier.OBJECTSERVER:
                    return ObjectServerMessage.Parse(_body, 0, _body.Length);
                default:
                    return UnknownService.Parse(new Common.IO.IndividualEndianessBinaryReader(_body), Header.ServiceTypeRaw, _body.Length);
            }
        }

        #region Static Parsing
        public static KnxNetIPFrame Parse(byte[] data, int offset = 0)
        {
            if (data == null)
                throw new ArgumentNullException("data is empty");
            if (data.Length - offset < 6)
                throw BufferSizeException.TooSmall("KNXNetIP Frame");

            var header = FrameHeader.Parse(data, offset);
            var body = new byte[header.TotalLength - header.Size];

            data.Slice(body, header.Size + offset, 0, body.Length);
            return new KnxNetIPFrame(header, body);
        }

        public static bool TryParse(out KnxNetIPFrame frame, byte[] data, int offset = 0)
        {
            frame = null;
            try
            {
                frame = Parse(data, offset);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
