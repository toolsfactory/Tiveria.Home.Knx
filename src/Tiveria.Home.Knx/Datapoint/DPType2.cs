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

using Tiveria.Home.Knx.Exceptions;
using Tiveria.Common.Extensions;

namespace Tiveria.Home.Knx.Datapoint
{

    public class DPType2 : DPType1
    {
        public DPType2(string id, string name, string allowedtrue, string allowedfalse, string unit = "", string description = "") : base(id, name, allowedtrue, allowedfalse, unit, description)
        { }

        #region decoding
        public override string DecodeString(byte[] dptData, int offset = 0, bool withUnit = false, bool invariant = false)
        {
            var (Value, Control) = DecodeControlled(dptData, offset);
            string result = Value ? _allowedTrue : _allowedFalse;
            if (Control)
                return result + " controlled";
            else
                return result + " not controlled";
        }

        public (bool Value, bool Control) DecodeControlled(byte[] dptData, int offset = 0)
        {
            if (dptData.Length - offset < 1)
                throw new KnxTranslationException("Data can not be translated to 1bit value");
            return ((dptData[offset] & 0x01) != 0, (dptData[offset] & 0x02) != 0);
        }

        public override object DecodeObject(byte[] dptData, int offset = 0)
        {
            if (dptData.Length - offset < 1)
                throw new KnxTranslationException("Data can not be translated to 1bit value");
            return dptData[offset] & 0x01;
        }
        #endregion

        #region encoding
        public byte[] Encode(bool value, bool controlled)
        {
            var data = new byte[] { (byte)(value ? 1 : 0) };
            data[0] = (byte)(controlled ? data[0] | 0x02 : data[0]);
            return data;
        }

        protected override byte[] EncodeFromBool(bool value)
        {
            return Encode(value, true);
        }

        protected override byte[] EncodeFromString(string value)
        {
            bool val, ctrl = false;
            var items = value.ToLower().NormalizeWhiteSpaces().Split(' ');
            if (items.Length > 3 || items.Length < 1)
                throw new Exceptions.KnxTranslationException("translation error, value not recognized");
            val = ValueFromString(items[0]);
            ctrl = CtrlFromItems(items);
            return Encode(val, ctrl);
        }

        private bool CtrlFromItems(string[] items)
        {
            if (items.Length == 2 && items[2] == "controlled")
                return true;
            if (items.Length == 2 && items[1] == "not" && items[3] == "controlled")
                return false;
            throw new Exceptions.KnxTranslationException("translation error, value not recognized");
        }

        private bool ValueFromString(string value)
        {
            if (value.ToLower() == _allowedTrue)
                return true;
            else
                if (value.ToLower() == _allowedFalse)
                    return false;
                else
                    throw new Exceptions.KnxTranslationException("translation error, value not recognized");
        }
        #endregion

        #region specific xlator instances
        public static readonly DPType2 DPT_SWITCH_CTRL = new DPType2("2.001", "Switch Controlled", "On", "Off");
        public static readonly DPType2 DPT_BOOL_CTRL = new DPType2("2.002", "Bool Controlled", "True", "False");
        public static readonly DPType2 DPT_ENABLE_CTRL = new DPType2("2.003", "Enable Controlled", "Enable", "Disable");
        public static readonly DPType2 DPT_RAMP_CTRL = new DPType2("2.004", "Ramp Controlled", "Ramp", "No Ramp");
        public static readonly DPType2 DPT_ALARM_CTRL = new DPType2("2.005", "Alarm Controlled", "Alarm", "No Alarm");
        public static readonly DPType2 DPT_BINARYVALUE_CTRL = new DPType2("2.006", "BinaryValue Controlled", "High", "Low");
        public static readonly DPType2 DPT_STEP_CTRL = new DPType2("2.007", "Step Controlled", "Increase", "Decrease");
        public static readonly DPType2 DPT_UPDOWN_CTRL = new DPType2("2.008", "UpDown Controlled", "Down", "Up");
        public static readonly DPType2 DPT_OPENCLOSE_CTRL = new DPType2("2.009", "OpenClose Controlled", "Close", "Open");
        public static readonly DPType2 DPT_START_CTRL = new DPType2("2.010", "Start Controlled", "Start", "Stop");
        public static readonly DPType2 DPT_STATE_CTRL = new DPType2("2.011", "State Controlled", "Active", "Inactive");
        public static readonly DPType2 DPT_INVERT_CTRL = new DPType2("2.012", "Invert Controlled", "Inverted", "Not inverted");

        internal static void Init()
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
        #endregion
    }
}