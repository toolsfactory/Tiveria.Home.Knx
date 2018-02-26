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

using Tiveria.Knx.Exceptions;

namespace Tiveria.Knx.Datapoint
{
    public class DPType1BitControl : DPType<(bool Value, bool Control)>
    {
        private readonly string _allowedTrue;
        private readonly string _allowedFalse;

        public string AllowedTrue { get; }
        public string AllowedFalse { get; }

        public DPType1BitControl(string id, string name, string allowedtrue, string allowedfalse, string unit = "", string description = "")
            : base(id, name, (false, false), (true, true), unit, description)
        {
            AllowedTrue = allowedtrue;
            AllowedFalse = allowedfalse;
            _allowedTrue = allowedtrue.Trim().ToLower();
            _allowedFalse = allowedfalse.Trim().ToLower();
            DataSize = 0;
        }

        public override byte[] ToData((bool Value, bool Control) value)
        {
            var data = new byte[] { (byte)(value.Value ? 1 : 0) };
            data[0] = (byte)(value.Control ? data[0] | 0x02 : data[0]);
            return data;
        }

        public override byte[] ToData(string value)
        {
            return ToData(value, true);
        }

        public byte[] ToData(string value, bool control)
        {
            if (value.ToLower() == _allowedTrue)
                return ToData((true, control));
            if (value.ToLower() == _allowedFalse)
                return ToData((false, control));
            throw new Exceptions.TranslationExcception("translation error, value not recognized");
        }


        public override byte[] ToData(double value)
        {
            return ToData(value, true);
        }

        public byte[] ToData(double value, bool control)
        {
            if (value == 0)
                return ToData((false, control));
            if (value == 1)
                return ToData((true, control));
            throw new Exceptions.TranslationExcception("translation error, value not recognized");
        }

        public byte[] ToData(long value, bool control)
        {
            if (value == 0)
                return ToData((false, control));
            if (value == 1)
                return ToData((true, control));
            throw new Exceptions.TranslationExcception("translation error, value not recognized");
        }

        public override double ToDoubleValue(byte[] data, int offset = 0)
        {
            return ToValue(data, offset).Value ? 1 : 0;
        }

        public override string ToStringValue(byte[] data, int offset = 0)
        {
            return ToValue(data, offset).Value ? AllowedTrue : AllowedFalse;
        }

        public override (bool Value, bool Control) ToValue(byte[] data, int offset = 0)
        {
            if (data.Length - offset < 1)
                throw new TranslationExcception("Data can not be translated to 1bit value");
            return ((data[offset] & 0x01) != 0, (data[offset] & 0x02) != 0);
        }

        public static readonly DPType1BitControl DPT_SWITCH_CTRL = new DPType1BitControl("2.001", "Switch Controlled", "On", "Off");
        public static readonly DPType1BitControl DPT_BOOL_CTRL = new DPType1BitControl("2.002", "Bool Controlled", "True", "False");
        public static readonly DPType1BitControl DPT_ENABLE_CTRL = new DPType1BitControl("2.003", "Enable Controlled", "Enable", "Disable");
        public static readonly DPType1BitControl DPT_RAMP_CTRL = new DPType1BitControl("2.004", "Ramp Controlled", "Ramp", "No Ramp");
        public static readonly DPType1BitControl DPT_ALARM_CTRL = new DPType1BitControl("2.005", "Alarm Controlled", "Alarm", "No Alarm");
        public static readonly DPType1BitControl DPT_BINARYVALUE_CTRL = new DPType1BitControl("2.006", "BinaryValue Controlled", "High", "Low");
        public static readonly DPType1BitControl DPT_STEP_CTRL = new DPType1BitControl("2.007", "Step Controlled", "Increase", "Decrease");
        public static readonly DPType1BitControl DPT_UPDOWN_CTRL = new DPType1BitControl("2.008", "UpDown Controlled", "Down", "Up");
        public static readonly DPType1BitControl DPT_OPENCLOSE_CTRL = new DPType1BitControl("2.009", "OpenClose Controlled", "Close", "Open");
        public static readonly DPType1BitControl DPT_START_CTRL = new DPType1BitControl("2.010", "Start Controlled", "Start", "Stop");
        public static readonly DPType1BitControl DPT_STATE_CTRL = new DPType1BitControl("2.011", "State Controlled", "Active", "Inactive");
        public static readonly DPType1BitControl DPT_INVERT_CTRL = new DPType1BitControl("2.012", "Invert Controlled", "Inverted", "Not inverted");

        static DPType1BitControl()
        {
            DatapointTypesList.AddOrReplace(DPT_SWITCH_CTRL);
            DatapointTypesList.AddOrReplace(DPT_BOOL_CTRL);
            DatapointTypesList.AddOrReplace(DPT_ENABLE_CTRL);
            DatapointTypesList.AddOrReplace(DPT_RAMP_CTRL);
            DatapointTypesList.AddOrReplace(DPT_ALARM_CTRL);
            DatapointTypesList.AddOrReplace(DPT_BINARYVALUE_CTRL);
            DatapointTypesList.AddOrReplace(DPT_STEP_CTRL);
            DatapointTypesList.AddOrReplace(DPT_UPDOWN_CTRL);
            DatapointTypesList.AddOrReplace(DPT_OPENCLOSE_CTRL);
            DatapointTypesList.AddOrReplace(DPT_START_CTRL);
            DatapointTypesList.AddOrReplace(DPT_STATE_CTRL);
            DatapointTypesList.AddOrReplace(DPT_INVERT_CTRL);
        }
    }
}
