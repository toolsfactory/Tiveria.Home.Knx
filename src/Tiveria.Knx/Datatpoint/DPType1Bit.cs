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
using Tiveria.Common.Extensions;
using Tiveria.Knx.Exceptions;

namespace Tiveria.Knx.Datapoint
{
    public class DPType1Bit : DPType<bool>
    {
        private readonly string _allowedTrue;
        private readonly string _allowedFalse;

        public string AllowedTrue { get; }
        public string AllowedFalse { get; }

        public DPType1Bit(string id, string name, string allowedtrue, string allowedfalse, string unit = "", string description = "") 
            : base(id, name, false, true, unit, description)
        {
            AllowedTrue = allowedtrue;
            AllowedFalse = allowedfalse;
            _allowedTrue = allowedtrue.Trim().ToLower();
            _allowedFalse = allowedfalse.Trim().ToLower();
        }

        public override byte[] ToData(bool value)
        {
            return new byte[] { (byte)(value ? 1 : 0) };
        }

        public override byte[] ToData(string value)
        {
            if (value.ToLower() == _allowedTrue)
                return ToData(true);
            if (value.ToLower() == _allowedFalse)
                return ToData(false);
            throw new Exceptions.TranslationExcception("translation error, value not recognized");
        }

        public override byte[] ToData(double value)
        {
            if (value == 0)
                return ToData(false);
            if (value == 1)
                return ToData(true);
            throw new Exceptions.TranslationExcception("translation error, value not recognized");
        }

        public override byte[] ToData(long value)
        {
            if (value == 0)
                return ToData(false);
            if (value == 1)
                return ToData(true);
            throw new Exceptions.TranslationExcception("translation error, value not recognized");
        }

        public override double ToDoubleValue(byte[] data)
        {
            return ToValue(data) ? 1 : 0;
        }

        public override long ToLongValue(byte[] data)
        {
            return ToValue(data) ? 1 : 0;
        }

        public override string ToStringValue(byte[] data)
        {
            return ToValue(data) ? AllowedTrue : AllowedFalse;
        }

        public override bool ToValue(byte[] data)
        {
            if(data.Length == 1)
                return (data[0] & 0x01) != 0;
            else if (data.Length == 2)
                return (data[1] & 0x01) != 0;
            throw new TranslationExcception("Data can not be translated to 1bit value");
        }

        public override bool ToBoolValue(byte[] data)
        {
            return ToValue(data);
        }

        public static readonly DPType1Bit DPT_SWITCH      = new DPType1Bit("1.001", "Switch", "On", "Off");
        public static readonly DPType1Bit DPT_BOOL        = new DPType1Bit("1.002", "Bool", "True", "False");
        public static readonly DPType1Bit DPT_ENABLE      = new DPType1Bit("1.003", "Enable", "Enable", "Disable");
        public static readonly DPType1Bit DPT_RAMP        = new DPType1Bit("1.004", "Ramp", "Ramp", "No Ramp");
        public static readonly DPType1Bit DPT_ALARM       = new DPType1Bit("1.005", "Alarm", "Alarm", "No Alarm");
        public static readonly DPType1Bit DPT_BINARYVALUE = new DPType1Bit("1.006", "BinaryValue", "High", "Low");
        public static readonly DPType1Bit DPT_STEP        = new DPType1Bit("1.007", "Step", "Increase", "Decrease");
        public static readonly DPType1Bit DPT_UPDOWN      = new DPType1Bit("1.008", "UpDown", "Down", "Up");
        public static readonly DPType1Bit DPT_OPENCLOSE   = new DPType1Bit("1.009", "OpenClose", "Close", "Open");
        public static readonly DPType1Bit DPT_START       = new DPType1Bit("1.010", "Start", "Start", "Stop");
        public static readonly DPType1Bit DPT_STATE       = new DPType1Bit("1.011", "State", "Active", "Inactive");
        public static readonly DPType1Bit DPT_INVERT      = new DPType1Bit("1.012", "Invert", "Inverted", "Not inverted");
        public static readonly DPType1Bit DPT_DIMSENDSTYLE= new DPType1Bit("1.013", "DimSendStyle", "Cyclically", "Start/Stop");
        public static readonly DPType1Bit DPT_INPUTSOURCE = new DPType1Bit("1.014", "InputSource", "Calculated", "Fixed");
        public static readonly DPType1Bit DPT_RESET       = new DPType1Bit("1.015", "Reset", "TriggerReset", "NoAction");
        public static readonly DPType1Bit DPT_ACK         = new DPType1Bit("1.016", "Ack", "Acknowledge", "NoAction");
        public static readonly DPType1Bit DPT_TRIGGER     = new DPType1Bit("1.017", "Trigger", "trigger", "trigger"); //For DPT_Trigger, both values 0 and 1 shall have the same effect and shall not be differentiated in sender or receiver. 
        public static readonly DPType1Bit DPT_OCCUPANCY   = new DPType1Bit("1.018", "Occupancy", "occupied", "not occupied");
        public static readonly DPType1Bit DPT_WINDOWDOOOR = new DPType1Bit("1.019", "Window_Door", "open", "closed");
        public static readonly DPType1Bit DPT_LOGICALFUNC = new DPType1Bit("1.021", "LogicalFunction", "AND", "OR");
        public static readonly DPType1Bit DPT_SCENEAB     = new DPType1Bit("1.022", "Scene A/B", "Scene B", "Scene A");
        public static readonly DPType1Bit DPT_SBMODE      = new DPType1Bit("1.023", "Shutter/Blinds Mode", "move up/down + step-stop", "only move up/down");
        public static readonly DPType1Bit DPT_HEAT_COOL   = new DPType1Bit("1.100", "Heat/Cool", "heating", "cooling");

        static DPType1Bit()
        {
            DatapointTypesList.AddOrReplace(DPT_SWITCH);
            DatapointTypesList.AddOrReplace(DPT_BOOL);
            DatapointTypesList.AddOrReplace(DPT_ENABLE);
            DatapointTypesList.AddOrReplace(DPT_RAMP);
            DatapointTypesList.AddOrReplace(DPT_ALARM);
            DatapointTypesList.AddOrReplace(DPT_BINARYVALUE);
            DatapointTypesList.AddOrReplace(DPT_STEP);
            DatapointTypesList.AddOrReplace(DPT_UPDOWN);
            DatapointTypesList.AddOrReplace(DPT_OPENCLOSE);
            DatapointTypesList.AddOrReplace(DPT_START);
            DatapointTypesList.AddOrReplace(DPT_STATE);
            DatapointTypesList.AddOrReplace(DPT_INVERT);
            DatapointTypesList.AddOrReplace(DPT_DIMSENDSTYLE);
            DatapointTypesList.AddOrReplace(DPT_INPUTSOURCE);
            DatapointTypesList.AddOrReplace(DPT_RESET);
            DatapointTypesList.AddOrReplace(DPT_ACK);
            DatapointTypesList.AddOrReplace(DPT_TRIGGER);
            DatapointTypesList.AddOrReplace(DPT_OCCUPANCY);
            DatapointTypesList.AddOrReplace(DPT_WINDOWDOOOR);
            DatapointTypesList.AddOrReplace(DPT_LOGICALFUNC);
            DatapointTypesList.AddOrReplace(DPT_SCENEAB);
            DatapointTypesList.AddOrReplace(DPT_SBMODE);
            DatapointTypesList.AddOrReplace(DPT_HEAT_COOL);
        }

    }
}
