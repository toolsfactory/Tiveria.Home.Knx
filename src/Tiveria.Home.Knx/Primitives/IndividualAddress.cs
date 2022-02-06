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

namespace Tiveria.Home.Knx.Primitives
{
    /// <summary>
    /// Class representing an individual device address
    /// </summary>
    public class IndividualAddress : Address
    {
        #region Public properties
        /// <summary>
        /// The area the device belongs to
        /// </summary>
        public int Area  => RawAddress >> 12;
        /// <summary>
        /// The line the device belongs to
        /// </summary>
        public int Line  => RawAddress >> 8 & 0x0F; 
        /// <summary>
        /// The individual device id in the line
        /// </summary>
        public int Device => RawAddress & 0xFF; 
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instence of the <see cref="IndividualAddress"/> object with a raw address
        /// </summary>
        /// <param name="address">The raw address</param>
        public IndividualAddress(ushort address)
           : base(AddressType.IndividualAddress, address)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndividualAddress"/> object with values for area, line and device
        /// </summary>
        /// <param name="area">The area (0-15)</param>
        /// <param name="line">The line (0-15)</param>
        /// <param name="device">The device (0-255)</param>
        /// <exception cref="ArgumentOutOfRangeException">Throw one of the parameters is not in the specified range</exception>
        public IndividualAddress(int area, int line, int device)
            : this((ushort) (area << 12 | line << 8 | device))
        {
            if (!IsValidAddressTriple(area, line, device))
                throw new ArgumentOutOfRangeException("part of address out of range");
        }
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return $"{Area}.{Line}.{Device}";
        }

        /// <summary>
        /// Creates a new copy of the <see cref="IndividualAddress"/>
        /// </summary>
        /// <returns>the new <see cref="IndividualAddress"/> with idlentical Area, Line and Device values</returns>
        public override object Clone()
        {
            return new IndividualAddress(RawAddress);
        }
        #endregion

        #region Static parsing
        /// <summary>
        /// Tries to convert the string represenation of an individual address to the equivalent <see cref="IndividualAddress"/> object.
        /// </summary>
        /// <param name="input">The string to convert</param>
        /// <param name="result">When this method returns, contains the parsed value. If the method returns true, result contains a valid <see cref="GroupAddress"/>. If the method returns false, result equals Empty.</param>
        /// <returns>true if the parse operation was successful; otherwise, false.</returns>
        public static bool TryParse(string input, out IndividualAddress result)
        {
            try
            {
                result = Parse(input);
                return true;
            }
            catch
            {
                result = IndividualAddress.Empty();
                return false;
            }
        }

        /// <summary>
        /// Converts the string represenation of an individual address to the equivalent <see cref="IndividualAddress"/> object.
        /// </summary>
        /// <param name="input">The string to convert</param>
        /// <returns>The resulting GroupAddress object</returns>
        /// <exception cref="ArgumentNullException">input is null</exception>
        /// <exception cref="FormatException">input is not in a recognized format.</exception>
        public static IndividualAddress Parse(string input)
        {
            if (input == null)
                throw new ArgumentNullException("address string is null");
            var elements = input.Split('.');
            if (elements == null || elements.Length != 3)
                throw new ArgumentException("address string has wrong format");
            var area = int.Parse(elements[0]);
            var line = int.Parse(elements[1]);
            var device = int.Parse(elements[2]);
            return new IndividualAddress(area, line, device);
        }
        #endregion

        #region Static helpers
        /// <summary>
        /// Checks if values for area, line and device are in the allowed ranges
        /// </summary>
        /// <param name="area">The area (0-15)</param>
        /// <param name="line">The line (0-15)</param>
        /// <param name="device">The device (0-255)</param>
        public static bool IsValidAddressTriple(int area, int line, int device)
        {
            return (area & ~0x0f) == 0 && (line & ~0x0f) == 0 && ((device & ~0xff) == 0);
        }

        /// <summary>
        /// Returns a static <see cref="IndividualAddress"/> with area, line and device set to 0
        /// </summary>
        /// <returns>The <see cref="IndividualAddress"/> 0.0.0</returns>
        public static IndividualAddress Empty() => new IndividualAddress(0);
        #endregion
    }
}
