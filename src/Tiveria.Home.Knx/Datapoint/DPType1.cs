﻿/*
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

using Tiveria.Home.Knx.Exceptions;
using System.Runtime.CompilerServices;

namespace Tiveria.Home.Knx.Datapoint
{
    public class DPType1 : DPType<bool>
    {
        #region private fields
        protected readonly string _allowedTrue;
        protected readonly string _allowedFalse;
        #endregion

        #region Additional Properties
        public string AllowedTrue { get; }
        public string AllowedFalse { get; }
        #endregion

        #region constructor
        public DPType1(string id, string name, string allowedtrue, string allowedfalse, string unit = "", string description = "")
            : base(id, name, false, true, unit, description)
        {
            AllowedTrue = allowedtrue;
            AllowedFalse = allowedfalse;
            _allowedTrue = allowedtrue.Trim().ToLower();
            _allowedFalse = allowedfalse.Trim().ToLower();
            DataSize = 0;
        }
        #endregion

        #region decoding dpt data
        public override bool Decode(byte[] dptData, int offset = 0)
        {
            return (bool)DecodeObject(dptData, offset);
        }

        public override string DecodeString(byte[] dptData, int offset = 0, bool withUnit = false, bool invariant = false)
        {
            return (bool)DecodeObject(dptData, offset) ? AllowedTrue : AllowedFalse;
        }

        public override object DecodeObject(byte[] dptData, int offset = 0)
        {
            if (dptData.Length - offset < 1)
                throw new KnxTranslationException("Data can not be translated to 1bit value");
            return (dptData[offset] & 0x01) != 0;
        }
        #endregion

        #region encoding dpt data
        public override byte[] Encode(bool value)
        {
            return EncodeFromBool(value);
        }

        protected override byte[] EncodeFromLong(long value)
        {
            if (value == 0)
                return EncodeFromBool(false);
            if (value == 1)
                return EncodeFromBool(true);
            throw new Exceptions.KnxTranslationException("translation error, value not recognized");
        }

        protected override byte[] EncodeFromULong(long value)
        {
            if (value == 0)
                return EncodeFromBool(false);
            if (value == 1)
                return EncodeFromBool(true);
            throw new Exceptions.KnxTranslationException("translation error, value not recognized");
        }

        protected override byte[] EncodeFromBool(bool value)
        {
            return new byte[] { (byte)(value ? 1 : 0) };
        }

        protected override byte[] EncodeFromDouble(double value)
        {
            if (value == 0)
                return EncodeFromBool(false);
            if (value == 1)
                return EncodeFromBool(true);
            throw new Exceptions.KnxTranslationException("translation error, value not recognized");
        }

        protected override byte[] EncodeFromString(string value)
        {
            if (value.ToLower() == _allowedTrue)
                return EncodeFromBool(true);
            if (value.ToLower() == _allowedFalse)
                return EncodeFromBool(false);
            throw new Exceptions.KnxTranslationException("translation error, value not recognized");
        }
        #endregion

        #region specific xlator instances
        public static readonly DPType1 DPT_SWITCH      = new("1.001", "Switch", "On", "Off");
        public static readonly DPType1 DPT_BOOL        = new("1.002", "Bool", "True", "False");
        public static readonly DPType1 DPT_ENABLE      = new("1.003", "Enable", "Enable", "Disable");
        public static readonly DPType1 DPT_RAMP        = new("1.004", "Ramp", "Ramp", "No Ramp");
        public static readonly DPType1 DPT_ALARM       = new("1.005", "Alarm", "Alarm", "No Alarm");
        public static readonly DPType1 DPT_BINARYVALUE = new("1.006", "BinaryValue", "High", "Low");
        public static readonly DPType1 DPT_STEP        = new("1.007", "Step", "Increase", "Decrease");
        public static readonly DPType1 DPT_UPDOWN      = new("1.008", "UpDown", "Down", "Up");
        public static readonly DPType1 DPT_OPENCLOSE   = new("1.009", "OpenClose", "Close", "Open");
        public static readonly DPType1 DPT_START       = new("1.010", "Start", "Start", "Stop");
        public static readonly DPType1 DPT_STATE       = new("1.011", "State", "Active", "Inactive");
        public static readonly DPType1 DPT_INVERT      = new("1.012", "Invert", "Inverted", "Not inverted");
        public static readonly DPType1 DPT_DIMSENDSTYLE= new("1.013", "DimSendStyle", "Cyclically", "Start/Stop");
        public static readonly DPType1 DPT_INPUTSOURCE = new("1.014", "InputSource", "Calculated", "Fixed");
        public static readonly DPType1 DPT_RESET       = new("1.015", "Reset", "TriggerReset", "NoAction");
        public static readonly DPType1 DPT_ACK         = new("1.016", "Ack", "Acknowledge", "NoAction");
        public static readonly DPType1 DPT_TRIGGER     = new("1.017", "Trigger", "trigger", "trigger"); //For DPT_Trigger, both values 0 and 1 shall have the same effect and shall not be differentiated in sender or receiver. 
        public static readonly DPType1 DPT_OCCUPANCY   = new("1.018", "Occupancy", "occupied", "not occupied");
        public static readonly DPType1 DPT_WINDOWDOOOR = new("1.019", "Window_Door", "open", "closed");
        public static readonly DPType1 DPT_LOGICALFUNC = new("1.021", "LogicalFunction", "AND", "OR");
        public static readonly DPType1 DPT_SCENEAB     = new("1.022", "Scene A/B", "Scene B", "Scene A");
        public static readonly DPType1 DPT_SBMODE      = new("1.023", "Shutter/Blinds Mode", "move up/down + step-stop", "only move up/down");
        public static readonly DPType1 DPT_HEAT_COOL   = new("1.100", "Heat/Cool", "heating", "cooling");

        internal static void Init()
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
        #endregion
    }
}
