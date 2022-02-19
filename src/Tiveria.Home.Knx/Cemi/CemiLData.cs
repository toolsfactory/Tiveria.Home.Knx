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
    /// | Code   | Length |  Information    |        |        |                 |                 | Length | (TPCI/APCI data)|
    /// +--------+--------+-----------------+--------+--------+--------+--------+--------+--------+--------+-----------------+
    /// </code>
    ///
    /// <list type="bullet">
    /// <item>
    /// <term>Add.Info Length</term>
    /// <description>0x00 - no additional info</description>
    /// </item>
    /// <item>
    /// <term>Control Field 1</term>
    /// <description>see the bit structure at <see cref="ControlField1"/></description>
    /// </item>
    /// <item>
    /// <term>Control Field 2</term>
    /// <description>see the bit structure at <see cref="ControlField2"/></description>
    /// </item>
    /// <item>
    /// <term>Source Address</term>
    /// <description>0x0000 - filled in by router/gateway with its source address which is part of the KNX subnet</description>
    /// </item>
    /// <item>
    /// <term>Data Length</term>
    /// <description>Number of bytes of data in the APDU excluding the TPCI/APCI bits</description>
    /// </item>
    /// <item>
    /// <term>APDU</term>
    /// <description>Application Protocol Data Unit - the actual payload including transport protocol control information (TPCI), application protocol control information (APCI) and data passed as an argument from higher layers of the KNX communication stack</description>
    /// </item>
    /// </list>
    /// </summary>
    public class CemiLData : ICemiMessage
    {
        private List<AdditionalInformationField> _additionalInfoFields = new();

        /// <summary>
        /// Size of the message when serialized to a byte array
        /// </summary>
        public int Size { get; init; }
        /// <summary>
        /// The <see cref="MessageCode"/> of the cEMI message
        /// </summary>
        public MessageCode MessageCode { get; init; }
        /// <summary>
        /// Length of the AdditionalInfo block when serialized
        /// </summary>
        public byte AdditionalInfoLength { get; init; } = 0;
        /// <summary>
        /// The <see cref="AdditionalInformationField"/> list
        /// </summary>
        public IReadOnlyList<AdditionalInformationField> AdditionalInfoFields { get; init; }
        /// <summary>
        /// Address from where the cEMI message is sent
        /// </summary>
        public IndividualAddress SourceAddress { get; init; }
        /// <summary>
        /// Whether an <see cref="IndividualAddress"/> or a <see cref="GroupAddress"/> as destination
        /// </summary>
        public IKnxAddress DestinationAddress { get; init; }
        /// <summary>
        /// Field with the flags for frame type, priority, and so on
        /// </summary>
        public ControlField1 ControlField1 { get; init; }
        /// <summary>
        /// Field with flags for destination address type, hop count and extended frame format
        /// </summary>
        public ControlField2 ControlField2 { get; init; }
        /// <summary>
        /// All the TPCI control flags
        /// </summary>
        public Tpci Tpci { get; init; }
        /// <summary>
        /// The application layer data structure
        /// </summary>
        public Apdu? Apdu { get; init; }

        /// <summary>
        /// Creates a new instance and initializes all fields with the provided parameters
        /// </summary>
        /// <param name="messageCode"></param>
        /// <param name="additionalInfoFields"></param>
        /// <param name="srcAddress"></param>
        /// <param name="dstAddress"></param>
        /// <param name="controlField1"></param>
        /// <param name="controlField2"></param>
        /// <param name="tpci"></param>
        /// <param name="apdu"></param>
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

        /// <summary>
        /// Creates a new instance of the CemiLData class. The <see cref="AdditionalInfoFields"/> and the <see cref="Tpci"/> property are initilized as 0/empty.
        /// </summary>
        /// <param name="messageCode"></param>
        /// <param name="srcAddress"></param>
        /// <param name="dstAddress"></param>
        /// <param name="controlField1"></param>
        /// <param name="controlField2"></param>
        /// <param name="apdu"></param>
        public CemiLData(MessageCode messageCode, IndividualAddress srcAddress, IKnxAddress dstAddress, ControlField1 controlField1,
            ControlField2 controlField2, Apdu apdu)
            : this(messageCode, new List<AdditionalInformationField>(), srcAddress, dstAddress, controlField1, controlField2, new Tpci(), apdu)
        { }

        /// <inheritdoc/>
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

            return $"MC {MessageCode}, {fields}, C1 {ControlField1}, C2 {ControlField2}, Src {SourceAddress}, Dst {DestinationAddress}, Tpci {Tpci}, Apdu {Apdu}";
        }
    }
}