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

namespace Tiveria.Home.Knx.BaseTypes
{
    public class SerialNumber
    {
        #region public constants
        public const int Size = 6;
        public static SerialNumber Zero = new SerialNumber(0);
        #endregion

        #region public properties
        public ulong Value { get => _value; set => _value = value & Mask; }
        #endregion

        #region private members
        private const ulong Mask = 0xffff_ffff_ffff;
        private ulong _value = 0;
        #endregion

        #region constructors
        public SerialNumber(ulong serno)
        {
            _value = serno;
        }

        public SerialNumber(long serno)
        {
            _value = (ulong)serno;
        }

        public SerialNumber(int serno)
        {
            _value = (ulong)serno;
        }

        public SerialNumber(byte[] serno)
        {
            if (serno == null) throw new ArgumentNullException(nameof(serno));
            if (serno.Length != Size) throw new ArgumentException(nameof(serno));
            _value = (ulong)((serno[0] << 40) + (serno[1] << 32) + (serno[2] << 24) + (serno[3] << 16) + (serno[4] << 8) + (serno[0] << 40));
        }
        #endregion

        #region public implementations
        public byte[]ToBytes()
        {
            var raw = new byte[Size];
            var data = BitConverter.GetBytes(Value);
            if (BitConverter.IsLittleEndian) Array.Reverse(data);
            data.AsSpan().Slice(2).CopyTo(raw);
            return raw;
        }

        public override string ToString()
        {
            return $"{Value>>32:x4}:{Value&0xffff_ffff:x8}";
        }
        #endregion
    }
}
