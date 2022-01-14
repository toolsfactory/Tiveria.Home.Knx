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

using Tiveria.Home.Knx.Exceptions;
using Tiveria.Home.Knx.Adresses;
using Tiveria.Common.IO;
using Tiveria.Common.Extensions;

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
    public class CemiLDataOld : CemiBase, ICemiMessage
    {
        private static int MESSAGEMINLENGTH = 9;

        #region public properties
        public IndividualAddress SourceAddress { get; protected set; } = IndividualAddress.Empty();
        public IKnxAddress DestinationAddress { get; protected set; } = GroupAddress.Empty();
        public ControlField1 ControlField1 { get; protected set; } = new ControlField1(MessageCode.LDATA_REQ);
        public ControlField2 ControlField2 { get; protected set; } = new ControlField2();
        public Apci Apci { get; protected set; }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new cEMI L_Data Frame
        /// </summary>
        /// <param name="br">BigEndianBinaryReader with the underlying buffer the frame is parsed from</param>
        protected CemiLDataOld(BigEndianBinaryReader br)
            : base(br)
        { }

        public CemiLDataOld(MessageCode messageCode, IndividualAddress srcAddress, IKnxAddress dstAddress, byte[] tpdu, 
                         Priority priority, bool repeat = true, BroadcastType broadcast = BroadcastType.Normal, bool ack = false, int hopCount = 6)
            : base(messageCode)
        {
            VerifyPayload(tpdu);
            SourceAddress = srcAddress;
            DestinationAddress = dstAddress;
            Apci = Apci.Parse(tpdu);
            // _payload = (byte[])tpdu.Clone();
            // _apci = Apci.Parse(_payload);
            ControlField1 = new ControlField1(MessageCode, false, priority, repeat, broadcast, ack);
            ControlField2 = new ControlField2(DestinationAddress.IsGroupAddress(), hopCount, 0);
            Size = MESSAGEMINLENGTH + tpdu.Length;  // AdditionalInfoLength is 0 for LData in this Message
        }

        public CemiLDataOld(MessageCode messageCode, IndividualAddress srcAddress, IKnxAddress dstAddress, byte[] tpdu,
                         Priority priority, ConfirmType confirm)
            : this(messageCode, srcAddress, dstAddress, tpdu, priority, true, BroadcastType.Normal, false, 6)
        {
            //  overwrite controlField1 now including confirm
            ControlField1 = new ControlField1(messageCode, false, priority, true, BroadcastType.Normal, false, confirm);
        }
        #endregion

        #region private methods
        protected virtual void VerifyPayload(byte[] tpdu)
        {
            if (tpdu == null)
                throw new ArgumentNullException("TPDU must not be null in cemi frame");
            if (tpdu.Length > 16)
                throw BufferSizeException.TooBig("TPDU is bigger than 16 bytes");
        }

        protected override void VerifyMessageCode(MessageCode messageCode)
        {
            if (messageCode != MessageCode.LDATA_REQ &&
                messageCode != MessageCode.LDATA_CON &&
                messageCode != MessageCode.LDATA_IND)
                throw BufferFieldException.WrongValue("MessageCode", "LDATA*", messageCode.ToString());
        }

        protected override bool VerifyBufferSize(BigEndianBinaryReader br)
        {
            return br.Size - br.Position >= MESSAGEMINLENGTH;
        }

        protected override void VerifyAdditionalLengthInfo(byte length)
        {
//            if (length != 0)
//                throw BufferFieldException.WrongValue("AdditionalInfoLength", 0, length);
        }

        #region parsing the buffer
        protected override void ParseServiceInformation(BigEndianBinaryReader br)
        {
            ParseControlField1(br);
            ParseControlField2(br);
            ParseSourceAddress(br);
            ParseDestinationAddress(br);
            ParseAPDU(br);
        }

        protected void ParseControlField1(BigEndianBinaryReader br)
        {
            var ctrl1 = br.ReadByte();
            ControlField1 = new ControlField1(MessageCode, ctrl1);
        }

        protected void ParseControlField2(BigEndianBinaryReader br)
        {
            var ctrl = br.ReadByte();
            ControlField2 = new ControlField2(ctrl);
        }

        protected void ParseSourceAddress(BigEndianBinaryReader br)
        {
            var src = br.ReadUInt16();
            SourceAddress = new IndividualAddress(src);
        }

        protected void ParseDestinationAddress(BigEndianBinaryReader br)
        {
            var dst = br.ReadUInt16();
            if (ControlField2.DestinationAddressType == AddressType.GroupAddress)
                DestinationAddress = new GroupAddress(dst);
            else
                DestinationAddress = new IndividualAddress(dst);
        }

        protected void ParseAPDU(BigEndianBinaryReader br)
        {
            // read length information and increase lenght by one due to TPCI/APCI encoding
            var len = br.ReadByte() + 1;
            if (br.Available < len)
                throw BufferSizeException.TooBig("Cemi Frame - TCPI Data");
            var payload = br.ReadBytes(len);
            Apci = Apci.Parse(payload);
        }
        #endregion

        /*        #region creating the buffer
                public override void WriteToByteArray(byte[] buffer, int offset = 0)
                {
                    base.WriteToByteArray(buffer, offset);
                    buffer[offset] = (byte)_messageCode;
                    buffer[offset + 1] = _additionalInfoLength;
                    buffer[offset + 2] = _controlField1.RawData;
                    buffer[offset + 3] = _controlField2.RawData;
                    _sourceAddress.WriteBytes(buffer.AsSpan(offset+4));
                    _destinationAddress.WriteBytes(buffer.AsSpan(offset+6));
                    buffer[offset + 8] = (byte) (_payload.Length - 1);
                    _payload.CopyTo(buffer, offset + 9);
                }
                #endregion
        */

        public override void CalculateSize()
        {
            Size = MESSAGEMINLENGTH + AdditionalInfoLength + Apci.Size;
        }

        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write((byte) Size);
            writer.Write((byte)AdditionalInfoLength);
            foreach(var info in AdditionalInfoFields)
                info.Write(writer);
            writer.Write((byte)ControlField1.RawData);
            writer.Write((byte)ControlField2.RawData);
            SourceAddress.Write(writer);
            DestinationAddress.Write(writer);
            Apci.Write(writer);
        }

        #endregion

        public override string ToDescription(int padding = 4)
        {
            return ($"CemiLData: Source = {SourceAddress}, Destination = {DestinationAddress}" + Environment.NewLine +
                ControlField1.ToDescription(padding + 4) + Environment.NewLine +
                ControlField2.ToDescription(padding + 4) +Environment.NewLine +
                Apci.ToString().AddPrefixSpaces(padding + 4))
                .AddPrefixSpaces(padding);
        }

        #region static methods
        // ToDo: Replace with more generic solution
        public static CemiLDataOld CreateReadRequestCemi(IndividualAddress srcAddress, IKnxAddress dstAddress, Priority priority = Priority.Normal)
        {
            return new CemiLDataOld(MessageCode.LDATA_REQ, srcAddress, dstAddress, new byte[2] { 0, 0 }, priority);
        }

        // ToDo: Replace with more generic solution
        public static CemiLDataOld CreateReadAnswerCemi(IndividualAddress srcAddress, IKnxAddress dstAddress, byte[] data, Priority priority = Priority.Normal)
        {
            return new CemiLDataOld(MessageCode.LDATA_IND, srcAddress, dstAddress, data , priority);
        }

        /// <summary>
        /// Parses a part of a buffer and creates a CemiLData class from it
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static CemiLDataOld Parse(byte[] buffer, int offset, int length)
        {
            return new CemiLDataOld(new BigEndianBinaryReader(buffer, offset, length));
        }

        public static CemiLDataOld Parse(BigEndianBinaryReader br)
        {
            return new CemiLDataOld(br);
        }

        public static bool TryParse(byte[] buffer, int offset, int length, out CemiLDataOld? cemildata)
        {
            bool result = false;
            try
            {
                cemildata = new CemiLDataOld(new BigEndianBinaryReader(buffer, offset, length));
                result = true;
            }
            catch
            {
                cemildata = null;
                result = false;
            }
            return result;
        }

        public static bool TryParse(BigEndianBinaryReader br, out CemiLDataOld? cemildata)
        {
            bool result = false;
            try
            {
                cemildata = new CemiLDataOld(br);
                result = true;
            }
            catch
            {
                cemildata = null;
                result = false;
            }
            return result;
        }
        #endregion
    }

}