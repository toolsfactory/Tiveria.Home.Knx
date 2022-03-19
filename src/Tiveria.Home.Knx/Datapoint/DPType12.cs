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

namespace Tiveria.Home.Knx.Datapoint
{
    public class DPType12 : DPType<UInt32>
    {
        protected DPType12(string id, string name, string unit = "", string description = "") : base(id, name, UInt32.MinValue, UInt32.MaxValue, unit, description)
        {
            DataSize = 4;
        }

        public override uint Decode(byte[] dptData, int offset = 0)
        {
            base.Decode(dptData, offset);
            return (uint) ((dptData[offset] << 24) | (dptData[offset + 1] << 16) | (dptData[offset + 2] << 8) | dptData[offset + 3]);
        }

        public override byte[] Encode(uint value)
        {
            return new byte[4] { (byte)((value >> 24) & 0xFF), (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF) };
        }

        #region specific xlator instances
        public static readonly DPType12 DPT_VALUE_4_UCOUNT = new("12.001", "Unsigned count", "counter pulses");

        internal static void Init()
        {
            DatapointTypesList.AddOrReplace(DPT_VALUE_4_UCOUNT);
        }
        #endregion
    }
}
