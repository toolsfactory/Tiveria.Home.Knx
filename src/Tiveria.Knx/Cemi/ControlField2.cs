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

namespace Tiveria.Knx.Cemi
{
    /// <summary>
    /// Represents the Control Field 2 of a cEMI structure.
    /// Details can be found in "03_06_03_EMI_IMI V01.03.03 AS.PDF" Chapter 4.1.5.3 
    /// </summary>
    /// <code>
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// | bit 7  | bit 6  | bit 5  | bit 4  | bit 3  | bit 2  | bit 1  | bit 0  |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// | Address| Hop Count                |Extended Frame Format              |
    /// | Type   |                          |                                   |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// 
    /// Address Type:       Destination Address Type
    ///                     0 = individual, 1 = group
    /// Hop Count:          Hop Count binary encoded
    ///                     0 = min value, 7 = max value
    /// Extended Frame Format:  (EFF) (bit 3 to bit 0 (lsb))
    ///                     0b0000 = for standard frame(long frames, APDU > 15 octet)
    ///                     0b01xx = for LTE frames (NOT FULLY SUPPORTED!)
    /// </code>
    public class ControlField2
    {
        #region private fields
        private byte _rawData;
        private KnxAddressType _destinationAddressType;
        private byte _hopCount;
        private byte _extendedFrameFormat;
        #endregion

        #region public properties
        public byte RawData => _rawData;
        public KnxAddressType DestinationAddressType => _destinationAddressType;
        public byte HopCount => _hopCount;
        public byte ExtendedFrameFormat => _extendedFrameFormat;
        #endregion

        #region constructors
        /// <summary>
        /// Creates a cEMI ControlField2 instance and parses the byte into the corresponding properties
        /// </summary>
        /// <param name="data">Raw represenation of the cemi controlfield 2</param>
        public ControlField2(byte data)
        {
            _rawData = data;
            ParseData();
        }

        /// <summary>
        /// Creates a cEMI Controlfield 2 from the individual flags and values
        /// </summary>
        /// <param name="groupAddress">True if address is a group address. Otherwise (individual address) false.</param>
        /// <param name="hopCount">Number of hops. Range: 0..7</param>
        /// <param name="extendedFrameFormat">Format of the extended  frame. 0=default, 0b0100-0b0111 LTE frames (not fully supported in here)</param>
        public ControlField2(bool groupAddress = true, int hopCount = 6, int extendedFrameFormat = 0)
        {
            if (hopCount < 0 || hopCount > 0b0111)
                throw new ArgumentOutOfRangeException("hop count out of range 0..7");
            if (extendedFrameFormat < 0 || extendedFrameFormat > 0b0111)
                throw new ArgumentOutOfRangeException("extended frame format out of range 0..7");
            _hopCount = (byte)hopCount;
            _destinationAddressType = groupAddress ? KnxAddressType.GroupAddress : KnxAddressType.IndividualAddress;
            _extendedFrameFormat = (byte)extendedFrameFormat;
            ToByte();
        }
        #endregion

        #region private methods
        private void ToByte()
        {
            _rawData = 0;
            _rawData = (byte)(_hopCount << 4);

            _rawData |= _extendedFrameFormat;

            if (_destinationAddressType == KnxAddressType.GroupAddress)
                _rawData |= 0b1000_0000;
        }

        private void ParseData()
        {
            _hopCount = (byte)((_rawData & 0b0111_0000) >> 4);
            _destinationAddressType = ((_rawData & 0b1000_0000) == 0) ? KnxAddressType.IndividualAddress : KnxAddressType.GroupAddress;
            _extendedFrameFormat = (byte) (_rawData & 0b0000_1111);
        }
        #endregion

        public string ToDescription(int padding)
        {
            var effbin = ("0000" + Convert.ToString(ExtendedFrameFormat, 2));
            effbin = effbin.Substring(effbin.Length - 5);
            var spaces = new String(' ', padding);
            return $"{spaces}Ctrl2: DestinationAddressType = {DestinationAddressType}, HopCount = {HopCount}, ExtendedFrameFormat = {effbin}";
        }
    }
}