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
    public class DPType232 : DPType<(byte Red, byte Green, byte Blue)>
    {
        protected DPType232(string id, string name, (byte Red, byte Green, byte Blue) min, (byte Red, byte Green, byte Blue) max, string unit = "", string description = "") : base(id, name, min, max, unit, description)
        {
            DataSize = 3;
        }

        public override byte[] Encode((byte Red, byte Green, byte Blue) value)
        {
            return new byte[3] { value.Red, value.Green, value.Blue };
        }

        public override (byte Red, byte Green, byte Blue) Decode(byte[] dptData, int offset = 0)
        {
            base.Decode(dptData, offset);
            return (dptData[offset], dptData[offset + 1], dptData[offset + 2]);
        }

        #region specific xlator instances
        public static DPType232 DPT_RGB = new DPType232("232.600", "RGB", (0,0,0), (255,255,255), "r g b");

        internal static void Init()
        {
            DatapointTypesList.AddOrReplace(DPT_RGB);
        }
        #endregion
    }
}
