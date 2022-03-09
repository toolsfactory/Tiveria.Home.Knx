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
    public class DPType10 : DPType<TimeOfDay>
    {
        protected DPType10(string id, string name, string unit = "", string description = "") : base(id, name, new TimeOfDay(DayOfWeek.NoDay, 0, 0, 0), new TimeOfDay(DayOfWeek.Sunday, 23, 59, 59), unit, description)
        {
            DataSize = 3;
        }

        public override TimeOfDay Decode(byte[] dptData, int offset = 0)
        {
            base.Decode(dptData, offset);
            var result = new TimeOfDay();
            result.Day = (DayOfWeek)(dptData[offset] >> 5);
            result.Hour = (byte)(dptData[offset] & 0b0001_1111);
            result.Minute = (byte)(dptData[offset + 1] & 0b0011_1111);
            result.Second = (byte)(dptData[offset + 2] & 0b0011_1111);
            return result;
        }

        public override byte[] Encode(TimeOfDay value)
        {
            var data = new byte[3];
            var day = ((int)value.Day) << 5;
            data[0] = (byte)(day + value.Hour);
            data[1] = value.Minute;
            data[2] = value.Second;
            return data;
        }

        #region specific xlator instances
        public static DPType10 DPT_TIMEOFDAY = new DPType10("10.001", "Time of day", "dow, hh:mm:ss");

        internal static void Init()
        {
            DatapointTypesList.AddOrReplace(DPT_TIMEOFDAY);
        }
        #endregion
    }
}
