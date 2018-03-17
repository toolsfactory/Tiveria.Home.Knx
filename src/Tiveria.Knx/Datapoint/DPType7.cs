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
using Tiveria.Knx.Exceptions;

namespace Tiveria.Knx.Datapoint
{
    public class DPType7 : DPType<uint>
    {
        #region constructor
        public DPType7(string id, string name, uint min, uint max, string unit = "", string description = "") : base(id, name, min, max, unit, description)
        {
            DataSize = 2;
        }
        #endregion

        #region encoding
        private byte[] InternalEncode(ushort value)
        {
            return new byte[] { (byte)(value >> 8), (byte)(value & 0xff) };
        }

        public override byte[] Encode(uint value)
        {
            if (value < Minimum || value > Maximum)
                throw new TranslationException($"value out of range [{Minimum}..{Maximum}]");
            if (this == DPT_TIMEPERIOD_10MS)
                return InternalEncode((ushort)(value / 10));
            if (this == DPT_TIMEPERIOD_100MS)
                return InternalEncode((ushort)(value / 100));
            return InternalEncode((ushort)value);
        }

        protected override byte[] EncodeFromLong(long value)
        {
            return Encode((uint)value);
        }

        protected override byte[] EncodeFromULong(long value)
        {
            return Encode((uint)value);
        }

        protected override byte[] EncodeFromDouble(double value)
        {
            return Encode((uint)value);
        }

        protected override byte[] EncodeFromString(string value)
        {
            return base.EncodeFromString(value);
        }
        #endregion

        #region decoding
        public override uint Decode(byte[] dptData, int offset = 0)
        {
            if (dptData.Length - offset < DataSize)
                throw new Exceptions.TranslationException("Data size invalid");
            var value = (uint)(dptData[0] << 8 + dptData[1]);
            if (this == DPT_TIMEPERIOD_10MS)
                return value * 10;
            if (this == DPT_TIMEPERIOD_100MS)
                return value * 100;
            return value;
        }

        public override string DecodeString(byte[] dptData, int offset = 0, bool withUnit = false)
        {
            ulong value = Decode(dptData, offset);
            if (this == DPT_TIMEPERIOD_10MS)
                value = value * 10;
            else if (this == DPT_TIMEPERIOD_100MS)
                value = value * 100;
            return withUnit ? $"{value} {Unit}" : value.ToString();
        }
        #endregion

        #region specific xlator instances
        public static DPType7 DPT_VALUE_2_UCOUNT   = new DPType7("7.001", "16bit Unsigned Counter", 0, 65535, "pulses");
        public static DPType7 DPT_TIMEPERIOD_1MS   = new DPType7("7.002", "Time Period 1ms resolution", 0, 65535, "ms");
        public static DPType7 DPT_TIMEPERIOD_10MS  = new DPType7("7.003", "Time Period 10ms resolution", 0, 65535, "ms");
        public static DPType7 DPT_TIMEPERIOD_100MS = new DPType7("7.004", "Time Period 100ms resolution", 0, 65535, "ms");
        public static DPType7 DPT_TIMEPERIOD_SEC   = new DPType7("7.005", "Time Period Seconds", 0, 65535, "sec");
        public static DPType7 DPT_TIMEPERIOD_MIN   = new DPType7("7.006", "Time Period Minutes", 0, 65535, "min");
        public static DPType7 DPT_TIMEPERIOD_HRS   = new DPType7("7.007", "Time Period Hours", 0, 65535, "hrs");

        public static DPType7 DPT_PROP_DATATYPE    = new DPType7("7.010", "Interface object property ID", 0, 65535, "");
        public static DPType7 DPT_LENGTH_MM        = new DPType7("7.011", "Length in mm", 0, 65535, "mm");
        public static DPType7 DPT_ELECTRICAL_CUR   = new DPType7("7.012", "Electrical current", 0, 65535, "mA");
        public static DPType7 DPT_BRIGHTNESS       = new DPType7("7.013", "Brightness", 0, 65535, "ls");

        static DPType7()
        {
            DatapointTypesList.AddOrReplace(DPT_VALUE_2_UCOUNT);
            DatapointTypesList.AddOrReplace(DPT_TIMEPERIOD_1MS);
            DatapointTypesList.AddOrReplace(DPT_TIMEPERIOD_10MS);
            DatapointTypesList.AddOrReplace(DPT_TIMEPERIOD_100MS);
            DatapointTypesList.AddOrReplace(DPT_TIMEPERIOD_SEC);
            DatapointTypesList.AddOrReplace(DPT_TIMEPERIOD_MIN);
            DatapointTypesList.AddOrReplace(DPT_TIMEPERIOD_HRS);
            DatapointTypesList.AddOrReplace(DPT_PROP_DATATYPE);
            DatapointTypesList.AddOrReplace(DPT_LENGTH_MM);
            DatapointTypesList.AddOrReplace(DPT_ELECTRICAL_CUR);
            DatapointTypesList.AddOrReplace(DPT_BRIGHTNESS);
        }
        #endregion
    }
}
