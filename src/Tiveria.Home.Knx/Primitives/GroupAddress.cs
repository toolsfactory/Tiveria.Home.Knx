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

using Tiveria.Home.Knx.Exceptions;
using Tiveria.Common.IO;


namespace Tiveria.Home.Knx.Primitives
{
    /// <summary>
    /// Class representing both a 2-level or a 3-level Group Address 
    /// </summary>
    public class GroupAddress : Address
    {
        #region Public properties
        /// <summary>
        /// Provides access to the Group Address using the ThreeLevel Address model
        /// </summary>
        public (int MainGroup, int MiddleGroup, int SubGroup) 
            ThreeLevelAddress { get => (RawAddress >> 11 & 0x1f, RawAddress >> 8 & 0x07, RawAddress & 0xff); }

        /// <summary>
        /// Provides access to the Group Address using the TwoLevel Address model
        /// </summary>
        public (int MainGroup, int SubGroup) 
            TwoLevelAddress { get => (RawAddress >> 11 & 0x1f, RawAddress & 0x07_ff); }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new GroupAddress from a unsigned short
        /// </summary>
        /// <param name="address">Address encoded as ushort</param>
        public GroupAddress(ushort address)
           : base(AddressType.GroupAddress, address)
        { }

        /// <summary>
        /// Creates a new Groupaddress from the three elements of a three level address
        /// </summary>
        /// <param name="mainGroup">Main Group (0-31)</param>
        /// <param name="middleGroup">Middle Group (0-7)</param>
        /// <param name="subGroup">Sub Group(0-255)</param>
        /// <exception cref="ArgumentException">Thrown in case one of the group values is out of range</exception>
        public GroupAddress(int mainGroup, int middleGroup, int subGroup)
            : this ((ushort)((mainGroup << 11) | (middleGroup << 8) | subGroup))
        {
            if (!IsValidThreeLevelAddress(mainGroup, middleGroup, subGroup))
                throw new ArgumentException("Invalid KNX group address triple");
        }

