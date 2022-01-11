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

namespace Tiveria.Home.Knx.Datapoint
{
    public class DPType17 : DPType<int>
    {
        protected DPType17(string id, string name, string unit = "", string description = "") : base(id, name, 0, 63, unit, description)
        {
            DataSize = 1;
        }

        public override byte[] Encode(int value)
        {
            if (value < Minimum || value > Maximum)
                throw new ArgumentOutOfRangeException($"Value must be in range [{Minimum}..{Maximum}]");
            return new byte[1] { (byte)(value & 0b0011_1111) };                
        }

        public override int Decode(byte[] dptData, int offset = 0)
        {
            base.Decode(dptData, offset);
            return dptData[offset] & 0b0011_1111;
        }

        #region specific xlator instances
        public static DPType17 DPT_SCENE_NUMBER = new DPType17("17.001", "Scene Number");

        static DPType17()
        {
            DatapointTypesList.AddOrReplace(DPT_SCENE_NUMBER);
        }
        #endregion
    }


}
