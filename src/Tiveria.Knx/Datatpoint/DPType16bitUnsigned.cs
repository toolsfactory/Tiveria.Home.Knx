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
    public class DPType16bitUnsigned : DPType<uint>
    {

        protected DPType16bitUnsigned(string id, string name, uint min = 0, uint max = 0, string unit = "", string description = "") : base(id, name, min, max, unit, description)
        {
        }

        public override bool ToBoolValue(byte[] data)
        {
            throw new TranslationExcception("Bool cannot be assigned to this Datapoint");
        }

        public override byte[] ToData(string value)
        {
            return ToData(uint.Parse(value));
        }

        public override byte[] ToData(double value)
        {
            return ToData((uint)value);
        }

        public override byte[] ToData(long value)
        {
            return ToData((uint)value);
        }

        public override byte[] ToData(uint value)
        {
            if (value < Minimum || value > Maximum)
                throw new TranslationExcception($"value out of range [{Minimum}..{Maximum}]");
            var val = value;
            if (this == DPT_TIMEPERIOD_10MS)
                val = (uint)Math.Round(value / 10.0f);
            else if (this == DPT_TIMEPERIOD_100MS)
                val = (uint)Math.Round(value / 100.0f);
            return new byte[] { (byte)(val >> 8), (byte)(val & 0xff) };
        }


        public override double ToDoubleValue(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override long ToLongValue(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override string ToStringValue(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override uint ToValue(byte[] data)
        {
            throw new NotImplementedException();
        }

        public static DPType16bitUnsigned DPT_VALUE_2_UCOUNT   = new DPType16bitUnsigned("7.001", "16bit Unsigned Counter", 0, 65535, "pulses");
        public static DPType16bitUnsigned DPT_TIMEPERIOD_1MS   = new DPType16bitUnsigned("7.002", "Time Period 1ms resolution", 0, 65535, "ms");
        public static DPType16bitUnsigned DPT_TIMEPERIOD_10MS  = new DPType16bitUnsigned("7.003", "Time Period 10ms resolution", 0, 655350, "ms");
        public static DPType16bitUnsigned DPT_TIMEPERIOD_100MS = new DPType16bitUnsigned("7.004", "Time Period 100ms resolution", 0, 6553500, "ms");
        public static DPType16bitUnsigned DPT_TIMEPERIOD_SEC   = new DPType16bitUnsigned("7.005", "Time Period Seconds", 0, 65535, "sec");
        public static DPType16bitUnsigned DPT_TIMEPERIOD_MIN   = new DPType16bitUnsigned("7.006", "Time Period Minutes", 0, 65535, "min");
        public static DPType16bitUnsigned DPT_TIMEPERIOD_HRS   = new DPType16bitUnsigned("7.007", "Time Period Hours", 0, 65535, "hrs");

        public static DPType16bitUnsigned DPT_PROP_DATATYPE    = new DPType16bitUnsigned("7.010", "Interface object property ID", 0, 65535, "");
        public static DPType16bitUnsigned DPT_LENGTH_MM        = new DPType16bitUnsigned("7.011", "Length in mm", 0, 65535, "mm");
        public static DPType16bitUnsigned DPT_ELECTRICAL_CURRENT = new DPType16bitUnsigned("7.012", "Electrical current", 0, 65535, "mA");
        public static DPType16bitUnsigned DPT_BRIGHTNESS       = new DPType16bitUnsigned("7.013", "Brightness", 0, 65535, "ls");

        static DPType16bitUnsigned()
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
            DatapointTypesList.AddOrReplace(DPT_ELECTRICAL_CURRENT);
            DatapointTypesList.AddOrReplace(DPT_BRIGHTNESS);

        }
    }
}
