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
    public class DPType3 : DPType<sbyte>
    {
        protected DPType3(string id, string name, sbyte min = -7, sbyte max = 7, string unit = "", string description = "") : base(id, name, min, max, unit, description)
        { }

        public override sbyte Decode(byte[] dptData, int offset = 0)
        {
            var data = dptData[offset];
            var control = (data & 0b0000_1000) != 0;
            var code = data & 0b0000_0111;
            return (sbyte) (control ? code : -code);
        }

        public override byte[] Encode(sbyte value)
        {
            if (value < Minimum || value > Maximum)
                throw new ArgumentOutOfRangeException($"Value must be in the range [{Minimum}..{Maximum}]");
            var data = new byte[1];
            data[0] = (byte)Math.Abs(value);
            if (value < 0) data[0] = (byte) (data[0] | 0b0000_1000);
            return data;
        }

        public static readonly DPType3 DPT_CONTROL_DIMMING = new DPType3("3.007", "Dimming", -7, 7);
        public static readonly DPType3 DPT_CONTROL_BLINDS = new DPType3("3.008", "Blinds", -7, 7, "intervals");

        static DPType3()
        {
            DatapointTypesList.AddOrReplace(DPT_CONTROL_DIMMING);
            DatapointTypesList.AddOrReplace(DPT_CONTROL_BLINDS);

        }
    }
}
