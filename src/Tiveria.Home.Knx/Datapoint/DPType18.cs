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
    public class DPType18 : DPType<(SceneControlMode Mode, int Scene)>
    {
        protected DPType18(string id, string name, (SceneControlMode Mode, int Scene) min, (SceneControlMode Mode, int Scene) max, string unit = "", string description = "") : base(id, name, min, max, unit, description)
        {
            DataSize = 1;
        }

        public override byte[] Encode((SceneControlMode Mode, int Scene) value)
        {
            if (value.Scene < Minimum.Scene || value.Scene > Maximum.Scene)
                throw new ArgumentOutOfRangeException($"Value must be in range [{Minimum.Scene}..{Maximum.Scene}]");
            return new byte[1] { (byte)((value.Scene & 0b0011_1111) | (value.Mode == SceneControlMode.Learn ? 0b1000_0000 : 0)) };
        }

        public override (SceneControlMode Mode, int Scene) Decode(byte[] dptData, int offset = 0)
        {
            (SceneControlMode Mode, int Scene) result;
            result.Scene = dptData[offset] & 0b0011_1111;
            result.Mode = (dptData[offset] & 0b1000_0000) > 0 ? SceneControlMode.Learn : SceneControlMode.Activate;
            return result;
        }

        #region specific xlator instances
        public static DPType18 DPT_SCENE_CONTROL = new DPType18("18.001", "Scene Control", (SceneControlMode.Learn, 0), (SceneControlMode.Activate, 63));

        static DPType18()
        {
            DatapointTypesList.AddOrReplace(DPT_SCENE_CONTROL);
        }
        #endregion
    }
}
