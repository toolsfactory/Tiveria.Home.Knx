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

namespace Tiveria.Knx.Datapoint
{
    public class DPType11 : DPType<(int day, int month, int year)>
    {
        protected DPType11(string id, string name, (int day, int month, int year) min, (int day, int month, int year) max, string unit = "", string description = "") : base(id, name, min, max, unit, description)
        {
            DataSize = 3;
        }

        public override (int day, int month, int year) Decode(byte[] dptData, int offset = 0)
        {
            base.Decode(dptData, offset);
            var day = dptData[offset];
            var month = dptData[offset + 1];
            int year = dptData[offset + 2];
            year = year + ((year > 89) ? 1900 : 2000);
            return (day, month, year);
        }

        public override byte[] Encode((int day, int month, int year) value)
        {
            if (value.day < 1 || value.day > 31)
                throw new ArgumentOutOfRangeException("Day must be in range [1..31]");
            if (value.month < 1 || value.month > 12)
                throw new ArgumentOutOfRangeException("Month must be in range [1..12]");
            if (value.year < 1990 || value.year > 2089)
                throw new ArgumentOutOfRangeException("Year must be in range [1990..2089]");
            return new byte[3] { (byte) value.day, (byte) value.month, (byte) (value.year % 100) };
        }

        #region specific xlator instances
        public static DPType11 DPT_DATE = new DPType11("11.001", "Date", (1, 1, 1990), (31, 12, 2089), "yyyy-mm-dd");

        static DPType11()
        {
            DatapointTypesList.AddOrReplace(DPT_DATE);
        }
        #endregion
    }
}
