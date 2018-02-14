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
using Tiveria.Knx.Exceptions;
using Tiveria.Common.IO;
using Tiveria.Common.Extensions;

namespace Tiveria.Knx.Cemi
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
    /// </code>
    /// </summary>
    /// <remarks> 
    /// This class does NOT support additional info!
    /// </remarks>
    public class CemiLData : CemiBase, ICemi
    {
        private static int MESSAGEMINLENGTH = 9;

        #region private fields
        protected IndividualAddress _sourceAddress;
        protected IKnxAddress _destinationAddress;
        protected Priority _priority;
        protected ControlField1 _controlField1;
        protected ControlField2 _controlField2;
        #endregion

        #region public properties
        public IndividualAddress SourceAddress => _sourceAddress;
        public IKnxAddress DestinationAddress => _destinationAddress;
        public ControlField1 ControlField1 => _controlField1;
        public ControlField2 ControlField2 => _controlField2;
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new cEMI L_Data Frame
        /// </summary>
        /// <param name="br">BinaryReaderEx with the underlying buffer the frame is parsed from</param>
        protected CemiLData(BinaryReaderEx br)
            : base(br)
        { }

        public CemiLData(CemiMessageCode messageCode, IndividualAddress srcAddress, IKnxAddress dstAddress, byte[] tpdu, 
                         Priority priority, bool repeat = true, BroadcastType broadcast = BroadcastType.Normal, bool ack = false, int hopCount = 6)
            : base(messageCode)
        {
            VerifyPayload(tpdu);
            _additionalInfoLength = 0;
            _controlField1 = new ControlField1(_messageCode, false, priority, repeat, broadcast, ack);
            _controlField2 = new ControlField2(_destinationAddress.IsGroupAddress(), hopCount, 0);
            _sourceAddress = srcAddress;
            _destinationAddress = dstAddress;
            _payload = (byte[]) tpdu.Clone();
            _structureLength = MESSAGEMINLENGTH + _payload.Length - 1;
        }

        public CemiLData(CemiMessageCode messageCode, IndividualAddress srcAddress, IKnxAddress dstAddress, byte[] tpdu,
                         Priority priority, ConfirmType confirm)
            : this(messageCode, srcAddress, dstAddress, tpdu, priority, true, BroadcastType.Normal, false, 6)
        {
            //  overwrite controlField1 now including confirm
            _controlField1 = new ControlField1(_messageCode, false, priority, true, BroadcastType.Normal, false, confirm);
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

        protected override void VerifyMessageCode(CemiMessageCode messageCode)
        {
            if (messageCode != CemiMessageCode.LDATA_REQ &&
                messageCode != CemiMessageCode.LDATA_CON &&
                messageCode != CemiMessageCode.LDATA_IND)
                throw BufferFieldException.WrongValue("MessageCode", "LDATA*", messageCode.ToString());
        }


        protected override bool VerifyBufferSize(BinaryReaderEx br)
        {
            return br.Size - br.Position >= MESSAGEMINLENGTH;
        }

        protected override void VerifyAdditionalLengthInfo(byte length)
        {
            if (length != 0)
                throw BufferFieldException.WrongValue("AdditionalInfoLength", 0, length);
            _additionalInfoFields = new AdditionalInformationField[0];
        }

        #region parsing the buffer
        protected override void ParseServiceInformation(BinaryReaderEx br)
        {
            ParseControlField1(br);
            ParseControlField2(br);
            ParseSourceAddress(br);
            ParseDestinationAddress(br);
            ParseAPDU(br);
        }

        protected void ParseControlField1(BinaryReaderEx br)
        {
            var ctrl1 = br.ReadByte();
            _controlField1 = new ControlField1(_messageCode, ctrl1);
        }

        protected void ParseControlField2(BinaryReaderEx br)
        {
            var ctrl = br.ReadByte();
            _controlField2 = new ControlField2(ctrl);
        }

        protected void ParseSourceAddress(BinaryReaderEx br)
        {
            var src = br.ReadU2be();
            _sourceAddress = new IndividualAddress(src);
        }

        protected void ParseDestinationAddress(BinaryReaderEx br)
        {
            var dst = br.ReadU2be();
            if (_controlField2.DestinationAddressType == KnxAddressType.GroupAddress)
                _destinationAddress = new GroupAddress(dst);
            else
                _destinationAddress = new IndividualAddress(dst);
        }

        protected void ParseAPDU(BinaryReaderEx br)
        {
            // read length information and increase lenght by one due to TPCI/APCI encoding
            var len = br.ReadByte() + 1;
            if (!br.AreAvailable(len))
                throw BufferSizeException.TooBig("Cemi Frame - TCPI Data");
            _payload = br.ReadBytes(len);
        }
        #endregion

        #region creating the buffer
        public override void WriteToByteArray(byte[] buffer, int offset = 0)
        {
            base.WriteToByteArray(buffer, offset);
            buffer[offset] = (byte)_messageCode;
            buffer[offset + 1] = _additionalInfoLength;
            buffer[offset + 2] = _controlField1.RawData;
            buffer[offset + 3] = _controlField2.RawData;
            _sourceAddress.WriteToByteArray(buffer, offset + 4);
            _destinationAddress.WriteToByteArray(buffer, offset + 6);
            buffer[offset + 8] = (byte) (_payload.Length - 1);
            _payload.CopyTo(buffer, offset + 9);
        }
        #endregion
        #endregion

        public string ToDescription(int padding)
        {
            var addinfos = "";
            foreach (var info in AdditionalInfoFields)
                addinfos += info.ToDescription(padding + 4) + Environment.NewLine;

            return $"CemiLData: AdditionalInfoLength = {AdditionalInfoLength}, Source = {SourceAddress}, Destination = {DestinationAddress}, TPDU = ".AddPrefixSpaces(padding) + _payload.ToHexString() + Environment.NewLine+
                addinfos+
                ControlField1.ToDescription(padding + 4) + Environment.NewLine +
                ControlField2.ToDescription(padding + 4);
        }

        #region static methods
        /// <summary>
        /// Parses a part of a buffer and creates a CemiLData class from it
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static CemiLData Parse(byte[] buffer, int offset, int length)
        {
            return new CemiLData(new BinaryReaderEx(buffer, offset, length));
        }

        public static CemiLData Parse(BinaryReaderEx br)
        {
            return new CemiLData(br);
        }

        public static bool TryParse(out CemiLData cemildata, byte[] buffer, int offset, int length)
        {
            bool result = false;
            try
            {
                cemildata = new CemiLData(new BinaryReaderEx(buffer, offset, length));
                result = true;
            }
            catch
            {
                cemildata = null;
                result = false;
            }
            return result;
        }

        public static bool TryParse(out CemiLData cemildata, BinaryReaderEx br)
        {
            bool result = false;
            try
            {
                cemildata = new CemiLData(br);
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