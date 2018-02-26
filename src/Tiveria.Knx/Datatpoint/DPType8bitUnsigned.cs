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
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Tiveria.Common.Extensions;


namespace Tiveria.Knx.Datapoint
{
    public class DPType8bitUnsigned : DPType<ushort>
    {
        public DPType8bitUnsigned(string id, string name, ushort min = 0, ushort max = 0, string unit = "", string description = "") : base(id, name, min, max, unit, description)
        { }

        public override byte[] ToData(string value)
        {
            var val = ushort.Parse(value);
            return ToData(val);
        }

        public override byte[] ToData(double value)
        {
            return ToData((ushort)value);
        }

        public override byte[] ToData(ushort value)
        {
            return new byte[] { ScaleToData(value) };
        }

        private byte ScaleToData(ushort value)
        {
            if (value < Minimum || value > Maximum)
                throw new ArgumentOutOfRangeException($"Value out of range [{Minimum}..{Maximum}] for DatapointType {Id} {Name}");
            if (this == DPT_SCALING)
                return (byte) Math.Round(value * 255.0f / 100);
            if (this == DPT_ANGLE)
                return (byte) Math.Round(value * 255.0f / 360);
            return (byte) value;
        }

        public override string ToStringValue(byte[] data)
        {
            return ToValue(data).ToString();
        }

        public override double ToDoubleValue(byte[] data)
        {
            return ToValue(data);
        }

        public override ushort ToValue(byte[] data)
        {
            return ScaleToValue(data[1]);
        }

        private ushort ScaleToValue(byte data)
        {
            if (this == DPT_SCALING)
                return (ushort) Math.Round(data * 100.0f / 255);
            if (this == DPT_ANGLE)
                return (ushort) Math.Round(data * 360.0f / 255);
            return (ushort)data;
        }

        public static readonly DPType8bitUnsigned DPT_SCALING = new DPType8bitUnsigned("5.001", "Scaling", 0, 100, "%");
        public static readonly DPType8bitUnsigned DPT_ANGLE = new DPType8bitUnsigned("5.003", "Angle", 0, 360, "\u00b0");
        public static readonly DPType8bitUnsigned DPT_PERCENT_U8 = new DPType8bitUnsigned("5.004", "Percent (8 Bit)", 0, 255, "%");
        public static readonly DPType8bitUnsigned DPT_DECIMALFACTOR = new DPType8bitUnsigned("5.005", "Decimal factor", 0, 255, "ratio");
        public static readonly DPType8bitUnsigned DPT_TARIFF = new DPType8bitUnsigned("5.006", "Tariff information", 0, 254);
        public static readonly DPType8bitUnsigned DPT_VALUE_1_UCOUNT = new DPType8bitUnsigned("5.010", "Unsigned count", 0, 255, "counter pulses");

        static DPType8bitUnsigned()
        {
            DatapointTypesList.AddOrReplace(DPT_ANGLE);
            DatapointTypesList.AddOrReplace(DPT_SCALING);
            DatapointTypesList.AddOrReplace(DPT_PERCENT_U8);
            DatapointTypesList.AddOrReplace(DPT_SCALING);
            DatapointTypesList.AddOrReplace(DPT_TARIFF);
            DatapointTypesList.AddOrReplace(DPT_VALUE_1_UCOUNT);
        }
    }
}
