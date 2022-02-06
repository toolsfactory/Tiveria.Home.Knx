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

using Tiveria.Home.Knx.Primitives;

namespace Tiveria.Home.Knx.Cemi
{
    /// <summary>
    /// Represents the Control Field 2 of a cEMI structure.
    /// Details can be found in "03_06_03_EMI_IMI V01.03.03 AS.PDF" Chapter 4.1.5.3 
    /// <code>
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// | bit 7  | bit 6  | bit 5  | bit 4  | bit 3  | bit 2  | bit 1  | bit 0  |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// | Address| Hop Count                |Extended Frame Format              |
    /// | Type   |                          |                                   |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// </code>
    /// 
    /// Address Type:       Destination Address Type
    ///                     0 = individual, 1 = group
    /// Hop Count:          Hop Count binary encoded
    ///                     0 = min value, 7 = max value
    /// Extended Frame Format:  (EFF) (bit 3 to bit 0 (lsb))
    ///                     0b0000 = for standard frame(long frames, APDU > 15 octet)
    ///                     0b01xx = for LTE frames (NOT FULLY SUPPORTED!)
    /// </summary>
    public class ControlField2
    {

        #region public properties
        #region private backing fields
        private byte _hopCount = 6;
        private byte extendedFrameFormat = 0;
        #endregion

        public AddressType DestinationAddressType { get; set; }
        public byte HopCount { get => _hopCount; set => _hopCount = (byte)((value > 7) ? 7 : value); }
        public byte ExtendedFrameFormat { get => extendedFrameFormat; set => extendedFrameFormat = (byte)((value > 0b1111) ? 0b1111 : value); }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a cEMI ControlField2 instance and parses the byte into the corresponding properties
        /// </summary>
        /// <param name="data">Raw represenation of the cemi controlfield 2</param>
        public ControlField2(byte data)
        {
            HopCount = (byte)((data & 0b0_111_0000) >> 4);
            DestinationAddressType = ((data & 0b1000_0000) == 0) ? AddressType.IndividualAddress : AddressType.GroupAddress;
            ExtendedFrameFormat = (byte)(data & 0b0000_1111);
        }

        /// <summary>
        /// Creates a cEMI Controlfield 2 from the individual flags and values
        /// </summary>
        /// <param name="groupAddress">True if address is a group address. Otherwise (individual address) false.</param>
        /// <param name="hopCount">Number of hops. Range: 0..7</param>
        /// <param name="extendedFrameFormat">Format of the extended  frame. 0=default, 0b0100-0b0111 LTE frames (not fully supported in here)</param>
        public ControlField2(bool groupAddress = true, int hopCount = 6, int extendedFrameFormat = 0)
        {
            HopCount = (byte)hopCount;
            DestinationAddressType = groupAddress ? AddressType.GroupAddress : AddressType.IndividualAddress;
            ExtendedFrameFormat = (byte)extendedFrameFormat;
        }
        #endregion

        #region public implementations
        public byte ToByte()
        {
            var raw = (byte)(HopCount << 4);
            raw |= ExtendedFrameFormat;
            if (DestinationAddressType == AddressType.GroupAddress)
                raw |= 0b1000_0000;
            return raw;
        }

        public string ToDescription(int padding)
        {
            var effbin = ("0000" + Convert.ToString(ExtendedFrameFormat, 2));
            effbin = effbin.Substring(effbin.Length - 5);
            var spaces = new String(' ', padding);
            return $"{spaces}Ctrl2: DestinationAddressType = {DestinationAddressType}, HopCount = {HopCount}, ExtendedFrameFormat = {effbin}";
        }
        #endregion
    }
}