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
using System.Collections.Generic;
using Tiveria.Common.Extensions;
using Tiveria.Common.IO;
using Tiveria.Knx.Structures;
using Tiveria.Knx.Exceptions;

namespace Tiveria.Knx.Cemi
{
    /// <summary>
    /// Class representing an AdditionalInformationField within a cEMI Frame.
    /// For details please read: "03_06_03_EMI_IMI V01.03.03 AS.PDF" chapter 4 from KNX Association
    /// <code>
    /// +--------+--------+-----------------+
    /// | byte 1 | byte 2 | byte 3 - n      |
    /// +--------+--------+-----------------+
    /// | Info   | Info   |  Additional     |
    /// | Type   | Length |  Information    |
    /// +--------+--------+-----------------+
    /// 
    ///  InfoType :   <seealso cref="Tiveria.Knx.Cemi.AdditionalInfoType"/>
    ///  InfoLength : Size of Additional Info 
    ///  Additional Info : Array of bytes representign the additional infos
    /// </summary>
    public class AdditionalInformationField : StructureBase, IStructure
    {
        #region internal static field with size infos
        private static readonly Dictionary<AdditionalInfoType, int> TypeSizes = new Dictionary<AdditionalInfoType, int>
        {
            {AdditionalInfoType.RESERVED0,      0 },
            {AdditionalInfoType.PLMEDIUM,       2 },
            {AdditionalInfoType.RFMEDIUM,       8 },
            {AdditionalInfoType.BUSMONITOR,     1 },
            {AdditionalInfoType.TIMESTAMP,      2 },
            {AdditionalInfoType.TIMEDELAY,      4 },
            {AdditionalInfoType.TIMESTAMP_EXT,  4 },
            {AdditionalInfoType.BIBAT,          2 },
            {AdditionalInfoType.RFMULTI,        4 },
            {AdditionalInfoType.PREPOSTAMBLE,   3 },
            {AdditionalInfoType.RFFASTACK,      255 },
            {AdditionalInfoType.MANUFACTURER,   255 },
            {AdditionalInfoType.RESERVED255,    0 }
        };
        #endregion

        #region private fields
        private byte[] _information;
        private int _infoLength;
        private AdditionalInfoType _infoType;
        #endregion

        #region public properties
        public AdditionalInfoType InfoType => _infoType;
        public int InfoLength => _infoLength;
        public byte[] Information => _information;
        #endregion

        #region construct from fields
        /// <summary>
        /// Create an <c>AdditionalInformationField</c> class from a type info and the actual info
        /// </summary>
        /// <param name="infoType"></param>
        /// <param name="info"></param>
        public AdditionalInformationField(AdditionalInfoType infoType, byte[] info)
        {
            if (info == null)
                throw new ArgumentNullException("data parameter must not be null!");
            if (info.Length < 1)
                throw new ArgumentException("Size of data array to small");
            _infoType = infoType;
            SetInfo(info);
            _structureLength = 2 + _infoLength;
        }

        private void SetInfo(byte[] data)
        {
            _infoLength = data[0];
            if (_infoLength != data.Length - 1)
                throw BufferFieldException.WrongValue("AdditionalInfo - Data", data.Length - 1, _infoLength);
            VerifyLength(_infoType, _infoLength);
            _information = new byte[_infoLength];
            Array.Copy(data, 1, _information, 0, _infoLength);
        }
        #endregion

        #region construct from buffer with BinaryReader
        /// <summary>
        /// Create an <c>AdditionalInformationField</c> class by parsing raw data
        /// </summary>
        /// <param name="br">Reader with the associated byte array</param>
        protected AdditionalInformationField(IndividualEndianessBinaryReader br)
        {
            ParseType(br);
            ParseLength(br);
            if (br.Available < _infoLength)
                throw BufferSizeException.TooSmall("AdditionalInformationField");
            ParseInfo(br);
            VerifyLength(_infoType, _infoLength);
            _structureLength = 2 + _infoLength;
        }

        private void ParseType(IndividualEndianessBinaryReader br)
        {
            var infotype = br.ReadByte();
            if (Enum.IsDefined(typeof(AdditionalInfoType), infotype))
            {
                _infoType = (AdditionalInfoType)infotype;
            }
            else
                throw BufferFieldException.TypeUnknown("AdditionalInfoType", infotype);
        }

        private void ParseLength(IndividualEndianessBinaryReader br)
        {
            _infoLength = br.ReadByte();
            if (br.Available <_infoLength)
                throw BufferSizeException.TooSmall("AdditionalFieldInfo");
        }

        private void ParseInfo(IndividualEndianessBinaryReader br)
        {
            if (_infoLength > 0)
                _information = br.ReadBytes(_infoLength);
        }
        #endregion

        #region other private methods
        private void VerifyLength(AdditionalInfoType type, int length)
        {
            if (type == AdditionalInfoType.RFFASTACK || type == AdditionalInfoType.MANUFACTURER)
            {
                // TODO: Add checking of correct length 
                // requires some calcs
            }
            else
            {
                if (length != TypeSizes[type])
                {
                    throw BufferFieldException.WrongValue($"AdditionalInfo ({type}) length", TypeSizes[type], length);
                }
            }
        }
        #endregion

        #region write data to buffer
        public override void WriteToByteArray(byte[] buffer, int offset = 0)
        {
            base.WriteToByteArray(buffer, offset);
            buffer[offset] = (byte)_infoType;
            buffer[offset + 1] = (byte)_infoLength;
            Array.Copy(_information, 0, buffer, offset + 2, _infoLength);
        }
        #endregion

        public string ToDescription(int padding)
        {
            return ($"AdditionalInfo : Type = {InfoType}, Length = {InfoLength}, Data = " + Information.ToHexString()).AddPrefixSpaces(padding);
        }

        #region Static parsing function
        public static AdditionalInformationField Parse(IndividualEndianessBinaryReader br)
        {
            if (br.Available < 2)
                throw BufferSizeException.TooSmall("AdditionalInformationField");
            return new AdditionalInformationField(br);
        }
        #endregion
    }
}