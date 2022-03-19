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

using System.Globalization;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.Datapoint
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
        private static byte[] InternalEncode(ushort value)
        {
            return new byte[] { (byte)(value >> 8), (byte)(value & 0xff) };
        }

        public override byte[] Encode(uint value)
        {
            if (value < Minimum || value > Maximum)
                throw new KnxTranslationException($"value out of range [{Minimum}..{Maximum}]");
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
                throw new Exceptions.KnxTranslationException("Data size invalid");
            var value = (uint)((dptData[0] << 8) + dptData[1]);
            if (this == DPT_TIMEPERIOD_10MS)
                return value * 10;
            if (this == DPT_TIMEPERIOD_100MS)
                return value * 100;
            return value;
        }

        public override string DecodeString(byte[] dptData, int offset = 0, bool withUnit = false, bool invariant = false)
        {
            ulong value = Decode(dptData, offset);
            if (this == DPT_TIMEPERIOD_10MS)
                value *= 10;
            else if (this == DPT_TIMEPERIOD_100MS)
                value *= 100;
            var ext = (withUnit & !String.IsNullOrEmpty(Unit)) ? " " + Unit : "";
            return String.Format(invariant ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture, "{0}{1}", value, ext);
        }
        #endregion

        #region specific xlator instances
        public static readonly DPType7 DPT_VALUE_2_UCOUNT   = new("7.001", "16bit Unsigned Counter", 0, 65535, "pulses");
        public static readonly DPType7 DPT_TIMEPERIOD_1MS   = new("7.002", "Time Period 1ms resolution", 0, 65535, "ms");
        public static readonly DPType7 DPT_TIMEPERIOD_10MS  = new("7.003", "Time Period 10ms resolution", 0, 65535, "ms");
        public static readonly DPType7 DPT_TIMEPERIOD_100MS = new("7.004", "Time Period 100ms resolution", 0, 65535, "ms");
        public static readonly DPType7 DPT_TIMEPERIOD_SEC   = new("7.005", "Time Period Seconds", 0, 65535, "sec");
        public static readonly DPType7 DPT_TIMEPERIOD_MIN   = new("7.006", "Time Period Minutes", 0, 65535, "min");
        public static readonly DPType7 DPT_TIMEPERIOD_HRS   = new("7.007", "Time Period Hours", 0, 65535, "hrs");

        public static readonly DPType7 DPT_PROP_DATATYPE    = new("7.010", "Interface object property ID", 0, 65535, "");
        public static readonly DPType7 DPT_LENGTH_MM        = new("7.011", "Length in mm", 0, 65535, "mm");
        public static readonly DPType7 DPT_ELECTRICAL_CUR   = new("7.012", "Electrical current", 0, 65535, "mA");
        public static readonly DPType7 DPT_BRIGHTNESS       = new("7.013", "Brightness", 0, 65535, "ls");

        internal static void Init()
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
