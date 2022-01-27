﻿/*
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

namespace Tiveria.Home.Knx.Cemi
{
    /// <summary>
    /// For data that is 6 bits or less in length, only the first two bytes are used in a Common EMI
    /// frame. Common EMI frame also carries the information of the expected length of the Protocol
    /// Data Unit (PDU). Data payload can be at most 14 bytes long.  <p>
    /// 
    /// The first byte is a combination of transport layer control information (TPCI) and application
    /// layer control information (APCI). First 6 bits are dedicated for TPCI while the two least
    /// significant bits of first byte hold the two most significant bits of APCI field, as follows:
    /// <code>
    /// +-----------------------------------------------------------------------+
    /// |          APDU Byte 1: 6 bit TPCI & 2 bit APCI                         |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// | bit 0  | bit 1  | bit 2  | bit 3  | bit 4  | bit 5  | bit 6  | bit 7  |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// |                       TPCI Data                     |    APCI         |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// 
    /// +-----------------------------------------------------------------------+
    /// |          APDU Byte 2: 2 bit APCI & 6bit Data or APCI                  |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// | bit 0  | bit 1  | bit 2  | bit 3  | bit 4  | bit 5  | bit 6  | bit 7  |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// |    APCI         |          ACPI or Data                               |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// </code>
    /// 
    /// APCI is whether 4 bit or 10 bit long
    /// 4 bits: APDU Byte 1 bits 6+7 and APDU Byte 2 bits 0+1
    /// 10 bits: 4 bits as above and APDU byte 2 bits 2+3+4+5+6+7
    /// </summary>
    public class Apci : ITpdu
    {
        #region public properties
        public int Size { get; private set; } = 2;
        public byte[] Data { get; private set; } = Array.Empty<byte>();
        public int Type { get; private set; } = ApciTypes.GroupValue_Read;

        public bool IsApci { get; private set; } = true;
        public bool IsTpci => false;
        public TpduType TpduType => TpduType.ApciOnly;
        #endregion

        #region private members
        private byte[] _raw = new byte[0];
        #endregion

        #region constructors
        public Apci(int type, byte[] data)
        {
            if (Type < 0 || Type > 0b1111111111)
                throw new ArgumentException("Apci type out of range!");

            Type = type;
            Data = data ?? Array.Empty<byte>();
            if (ApciTypes.IsKnown(Type))
            {
                if (ApciTypes.RequiresData(Type) && Data.Length == 0)
                    throw new ArgumentException($"Data cannot be empty for APCI Type {ApciTypes.ToString(type)}");
                if (!ApciTypes.RequiresData(Type) && Data.Length > 0)
                    throw new ArgumentException($"Data must empty for APCI Type {ApciTypes.ToString(type)}");
            }
            CalculateSize();
            BuildRaw();
        }

        public Apci(int type)
            : this(type, Array.Empty<byte>())
        { }

        public Apci(Span<byte> buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("data");
            if (buffer.Length < 2)
                throw BufferSizeException.TooSmall("Apci");

            Size = buffer.Length;
            _raw = buffer.ToArray();
            ParseApci(buffer);
        }
        #endregion

        #region public methods
        public byte[] ToBytes()
        {
            return _raw;
        }

        public void Write(BigEndianBinaryWriter writer)
        {
            writer.Write(_raw);
        }

        public override string? ToString()
        {
            return $"APCI: Size {Size} / Type {Type} / Data {BitConverter.ToString(Data)}";
        }
        #endregion

        #region private implementation
        private void ParseApci(Span<byte> buffer)
        {
            var apci4 = ((buffer[0] & 0b000000_11) << 2) | (buffer[1] >> 6);
            var apci6 = (buffer[1] & 0b00_111111);
            Type = ApciTypes.GetApciType((apci4, apci6, buffer.Length));
            var loc = ApciTypes.GetASDUMaskAndOffset((Type, buffer.Length));
            Data = buffer.Slice(loc.offset).ToArray();
            if (Data.Length > 0)
                Data[0] &= (byte)loc.mask;
        }

        private void CalculateSize()
        {
            Size = ((Type == ApciTypes.GroupValue_Write || Type == ApciTypes.GroupValue_Response)
                 && (Data.Length == 1) && (Data[0] <= 63)) ? 2 : Data.Length + 2;
        }
        private void BuildRaw()
        {
            _raw = new byte[Size];
            _raw[0] = (byte) ((Type <= 0b1111) ? Type >> 2 : Type >> 8 );
            _raw[1] = (byte) ((Type <= 0b1111) ? (Type & 0b0011) << 6 : Type & 0b11111111);

            if((Type == ApciTypes.GroupValue_Write || Type == ApciTypes.GroupValue_Response) 
                && Data.Length == 1 && Data[0] <= 63)
                _raw[1] |= Data[0];
            else
                Data.CopyTo(_raw, 2);
        }
        #endregion

        #region static helpers
        public static Apci Parse(Span<byte> buffer)
        {
            return new Apci(buffer);
        }
        #endregion
    }
}
