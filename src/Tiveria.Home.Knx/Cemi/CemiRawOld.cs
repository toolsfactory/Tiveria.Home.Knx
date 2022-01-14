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
    public class CemiRawOld : CemiBase, ICemiMessage
    {
        private static int MESSAGEMINLENGTH = 1;

        #region public properties
        public byte[] Payload { get; private set; }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new cEMI L_Data Frame
        /// </summary>
        /// <param name="br">BigEndianBinaryReader with the underlying buffer the frame is parsed from</param>
        protected CemiRawOld(BigEndianBinaryReader br)
            : base(br)
        { 
            if (Payload == null)
                Payload = Array.Empty<byte>();
        }

        public CemiRawOld(MessageCode messageCode, byte[] payload)
            : base(messageCode)
        {
            Payload = payload ?? Array.Empty<byte>();
            Size = MESSAGEMINLENGTH + AdditionalInfoLength + Payload.Length; 
        }

        #endregion

        #region private methods
        protected override void ParseServiceInformation(BigEndianBinaryReader br)
        {
        }

        protected virtual void VerifyPayload(byte[] tpdu)
        {
        }

        protected override void VerifyMessageCode(MessageCode messageCode)
        {
        }

        protected override bool VerifyBufferSize(BigEndianBinaryReader br)
        {
            return br.Size - br.Position >= MESSAGEMINLENGTH;
        }

        protected override void VerifyAdditionalLengthInfo(byte length)
        {
        }

        public override void CalculateSize()
        {
            Size = MESSAGEMINLENGTH + AdditionalInfoLength + Payload.Length;
        }

        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write((byte) MessageCode);
            writer.Write((byte)Size);
            writer.Write((byte)AdditionalInfoLength);
            foreach (var info in AdditionalInfoFields)
                info.Write(writer);
            if(Payload.Length > 0)
                writer.Write(Payload);
        }

        #endregion

        public override string ToDescription(int padding = 4)
        {
            return $"CemiRaw: MC = {MessageCode}, Payload = {BitConverter.ToString(Payload)}".AddPrefixSpaces(padding);
        }

        #region static methods
        /// <summary>
        /// Parses a part of a buffer and creates a CemiRaw class from it
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static CemiRawOld Parse(byte[] buffer, int offset, int length)
        {
            return new CemiRawOld(new BigEndianBinaryReader(buffer, offset, length));
        }

        public static CemiRawOld Parse(BigEndianBinaryReader br)
        {
            return new CemiRawOld(br);
        }

        public static bool TryParse(byte[] buffer, int offset, int length, out CemiRawOld? CemiRaw)
        {
            bool result = false;
            try
            {
                CemiRaw = new CemiRawOld(new BigEndianBinaryReader(buffer, offset, length));
                result = true;
            }
            catch
            {
                CemiRaw = null;
                result = false;
            }
            return result;
        }

        public static bool TryParse(BigEndianBinaryReader br, out CemiRawOld? CemiRaw)
        {
            bool result = false;
            try
            {
                CemiRaw = new CemiRawOld(br);
                result = true;
            }
            catch
            {
                CemiRaw = null;
                result = false;
            }
            return result;
        }

        #endregion
    }
}