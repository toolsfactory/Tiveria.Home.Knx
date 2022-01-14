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

using System.Net;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.Adresses;
using Tiveria.Common.IO;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.IP.Structures
{
    /// <summary>
    /// Represents the Device Information DIB block described in chapter 7.5.4.2 of the spec 3/8/2 KnxIPNet core.
    /// </summary>
    /// <code>
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// | byte 1 | byte 2 | byte 3 | byte 4 | byte 5 | byte 6 | byte 7 | byte 8 |
    /// +--------+--------+--------+--------+--------+--------+-----------------+
    /// |  Size  |Descript| Medium | Status | Individual Addr | Proj Installer  |  
    /// |  (8)   | Type   |        |        |                 | Identifier      |
    /// +--------+--------+--------+--------+--------+--------+-----------------+
    /// 
    /// +--------+--------+--------+--------+--------+--------+-----------------+
    /// | bytes 9 - 14    | bytes 15 - 18   | bytes 19 - 24   | bytes 25 - 54   |
    /// +--------+--------+-----------------+-----------------+-----------------+
    /// |  Device serial  | Device Routing  | Device MAC      | Device Friendly |
    /// |  Number         | Multicast Addr  | Address         | Name            |
    /// +--------------------------+--------------------------+-----------------+
    /// 
    /// Project Installer
    /// </code>
    public class DeviceInfoDIB : KnxDataElement
    {
        public static readonly byte DEVICEINFODIB_SIZE = 0x36;
        public static readonly byte DIB_TYPE = 0x01;

        public KnxMediumType MediumType { get; init; }
        public DeviceStatus Status { get; init; }
        public IndividualAddress IndividualAddress { get; init; }
        public ushort ProjectInstallerId { get; init; } = 0;
        public long SerialNumber { get; init; } = 0;
        public IPAddress RoutingMulticastIP { get; init; }
        public byte[] MAC { get; init; } = new byte[6];
        public string FriendlyName { get; init; }

        public DeviceInfoDIB(KnxMediumType medium, DeviceStatus status, IndividualAddress individualAddress,
                             ushort projectInstallerId, long serialNumber, IPAddress routingMulticastIP,
                             byte[] mac, string friendlyName)
        {
            if (serialNumber > 0xffff_ffff_ffff)
                throw new ArgumentOutOfRangeException(nameof(serialNumber));
            MediumType = medium;
            Status = status;
            IndividualAddress = individualAddress;
            ProjectInstallerId = projectInstallerId;
            SerialNumber = serialNumber;
            RoutingMulticastIP = routingMulticastIP;
            MAC = mac;
            FriendlyName = friendlyName.Length < 30 ? friendlyName : friendlyName.Substring(0, 29);
            Size = DEVICEINFODIB_SIZE;
        }

        void WriteFriendlyName(BigEndianBinaryWriter writer)
        {
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(FriendlyName);
            writer.Write(buffer);
            for (int i = 0; i < 30 - (buffer.Length); i++)
                writer.Write((byte)0);
        }

        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write(DEVICEINFODIB_SIZE);
            writer.Write(DIB_TYPE);
            writer.Write((byte)MediumType);
            writer.Write((byte)Status);
            IndividualAddress.Write(writer);
            writer.Write(ProjectInstallerId);
            WriteSerialNumber(writer);
            writer.Write(RoutingMulticastIP.GetAddressBytes());
            writer.Write(MAC);
            WriteFriendlyName(writer);
        }

        private void WriteSerialNumber(BigEndianBinaryWriter writer)
        {
            var ser_high = (SerialNumber & 0xffff_0000_0000) >> 32;
            var ser_mid = (SerialNumber & 0x0000_ffff_0000) >> 16;
            var ser_low = (SerialNumber & 0x0000_0000_ffff);
            writer.Write((ushort)ser_high); 
            writer.Write((ushort)ser_mid);
            writer.Write((ushort)ser_low);
        }

        public static DeviceInfoDIB Parse(BigEndianBinaryReader reader)
        {
            return (new DeviceInfoDIBParser()).Parse(reader);
        }

        public static DeviceInfoDIB Parse(byte[] data)
        {
            return Parse(new BigEndianBinaryReader(new MemoryStream(data)));
        }
    }

    internal class DeviceInfoDIBParser
    {
        internal DeviceInfoDIB Parse(BigEndianBinaryReader reader)
        {
            if (reader.Available < DeviceInfoDIB.DEVICEINFODIB_SIZE)
                throw BufferSizeException.TooSmall("DEVICEINFODIB");

            var size = ParseSize(reader);
            var type = ParseType(reader);
            var medium = reader.ReadByte();
            var status = reader.ReadByte();
            var individual = new IndividualAddress(reader.ReadUInt16());
            var installerid = reader.ReadUInt16();
            var serial = ParseSerialNumber(reader);
            var multicastIP = ParseIPAddress(reader);
            var mac = reader.ReadBytes(6);
            var friendly = ParseFriendlyName(reader);

            return new DeviceInfoDIB((KnxMediumType)medium, (DeviceStatus)status, individual, installerid, serial, multicastIP, mac, friendly);
        }

        private static long ParseSerialNumber(BigEndianBinaryReader reader)
        {
            long ser_high = reader.ReadUInt16(); 
            long ser_mid =  reader.ReadUInt16();
            long ser_low =  reader.ReadUInt16();

            return (ser_high << 32) + (ser_mid << 16) + ser_low;
        }

        private byte ParseType(BigEndianBinaryReader reader)
        {
            var type = reader.ReadByte();
            if (type != DeviceInfoDIB.DIB_TYPE)
                throw BufferFieldException.WrongValue("DIBTYPE", DeviceInfoDIB.DIB_TYPE, type);
            return type;
        }

        private byte ParseSize(BigEndianBinaryReader reader)
        {
            var size = reader.ReadByte();
            if (size != DeviceInfoDIB.DEVICEINFODIB_SIZE)
                throw BufferSizeException.WrongSize("DEVICEINFODIB", DeviceInfoDIB.DEVICEINFODIB_SIZE, size);
            return size;
        }

        private string ParseFriendlyName(BigEndianBinaryReader reader)
        {
            var data = reader.ReadBytes(30);
            return System.Text.Encoding.ASCII.GetString(data).Trim('\0');
        }

        private IPAddress ParseIPAddress(BigEndianBinaryReader br)
        {
            var ipbytes = br.ReadBytes(4);
            return new IPAddress(ipbytes);
        }
    }
}
