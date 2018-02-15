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
using Tiveria.Common.IO;
using System.Collections.Generic;
using Tiveria.Knx.Exceptions;

namespace Tiveria.Knx.Cemi
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
    ///
    /// MsgCode : A code describing the message transported in this cEMI Frame
    ///           See <seealso cref="Tiveria.Knx.Cemi.CemiMessageCode"/> for all supported codes and their meaning
    /// 
    ///  Add.Info Length : 0x00      = no additional info
    ///                    0x01-0xfe = number of total bytes in Service Information block
    ///                    0xff      = reserved
    ///                    
    /// Additional Information : N repetitions of a Additional Information Fields.
    ///                          See <seealso cref="Tiveria.Knx.Cemi.AdditionalInformationField"/> for more details on this substructure
    /// </code>
    /// </summary>
    public abstract class CemiBase : Structures.StructureBase, ICemi
    {

        #region private fields
        protected CemiMessageCode _messageCode;
        protected byte _additionalInfoLength;
        protected AdditionalInformationField[] _additionalInfoFields;
        protected byte[] _payload;
        #endregion

        #region public properties
        public CemiMessageCode MessageCode => _messageCode;
        public byte AdditionalInfoLength => _additionalInfoLength;
        public AdditionalInformationField[] AdditionalInfoFields => _additionalInfoFields;
        public byte[] Payload => _payload;
        #endregion

        #region constructors
        protected CemiBase(CemiMessageCode messageCode)
        {
            VerifyMessageCode(messageCode);
            _messageCode = messageCode;
        }

        /// <summary>
        /// Creates a new cEMI Frame
        /// </summary>
        /// <param name="br">BinaryReaderEx with the underlying buffer the frame is parsed from</param>
        protected CemiBase(EndianessAwareBinaryReader br)
        {
            if (!VerifyBufferSize(br))
                throw BufferSizeException.TooSmall("buffer too short for cEMI frame");
            ParseBuffer(br);
        }
        #endregion


        #region verifying elements
        protected abstract bool VerifyBufferSize(EndianessAwareBinaryReader br);

        protected abstract void VerifyMessageCode(CemiMessageCode messageCode);

        protected abstract void VerifyAdditionalLengthInfo(byte length);
        #endregion

        #region parsing the buffer
        protected virtual void ParseBuffer(EndianessAwareBinaryReader br)
        {
            ParseMessageCode(br);
            ParseAdditinalInfoLength(br);
            ParseAdditionalInfo(br);
            ParseServiceInformation(br);
        }

        protected void ParseMessageCode(EndianessAwareBinaryReader br)
        {
            var messageCode = br.ReadByte();
            if (Enum.IsDefined(typeof(CemiMessageCode), messageCode))
            {
                _messageCode = (CemiMessageCode)messageCode;
                VerifyMessageCode(_messageCode);
            }
            else
            {
                throw BufferFieldException.TypeUnknown("MessageCode", messageCode);
            }
        }

        protected void ParseAdditinalInfoLength(EndianessAwareBinaryReader br)
        {
            _additionalInfoLength = br.ReadByte();
            VerifyAdditionalLengthInfo(_additionalInfoLength);
        }

        protected void ParseAdditionalInfo(EndianessAwareBinaryReader br)
        {
            if (_additionalInfoLength == 0)
                return;
            var items = new List<AdditionalInformationField>(2);
            var count = 0;
            while (count < _additionalInfoLength)
            {
                var field = AdditionalInformationField.Parse(br);
                items.Add(field);
                count += field.StructureLength;
            }
            _additionalInfoFields = items.ToArray();
        }

        protected abstract void ParseServiceInformation(EndianessAwareBinaryReader br);
        #endregion

        public override void WriteToByteArray(byte[] buffer, int offset = 0)
        {
            base.WriteToByteArray(buffer, offset);
        }
    }
}