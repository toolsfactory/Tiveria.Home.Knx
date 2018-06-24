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
using Tiveria.Common.IO;

namespace Tiveria.Home.Knx.Cemi
{
    /// <summary>
    /// Extended Version of the <see cref="CemiLData"/> class that supports <see cref="AdditionalInformationField"/>.
    /// </summary>
    /// <remarks>Actual implementation of the correct parsing of <see cref="AdditionalInformationField"/> is already implemented in the base class</remarks>
    public class CemiLDataEx : CemiLData
    {
        #region constructors
        /// <summary>
        /// Creates a new cEMI L_Data Frame
        /// </summary>
        /// <param name="br">IndividualEndianessBinaryReader with the underlying buffer the frame is parsed from</param>
        protected CemiLDataEx(IndividualEndianessBinaryReader br)
            : base(br)
        { }

        public CemiLDataEx(CemiMessageCode messageCode, IndividualAddress srcAddress, IAddress dstAddress, byte[] tpdu,
                         Priority priority, bool repeat = true, BroadcastType broadcast = BroadcastType.Normal, bool ack = false, int hopCount = 6)
            : base(messageCode, srcAddress, dstAddress, tpdu, priority, repeat, broadcast, ack, hopCount)
        { }

        public CemiLDataEx(CemiMessageCode messageCode, IndividualAddress srcAddress, IAddress dstAddress, byte[] tpdu,
                         Priority priority, ConfirmType confirm)
            : this(messageCode, srcAddress, dstAddress, tpdu, priority, true, BroadcastType.Normal, false, 6)
        { }
        #endregion

        #region parsing the buffer
        protected override void VerifyAdditionalLengthInfo(byte length)
        {
            _additionalInfoLength = length;
        }
        #endregion

        #region static methods
        /// <summary>
        /// Parses a part of a buffer and creates a CemiLData class from it
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public new static CemiLDataEx Parse(byte[] buffer, int offset, int length)
        {
            return new CemiLDataEx(new IndividualEndianessBinaryReader(buffer, offset, length));
        }

        public new static CemiLDataEx Parse(IndividualEndianessBinaryReader br)
        {
            return new CemiLDataEx(br);
        }
        
        public static bool TryParse(out CemiLDataEx cemildata, IndividualEndianessBinaryReader br)
        {
            bool result = false;
            try
            {
                cemildata = new CemiLDataEx(br);
                result = true;
            }
            catch
            {
                cemildata = null;
                result = false;
            }
            return result;
        }
        #endregion

    }
}