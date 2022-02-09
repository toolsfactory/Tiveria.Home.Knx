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

using Tiveria.Common.IO;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.Cemi
{
    /// <summary>
    /// Base class handling the core CEMI Frame structures.
    /// For details please read: "03_06_03_EMI_IMI V01.03.03 AS.PDF" chapter 4 from KNX Association
    /// <code>
    /// +--------+--------+-----------------+-----------------+
    /// | byte 1 | byte 2 | byte 3 - n      | n bytes         |
    /// +--------+--------+-----------------+-----------------+
    /// |  Msg   |Add.Info|  Additional     |  Service        |
    /// | Code   | Length |  Information    |  Information    |
    /// +--------+--------+-----------------+-----------------+
    /// </code>
    ///
    /// MsgCode : 
    /// A code describing the message transported in this cEMI Frame
    /// See <seealso cref="Tiveria.Home.Knx.Cemi.MessageCode"/> for all supported codes and their meaning
    /// 
    ///  Add.Info Length : 
    ///  0x00      = no additional info
    ///  0x01-0xfe = number of total bytes in Service Information block
    ///  0xff      = reserved
    ///                    
    /// Additional Information : 
    /// N repetitions of a Additional Information Fields.
    /// See <seealso cref="Tiveria.Home.Knx.Cemi.AdditionalInformationField"/> for more details on this substructure
    /// </summary>
    public abstract class CemiBase : ICemiMessage
    {
        #region private fields
        protected List<AdditionalInformationField> _additionalInfoFields = new ();
        #endregion

        #region public properties
        public MessageCode MessageCode { get; protected set; } = MessageCode.LDATA_REQ;
        public byte AdditionalInfoLength { get; protected set; } = 0;
        public IReadOnlyList<AdditionalInformationField> AdditionalInfoFields => _additionalInfoFields;
        public int Size { get; protected set; } = 0;
        #endregion

        #region constructors
        protected CemiBase(MessageCode messageCode)
        {
            VerifyMessageCode(messageCode);
            MessageCode = messageCode;
        }

        protected CemiBase(BigEndianBinaryReader br)
        {
            ParseBuffer(br);
        }
        #endregion
        
        #region verifying elements
        protected abstract bool VerifyBufferSize(BigEndianBinaryReader br);

        protected abstract void VerifyMessageCode(MessageCode messageCode);

        protected abstract void VerifyAdditionalLengthInfo(byte length);
        #endregion

        #region parsing the buffer
        protected virtual void ParseBuffer(BigEndianBinaryReader br)
        {
            ParseMessageCode(br);
            ParseAdditinalInfoLength(br);
            ParseAdditionalInfo(br);
            ParseServiceInformation(br);
            CalculateSize();
        }

        protected void ParseMessageCode(BigEndianBinaryReader br)
        {
            var messageCode = br.ReadByte();
            if (Enum.IsDefined(typeof(MessageCode), messageCode))
            {
                MessageCode = (MessageCode)messageCode;
                VerifyMessageCode(MessageCode);
            }
            else
            {
                throw KnxBufferFieldException.TypeUnknown("MessageCode", messageCode);
            }
        }

        protected void ParseAdditinalInfoLength(BigEndianBinaryReader br)
        {
            AdditionalInfoLength = br.ReadByte();
            VerifyAdditionalLengthInfo(AdditionalInfoLength);
        }

        protected void ParseAdditionalInfo(BigEndianBinaryReader br)
        {
            if (AdditionalInfoLength == 0)
                return;

            var count = 0;
            while (count < AdditionalInfoLength)
            {
                var field = AdditionalInformationField.Parse(br);
                _additionalInfoFields.Add(field);
                count += field.Size;
            }
        }

        protected abstract void ParseServiceInformation(BigEndianBinaryReader br);

        public abstract void CalculateSize();  
        #endregion

        public byte[] ToBytes()
        {
            var data = new byte[Size];
            var writer = new BigEndianBinaryWriter(new MemoryStream(data));
            Write(writer);
            return data;
        }

        public abstract string ToDescription(int padding);

        public abstract void Write(BigEndianBinaryWriter writer);
    }
}