        /// <summary>
        /// Creates a new Groupaddress from the two elements of a two level address
        /// </summary>
        /// <param name="mainGroup">Main Group(0-31)</param>
        /// <param name="subGroup">Sub Group(0-2047)</param>
        /// <exception cref="ArgumentException"></exception>
        public GroupAddress(int mainGroup, int subGroup)
            : this((ushort)((mainGroup << 11) | subGroup))
        {
            if (!IsValidTwoLevelAddress(mainGroup, subGroup))
                throw new ArgumentException("Invalid KNX group address touple");
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates a cloned Groupaddress with the same value
        /// </summary>
        /// <returns>New GroupAddress object</returns>
        public override object Clone()
        {
            return new GroupAddress(RawAddress);
        }

        /// <summary>
        /// Converts the group address to a string based on the global address style <see cref="GroupAddress.Style"/>
        /// </summary>
        /// <returns>formatted string</returns>
        public override string ToString()
        {
            return ToString(GroupAddress.Style);
        }

        /// <summary>
        /// Converts the group address to a string based on the paramter
        /// </summary>
        /// <param name="style">Styple to be appied</param>
        /// <returns>formatted string</returns>
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
        #endregion

        #region Public Static Properties
        /// <summary>
        /// Global <see cref="GroupAddressStyle"/>. Mainly used for <see cref="GroupAddress.ToString"/>.
        /// </summary>
        public static GroupAddressStyle Style { get; set; } = GroupAddressStyle.ThreeLevel;
        #endregion Public Static Properties

        #region Public Static Methods
        #region Static Empty
        /// <summary>
        /// Returns an empty GroupAddress with all levels being set to 0.
        /// </summary>
        /// <returns>The empty GroupAddress</returns>
        public static GroupAddress Empty() => new GroupAddress(0);
        #endregion Static Empty


        #region Static Byte Converters
        public static GroupAddress FromReader(BigEndianBinaryReader reader)
        {
            return new GroupAddress(reader.ReadUInt16());
        }
        #endregion


        #region Static parsing
        /// <summary>
        /// Converts the string represenation of a GroupAddress to the equivalent <see cref="GroupAddress"/> object.
        /// Supports all three <see cref="GroupAddressStyle"/> variants.
        /// </summary>
        /// <param name="input">The string to convert</param>
        /// <returns>The resulting GroupAddress object</returns>
        /// <exception cref="ArgumentNullException">input is null</exception>
        /// <exception cref="FormatException">input is not in a recognized format.</exception>
        public static GroupAddress Parse(string input)
        {
            if (input == null)
                throw new ArgumentNullException("address string is null");
            var elements = input.Split('/');
            switch (elements.Length)
            {
                case 1: return new GroupAddress(ushort.Parse(elements[0]));
                case 2: return new GroupAddress(byte.Parse(elements[0]), ushort.Parse(elements[1]));
                case 3: return new GroupAddress(byte.Parse(elements[0]), byte.Parse(elements[1]), byte.Parse(elements[2]));
                default: throw new FormatException($"address string has wrong format: {input}");
            }
        }

        /// <summary>
        /// Tries to convert the string represenation of a GroupAddress to the equivalent <see cref="GroupAddress"/> object.
        /// Supports all three <see cref="GroupAddressStyle"/> variants.
        /// </summary>
        /// <param name="input">The string to convert</param>
        /// <param name="result">When this method returns, contains the parsed value. If the method returns true, result contains a valid <see cref="GroupAddress"/>. If the method returns false, result equals Empty.</param>
        /// <returns>true if the parse operation was successful; otherwise, false.</returns>
        public static bool TryParse(string input, out GroupAddress result)
        {
            try
            {
                result = Parse(input);
                return true;
            }
            catch
            {
                result = GroupAddress.Empty();
                return false;
            }
        }
        #endregion


        #region Static Group Styling Helpers

        /// <summary>
        /// Checks if the global <see cref="GroupAddress.Style"/> switch is set to <see cref="GroupAddressStyle.Free"/>
        /// </summary>
        /// <returns>true if the style is <see cref="GroupAddressStyle.Free"/></returns>
        public static bool IsFreeLevelStyle()
        {
            return Style == GroupAddressStyle.Free;
        }

        /// <summary>
        /// Checks if the global <see cref="GroupAddress.Style"/> switch is set to <see cref="GroupAddressStyle.ThreeLevel"/>
        /// </summary>
        /// <returns>true if the style is <see cref="GroupAddressStyle.ThreeLevel"/></returns>
        public static bool IsThreeLevelStyle()
        {
            return Style == GroupAddressStyle.ThreeLevel;
        }

        /// <summary>
        /// Checks if the global <see cref="GroupAddress.Style"/> switch is set to <see cref="GroupAddressStyle.TwoLevel"/>
        /// </summary>
        /// <returns>true if the style is <see cref="GroupAddressStyle.TwoLevel"/></returns>
        public static bool IsTwoLevelStyle()
        {
            return Style == GroupAddressStyle.TwoLevel;
        }

        /// <summary>
        /// Checks whether the values provided represent a valid three level address or not
        /// </summary>
        /// <param name="main">Main group level of the KNX address</param>
        /// <param name="middle">Middle group level of the KNX address</param>
        /// <param name="sub">Sub group level of the KNX address</param>
        /// <returns>true in case the values are in valid ranges for the individual group levels. Otherwise false</returns>
        public static bool IsValidThreeLevelAddress(int main, int middle, int sub)
        {
            return (main & ~0x1f) == 0 && (middle & ~0x07) == 0 && ((sub & ~0xff) == 0);
        }

        /// <summary>
        /// Checks whether the values provided represent a valid two level address or not
        /// </summary>
        /// <param name="main">Main group level of the KNX address</param>
        /// <param name="sub">Sub group level of the KNX address</param>
        /// <returns>true in case the values are in valid ranges for the individual group levels. Otherwise false</returns>
        public static bool IsValidTwoLevelAddress(int main, int sub)
        {
            return (main & ~0x1f) == 0 && (sub & ~0x07_ff) == 0;
        }

        #endregion Static Group Styling Helpers

        #endregion Public Static Methods
    }
}
