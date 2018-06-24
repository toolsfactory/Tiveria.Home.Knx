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

namespace Tiveria.Home.Knx.Datapoint
{
    public class DPType6 : DPType<sbyte>
    {
        protected DPType6(string id, string name, sbyte min, sbyte max, string unit = "", string description = "") : base(id, name, min, max, unit, description)
        {  }

        public override sbyte Decode(byte[] dptData, int offset = 0)
        {
            return (sbyte)dptData[offset];
        }

        public override byte[] Encode(sbyte value)
        {
            return new byte[1] { (byte) value };
        }

        public static readonly DPType6 DPT_PERCENT_V8 = new DPType6("6.001", "Percent (8 Bit)", -128, 127, "%");
        public static readonly DPType6 DPT_VALUE_1_UCOUNT = new DPType6("6.010", "Decimal factor", -128, 127, "pulses");

        // 6.020 not supported at the moment

    }
}
