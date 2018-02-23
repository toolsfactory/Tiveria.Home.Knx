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

        public override byte[] ToData(long value)
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

        public override long ToLongValue(byte[] data)
        {
            return ToValue(data);
        }

        public override ushort ToValue(byte[] data)
        {
            return ScaleToValue(data[0]);
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
    }

    public class DPTypeBoolean : DPType<bool>
    {
        public string AllowedTrue { get; }
        public string AllowedFalse { get; }

        public DPTypeBoolean(string id, string name, string allowedtrue, string allowedfalse, string unit = "", string description = "") 
            : base(id, name, false, true, unit, description)
        {
            AllowedTrue = allowedtrue;
            AllowedFalse = allowedfalse;
        }

        public override byte[] ToData(bool value)
        {
            return new byte[] { (byte)(value ? 1 : 0) };
        }

        public override byte[] ToData(string value)
        {
            if (AllowedTrue.ListContains(',', value))
                return ToData(true);
            if (AllowedFalse.ListContains(',', value))
                return ToData(false);
            throw new Exceptions.TranslationExcception("translation error, value not recognized");
        }.

        public override byte[] ToData(double value)
        {
            if(value == 0)
                return ToData(false);
        }

        public override byte[] ToData(long data)
        {
            throw new NotImplementedException();
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

        public override bool ToValue(byte[] data)
        {
            return (data[0] & 0x01) != 0 ? true : false;
        }
    }
}
