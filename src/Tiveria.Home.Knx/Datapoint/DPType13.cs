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
    public class DPType13 : DPType<Int32>
    {
        public DPType13(string id, string name, string unit = "", string description = "") : base(id, name, Int32.MinValue, Int32.MaxValue, unit, description)
        {
            DataSize = 4;
        }

        public override int Decode(byte[] dptData, int offset = 0)
        {
            base.Decode(dptData, offset);
            return (int)((dptData[offset] << 24) | (dptData[offset + 1] << 16) | (dptData[offset + 2] << 8) | dptData[offset + 3]);
        }

        public override byte[] Encode(int value)
        {
            return new byte[4] { (byte)((value >> 24) & 0xFF), (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF) };
        }

        #region specific xlator instances
        public static DPType13 DPT_COUNT = new DPType13("13.001", "Counter pulses", "counter pulses");
        public static DPType13 DPT_FLOWRATE = new DPType13("13.002", "Flow rate", "m3/h");
        public static DPType13 DPT_ACTIVE_ENERGY = new DPType13("13.010", "Active Energy", "Wh");
        public static DPType13 DPT_APPARENT_ENERGY = new DPType13("13.011", "Apparent energy", "VAh");
        public static DPType13 DPT_REACTIVE_ENERGY = new DPType13("13.012", "Reactive energy", "VARh");
        public static DPType13 DPT_ACTIVE_ENERGY_KWH = new DPType13("13.013", "Active energy in kWh", "kWh");
        public static DPType13 DPT_APPARENT_ENERGY_KVAH = new DPType13("13.014", "Apparent energy in kVAh", "kVAh");
        public static DPType13 DPT_REACTIVE_ENERGY_KVARH = new DPType13("13.015", "Reactive energy in kVARh",   "kVARh");
        public static DPType13 DPT_DELTA_TIME = new DPType13("13.100", "Delta time in seconds", "s");

        static DPType13()
        {
            DatapointTypesList.AddOrReplace(DPT_COUNT);
            DatapointTypesList.AddOrReplace(DPT_FLOWRATE);
            DatapointTypesList.AddOrReplace(DPT_ACTIVE_ENERGY);
            DatapointTypesList.AddOrReplace(DPT_APPARENT_ENERGY);
            DatapointTypesList.AddOrReplace(DPT_REACTIVE_ENERGY);
            DatapointTypesList.AddOrReplace(DPT_ACTIVE_ENERGY_KWH);
            DatapointTypesList.AddOrReplace(DPT_APPARENT_ENERGY_KVAH);
            DatapointTypesList.AddOrReplace(DPT_REACTIVE_ENERGY_KVARH);
            DatapointTypesList.AddOrReplace(DPT_DELTA_TIME);
        }
        #endregion
    }
}
