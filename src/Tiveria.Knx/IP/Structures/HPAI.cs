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
using System.Net;
using Tiveria.Knx.Structures;
using Tiveria.Knx.IP.Utils;
using Tiveria.Knx.Exceptions;
using Tiveria.Common.Extensions;
using Tiveria.Common.IO;

namespace Tiveria.Knx.IP.Structures
{
    /// <summary>
    /// Represents the Host Protocol Address information block of the KnxNetIP specification.
    /// </summary>
    /// <code>
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// | byte 1 | byte 2 | byte 3 | byte 4 | byte 5 | byte 6 | byte 7 | byte 8 |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// |  Size  |Endpoint|        Endpoint IP Address        |  Endpoint Port  |
    /// |  (8)   | Type   |                                   |                 |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// </code>
    public class Hpai : StructureBase
    {
        #region private fields
        private static byte HPAI_SIZE = 8;
        private IPAddress _ip;
        private ushort _port;
        private HPAIEndpointType _endpointType;
        #endregion

        #region properties
        public IPAddress Ip { get => _ip; }
        public ushort Port { get => _port; }
        public HPAIEndpointType EndpointType { get => _endpointType; }
        #endregion

        #region constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointType"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public Hpai(HPAIEndpointType endpointType, IPAddress address, ushort port)
        {
            if (address != null)
            {
                if (address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                    throw new ArgumentOutOfRangeException("not an IPv4 address");
                if (IPAddress.IsLoopback(address))
                    throw new ArgumentException("IPv4 loopback address");
            } else {
                address = IPAddress.Parse("0.0.0.0");
                port = 0;
            }
            _endpointType = endpointType;
            _ip = address;
            _port = port;
            _structureLength = HPAI_SIZE;
        }

        protected Hpai(BinaryReaderEx br)
        {
            if (br.Available < Hpai.HPAI_SIZE)
                throw BufferSizeException.TooSmall("HPAI");
            ParseStructureSize(br);
            ParseEndpointType(br);
            ParseIPAddress(br);
            ParsePort(br);
        }
        #endregion

        #region parsing
        private void ParseStructureSize(BinaryReaderEx br)
        {
            _structureLength = br.ReadByte();
            if (_structureLength != Hpai.HPAI_SIZE)
                throw BufferFieldException.WrongValue("HPAI.Size", Hpai.HPAI_SIZE, _structureLength);
        }

        private void ParseEndpointType(BinaryReaderEx br)
        {
            var endpointType = br.ReadByte();
            if (!Enum.IsDefined(typeof(HPAIEndpointType), endpointType))
                throw BufferFieldException.TypeUnknown("EndpointType", endpointType);
            _endpointType = (HPAIEndpointType)endpointType;
        }

        private void ParseIPAddress(BinaryReaderEx br)
        {
            var ipbytes = br.ReadBytes(4);
            _ip = new IPAddress(ipbytes);
        }

        private void ParsePort(BinaryReaderEx br)
        {
            _port = br.ReadU2be();
        }
        #endregion

        public override void WriteToByteArray(byte[] buffer, int offset = 0)
        {
            base.WriteToByteArray(buffer, offset);
            buffer[offset + 0] = Hpai.HPAI_SIZE;
            buffer[offset + 1] = (byte)EndpointType;
            Ip.GetAddressBytes().CopyTo(buffer, offset + 2);
            buffer[offset + 6] = (byte)(Port >> 8);
            buffer[offset + 7] = (byte)Port;
        }

        public override string ToString()
        {
            return $"";
        }

        public static Hpai Parse(BinaryReaderEx br)
        {
            return new Hpai(br);
        }

        public static Hpai Parse(byte[] buffer, int offset)
        {
            return new Hpai(new BinaryReaderEx(buffer, offset));
        }
    }
}
