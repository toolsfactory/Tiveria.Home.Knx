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
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    Linking this library statically or dynamically with other modules is
    making a combined work based on this library. Thus, the terms and
    conditions of the GNU General Public License cover the whole
    combination.
*/

using Tiveria.Common.IO;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.IP
{
    public class KnxNetIPFrame : IKnxNetIPFrame
    {
        public int Size { get; private set; }

        public FrameHeader FrameHeader { get; private set; }

        public IKnxNetIPService Service { get; private set; }

        public KnxNetIPFrame(FrameHeader header, IKnxNetIPService service)
        {
            Service = service;
            FrameHeader = header;
            Size = FrameHeader.TotalLength;
        }
        public KnxNetIPFrame(IKnxNetIPService service)
            :this (new FrameHeader(service), service)
        { }

        public KnxNetIPFrame(KnxNetIPVersion version, IKnxNetIPService service)
            : this (new FrameHeader(version, service), service)
        { }

        public byte[] ToBytes()
        {
            var data = new byte[Size];
            var writer = new BigEndianBinaryWriter(new MemoryStream(data));
            Write(writer);
            return data;
        }

        public void Write(BigEndianBinaryWriter writer)
        {
            FrameHeader.Write(writer);
            var serializer = KnxNetIPServiceSerializerFactory.Instance.Create(Service.ServiceTypeIdentifier);
            serializer.Serialize(Service, writer);
        }

        public static KnxNetIPFrame Parse (byte[]buffer)
        {
            var reader  = new BigEndianBinaryReader(buffer);
            var header  = FrameHeader.Parse(reader);
            var parser  = KnxNetIPServiceSerializerFactory.Instance.Create(header.ServiceTypeIdentifier);
            var service = parser.Deserialize(reader);
            return new KnxNetIPFrame(header, service);
        }

        public static bool TryParse(byte[] buffer, out KnxNetIPFrame? frame)
        {
            try
            {
                frame = Parse(buffer);
                return true;
            }
            catch
            {
                frame = null;
                return false;
            }
        }
    }
}