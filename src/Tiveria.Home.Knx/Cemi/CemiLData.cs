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

using System.Text;
using Tiveria.Common.Extensions;
using Tiveria.Home.Knx.Primitives;

namespace Tiveria.Home.Knx.Cemi
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

    public class CemiLData : ICemiMessage
    {
        private List<AdditionalInformationField> _additionalInfoFields = new();

        public int Size { get; init; }
        public MessageCode MessageCode { get; init; }
        public byte AdditionalInfoLength { get; init; } = 0;
        public IReadOnlyList<AdditionalInformationField> AdditionalInfoFields { get; init; }
        public IndividualAddress SourceAddress { get; init; }
        public IKnxAddress DestinationAddress { get; init; }
        public ControlField1 ControlField1 { get; init; }
        public ControlField2 ControlField2 { get; init; }
        public Tpci Tpci { get; init; }
        public Apdu? Apdu { get; init; }

        public CemiLData(MessageCode messageCode, IReadOnlyList<AdditionalInformationField> additionalInfoFields, IndividualAddress srcAddress, IKnxAddress dstAddress, ControlField1 controlField1,
            ControlField2 controlField2, Tpci tpci, Apdu? apdu)
        {
            MessageCode = messageCode;
            AdditionalInfoFields = additionalInfoFields;
            SourceAddress = srcAddress;
            DestinationAddress = dstAddress;
            ControlField1 = controlField1;
            ControlField2 = controlField2;
            Tpci = tpci;
            Apdu = apdu;

            foreach(var item in additionalInfoFields)
                AdditionalInfoLength += (byte)item.Size;

            Size = 9 + AdditionalInfoLength + (Apdu!=null ? Apdu.Size : 1);
        }

        public override string ToString()
        {
            StringBuilder fields = new();
            if (AdditionalInfoFields.Count > 0)
            {
                fields.Append("AdditionalInfoFields: length=");
                fields.Append(AdditionalInfoLength);
                fields.Append(", ");
                foreach (var item in AdditionalInfoFields)
                {
                    fields.Append("[");
                    fields.Append(item.InfoType);
                    fields.Append("/");
                    fields.Append(BitConverter.ToString(item.Information));
                    fields.Append("] ,");
                }
            }

            return $"CemiLData: MC: {MessageCode}, {fields}, Ctrl1_ {ControlField1}, Ctrl2_ {ControlField2}, Src:{SourceAddress}, Dst: {DestinationAddress}, Tpci: {Tpci}, Apdu: {Apdu}";
        }
    }
}