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

namespace Tiveria.Home.Knx.Primitives
{
    /// <summary>
    /// Class representing a Knx device serialnumber
    /// </summary>
    public class SerialNumber : IKnxDataElement
    {

        #region Public Constructors
        /// <summary>
        /// Initializes a new instance of <see cref="SerialNumber"/> from an long.
        /// </summary>
        /// <param name="serno">The serialnumber (0-281474976710655 / 0xffff_ffff_ffff). Only lower 48bits are considered</param>
        public SerialNumber(ulong serno)
        {
            _value = serno;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SerialNumber"/> from a long.
        /// </summary>
        /// <param name="serno">The serialnumber (0-281474976710655 / 0xffff_ffff_ffff). Only lower 48bits are considered</param>
        public SerialNumber(long serno)
        {
            _value = (ulong)serno;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SerialNumber"/> from an int.
        /// </summary>
        /// <param name="serno">The serialnumber</param>
        public SerialNumber(int serno)
        {
            _value = (ulong)serno;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SerialNumber"/> from a byte array.
        /// </summary>
        /// <param name="serno">The byte array</param>
        /// <exception cref="ArgumentNullException">The byte array is null</exception>
        /// <exception cref="ArgumentException">The size of the byte array is not 6</exception>
        public SerialNumber(byte[] serno)
        {
            if (serno == null) throw new ArgumentNullException(nameof(serno));
            if (serno.Length != Size) throw new ArgumentException(nameof(serno));
            _value = (ulong)((serno[0] << 40) + (serno[1] << 32) + (serno[2] << 24) + (serno[3] << 16) + (serno[4] << 8) + (serno[0] << 40));
        }
        #endregion Public Constructors

        #region Public Properties
        /// <summary>
        /// Size of the <see cref="SerialNumber"/> when sent via the KNX protocol
        /// </summary>
        public int Size => 6;

        public ulong Value { get => _value; set => _value = value & Mask; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>
        /// Creates an serialnumber object equal to 0
        /// </summary>
        /// <returns>The new <see cref="SerialNumber"/> object</returns>
        public static SerialNumber Zero() => new SerialNumber(0);

        /// <summary>
        /// Converts the <see cref="SerialNumber"/> to its string representation following the Knx default format ("a0b0:c0d0e0f0")
        /// </summary>
        /// <returns>The resulting string</returns>
        public override string ToString()
        {
            return $"{Value>>32:x4}:{Value&0xffff_ffff:x8}";
        }

        /// <summary>
        /// Writes a <see cref="SerialNumber"/> using the provided writer
        /// </summary>
        /// <param name="writer">The writer</param>
        public void Write(BigEndianBinaryWriter writer)
        {
            var raw = new byte[Size];
            var data = BitConverter.GetBytes(Value);
            if (BitConverter.IsLittleEndian) Array.Reverse(data);
            for (int i = 0; i < Size; i++)
                writer.Write(data[2 + i]);
        }

        /// <summary>
        /// Creates a byte array from the <see cref="SerialNumber"/>
        /// </summary>
        /// <returns>The array</returns>
        public byte[] ToBytes()
        {
            var raw = new byte[Size];
            var data = BitConverter.GetBytes(Value);
            if (BitConverter.IsLittleEndian) Array.Reverse(data);
            data.AsSpan().Slice(2).CopyTo(raw);
            return raw;
        }
        #endregion Public Methods

        #region Private Fields
        private const ulong Mask = 0xffff_ffff_ffff;
        private ulong _value = 0;
        #endregion Private Fields    
    }
}
