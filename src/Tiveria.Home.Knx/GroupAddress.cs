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

using System;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx
{
    /// <summary>
    /// Class representing both a 2-level or a 3-level Group Address 
    /// </summary>
    public class GroupAddress : Address
    {
        #region Public properties
        public (int MainGroup, int MiddleGroup, int SubGroup) 
            ThreeLevelAddress { get => (RawAddress >> 11 & 0x1f, RawAddress >> 8 & 0x07, RawAddress & 0xff); }

        public (int MainGroup, int SubGroup) 
            TwoLevelAddress { get => (RawAddress >> 11 & 0x1f, RawAddress & 0x07_ff); }
        #endregion

        #region Constructors
        public GroupAddress(ushort address) 
            : base(address)
        {
            _addressType = AddressType.GroupAddress;
        }

        public GroupAddress(int mainGroup, int middleGroup, int subGroup)
            : base()
        {
            if (!IsValidThreeLevelAddress(mainGroup, middleGroup, subGroup))
                throw new ArgumentException("Invalid KNX group address triple");

            _addressType = AddressType.GroupAddress;
            _rawAddress = (ushort)((mainGroup << 11) | (middleGroup << 8) | subGroup);
        }

        public GroupAddress(int mainGroup, int subGroup)
            : base()
        {
            if (!IsValidTwoLevelAddress(mainGroup, subGroup))
                throw new ArgumentException("Invalid KNX group address touple");

            _addressType = AddressType.GroupAddress;
            _rawAddress = (ushort)((mainGroup << 11) | subGroup);
        }
        #endregion

        #region Public methods
        public override string ToString()
        {
            return ToString(GroupAddress.Style);
        }

        public string ToString(GroupAddressStyle style)
        {
            switch (style)
            {
                case GroupAddressStyle.ThreeLevel:
                    return $"{ThreeLevelAddress.MainGroup}/{ThreeLevelAddress.MiddleGroup}/{ThreeLevelAddress.SubGroup}";
                case GroupAddressStyle.TwoLevel:
                    return $"{TwoLevelAddress.MainGroup}/{TwoLevelAddress.SubGroup}";
                default:
                    return RawAddress.ToString();
            }
        }

        public bool IsValidThreeLevelAddress(int main, int middle, int sub)
        {
            return (main & ~0x1f) == 0 && (middle & ~0x07) == 0 && ((sub & ~0xff) == 0);
        }

        public bool IsValidTwoLevelAddress(int main, int sub)
        {
            return (main & ~0x1f) == 0 && (sub & ~0x07_ff) == 0;
        }
        #endregion


        #region Static parsing
        public static GroupAddress Parse(string address)
        {
            if (address == null)
                throw new ArgumentNullException("address string is null");
            var elements = address.Split('/');
            switch (elements.Length)
            {
                case 1 : return new GroupAddress(ushort.Parse(elements[0]));
                case 2 : return new GroupAddress(byte.Parse(elements[0]), ushort.Parse(elements[1]));
                case 3 : return new GroupAddress(byte.Parse(elements[0]), byte.Parse(elements[1]), byte.Parse(elements[2]));
                default: throw new ArgumentException("address string has wrong format");
            }
        }

        public static GroupAddress FromBuffer(byte[] buffer, int offset)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");
            if (buffer.Length - offset < 2)
                throw BufferSizeException.TooSmall("GroupAddress");
            return new GroupAddress((ushort)(buffer[offset] << 8 + buffer[offset + 1]));
        }
        #endregion
        
        #region Static style management
        public static GroupAddressStyle Style { get; set; } = GroupAddressStyle.ThreeLevel;

        public bool IsThreeLevelStyle()
        {
            return Style == GroupAddressStyle.ThreeLevel;
        }

        public bool IsTwoLevelStyle()
        {
            return Style == GroupAddressStyle.TwoLevel;
        }

        public bool IsFreeLevelStyle()
        {
            return Style == GroupAddressStyle.Free;
        }
        #endregion
    }
}
