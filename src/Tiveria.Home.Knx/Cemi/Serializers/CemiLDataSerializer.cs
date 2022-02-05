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

using Tiveria.Common.IO;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.Cemi.Serializers
{
    /// <summary>
    /// Class representing a CEMI Frame for L_Data services.
    /// For details please read: "03_06_03_EMI_IMI V01.03.03 AS.PDF" chapter 4 from KNX Association
    /// <code>
    /// +--------+--------+-----------------+--------+--------+--------+--------+--------+--------+--------+-----------------+
    /// | byte 1 | byte 2 | byte 3 - n      | byte   | byte   | byte   | byte   | byte   | byte   | byte   | n bytes         |
    /// +--------+--------+-----------------+--------+--------+--------+--------+--------+--------+--------+-----------------+
    /// |  Msg   |Add.Info|  Additional     |Ctrl 1  | Ctrl 2 | Source Address  | Dest. Address   |  NPDU  |      APDU       |
    /// | Code   | Length |  Information    |        |        |                 |                 | Length | (TPCI/APCI&data)|
    /// +--------+--------+-----------------+--------+--------+--------+--------+--------+--------+--------+-----------------+
    /// </code>
    ///
    ///  Add.Info Length = 0x00 - no additional info
    ///  Control Field 1 = see the bit structure above
    ///  Control Field 2 = see the bit structure above
    ///  Source Address  = 0x0000 - filled in by router/gateway with its source address which is
    ///                    part of the KNX subnet
    ///  Dest. Address   = KNX group or individual address (2 byte)
    ///  Data Length     = Number of bytes of data in the APDU excluding the TPCI/APCI bits
    ///  APDU            = Application Protocol Data Unit - the actual payload including transport
    ///                    protocol control information (TPCI), application protocol control
    ///                    information (APCI) and data passed as an argument from higher layers of
    ///                    the KNX communication stack
    /// </summary>
    /// <remarks> 
    /// This class does NOT support additional info!
    /// </remarks>
    public class CemiLDataSerializer : CemiSerializerBase<CemiLData>
    {
        public override CemiLData Deserialize(BigEndianBinaryReader reader, int size)
        {
            if (size > reader.Available)
                throw new ArgumentOutOfRangeException(nameof(size), size, $"Size bigger than Data available {reader.Available}");

            var msgCode = reader.ReadByteEnum<MessageCode>("Cemi.MessageCode");
            var additionalInfoLength = reader.ReadByte();
            var additionalInfoFields = ReadAdditionalInfoFields(reader, additionalInfoLength);
            var controlField1 = new ControlField1(msgCode, reader.ReadByte());
            var controlField2 = new ControlField2(reader.ReadByte());
            var srcAddr = ReadSourceAddress(reader);
            var dstAddr = ReadDestinationAddress(reader, controlField2.DestinationAddressType == BaseTypes.AddressType.GroupAddress);
            var npduLength = reader.ReadByte();
            var data = reader.ReadBytes(npduLength + 1); // TPCI Octet not included in length field!
            var tpci = Tpci.Parse(data[0]);
            var apdu = (npduLength == 0) ? null : Apdu.Parse(data);

            return new CemiLData(msgCode, additionalInfoFields, srcAddr, dstAddr, controlField1, controlField2, tpci, apdu);
        }

        public override void Serialize(CemiLData cemiMessage, BigEndianBinaryWriter writer)
        {
            writer.Write((byte)cemiMessage.MessageCode);
            writer.Write((byte)cemiMessage.AdditionalInfoLength);
            foreach (var info in cemiMessage.AdditionalInfoFields) 
                info.Write(writer);
            writer.Write((byte)cemiMessage.ControlField1.ToByte(cemiMessage.MessageCode));
            writer.Write((byte)cemiMessage.ControlField2.ToByte());
            cemiMessage.SourceAddress.Write(writer);
            cemiMessage.DestinationAddress.Write(writer);
            if (cemiMessage.Apdu == null)
            {
                writer.Write((byte)0); // Field with TPCI mask not counted
                writer.Write(cemiMessage.Tpci.ToByte());
            }
            else
            {
                if (cemiMessage.Tpci.PacketType != PacketType.Data)
                    throw new KnxBufferException("Cannot serialize cemi with Tpci Flag 'Control' and APDU Data!");
                var apdu = cemiMessage.Apdu.ToBytes();
                apdu[0] = (byte)((apdu[0] & 0b000000_11) | (cemiMessage.Tpci.ToByte() & 0b111111_00));
                writer.Write((byte)(cemiMessage.Apdu.Size - 1)); // Field with TPCI mask and upper apci bits not counted 
                writer.Write(apdu); // Apci structure includes TPCI and therefore Size is NPDULength+1
            }
        }
    }
}
