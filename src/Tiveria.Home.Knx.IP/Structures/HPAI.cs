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

using System.Net;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Common.IO;

namespace Tiveria.Home.Knx.IP.Structures
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
    public class Hpai : KnxDataElement
    {
        public static readonly byte STRUCTURE_SIZE = 8;

        #region properties
        public IPAddress Ip { get; init; }
        public ushort Port { get; init; }
        public HPAIEndpointType EndpointType { get; init; }
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
            }
            else
            {
                address = IPAddress.Parse("0.0.0.0");
                port = 0;
            }
            EndpointType = endpointType;
            Ip = address;
            Port = port;
            Size = STRUCTURE_SIZE;
        }
        #endregion

        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write(Hpai.STRUCTURE_SIZE);
            writer.Write((byte)EndpointType);
            writer.Write(Ip.GetAddressBytes());
            writer.Write((byte)(Port >> 8));
            writer.Write((byte)Port);
        }

        public override string ToString()
        {
            return $"";
        }

        public static Hpai Parse(BigEndianBinaryReader reader)
        {
            var size = reader.ReadSizeAndCheck("HPAI", STRUCTURE_SIZE);
            var endpointType = reader.ReadByteEnum<HPAIEndpointType>("HPAI.EndpointType");
            var ip = new IPAddress(reader.ReadBytes(4));
            var port = reader.ReadUInt16();
            return new Hpai(endpointType, ip, port);
        }
    }

}
