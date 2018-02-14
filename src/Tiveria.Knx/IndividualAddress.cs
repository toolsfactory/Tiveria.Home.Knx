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

namespace Tiveria.Knx
{
    public class IndividualAddress : KnxAddress
    {
        public int Area { get => (int) _rawAddress >> 12; }
        public int Line { get => (int)_rawAddress >> 8 & 0x0F; }
        public int Device { get => (int)_rawAddress & 0xFF; }

        public IndividualAddress(ushort address) 
            : base (address)
        {
            _addressType = KnxAddressType.IndividualAddress;
        }

        public IndividualAddress(int area, int line, int device)
            : base()
        {
            if (!IsValidAddressTriple(area, line, device))
                throw new ArgumentOutOfRangeException("part of address out of range");
            _rawAddress =(ushort) (area << 12 | line << 8 | device);
            _addressType = KnxAddressType.IndividualAddress;
        }

        public override string ToString()
        {
            return $"{Area}.{Line}.{Device}";
        }

        public bool IsValidAddressTriple(int area, int line, int device)
        {
            return (area & ~0x0f) == 0 && (line & ~0x0f) == 0 && ((device & ~0xff) == 0);
        }

        public static IndividualAddress Parse(string address)
        {
            if (address == null)
                throw new ArgumentNullException("address string is null");
            var elements = address.Split('.');
            if (elements == null || elements.Length != 3)
                throw new ArgumentException("address string has wrong format");
            var area = int.Parse(elements[0]);
            var line = int.Parse(elements[1]);
            var device = int.Parse(elements[2]);
            return new IndividualAddress(area, line, device);
        }

        public static IndividualAddress FromBuffer(byte[] buffer, int offset)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");
            if (buffer.Length - offset < 2)
                throw BufferSizeException.TooSmall("GroupAddress");
            return new IndividualAddress((ushort)(buffer[offset] << 8 + buffer[offset + 1]));
        }
    }
}
