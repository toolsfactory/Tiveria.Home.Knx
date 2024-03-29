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

using System.Globalization;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.Datapoint
{
    public class DPType8 : DPType<int>
    {
        #region constructor
        protected DPType8(string id, string name, int min, int max, string unit = "", string description = "") : base(id, name, min, max, unit, description)
        {
            DataSize = 2;
        }
        #endregion

        #region encoding
        private static byte[] InternalEncode(short value)
        {
            return new byte[] { (byte)(value >> 8), (byte)(value & 0xff) };
        }

        public override byte[] Encode(int value)
        {
            if (value < Minimum || value > Maximum)
                throw new KnxTranslationException($"value out of range [{Minimum}..{Maximum}]");
            if (this == DPT_DELTATIME_10MS)
                return InternalEncode((short)(value / 10));
            if (this == DPT_DELTATIME_100MS)
                return InternalEncode((short)(value / 100));
            return InternalEncode((short)value);
        }

        protected override byte[] EncodeFromLong(long value)
        {
            return Encode((int)value);
        }

        protected override byte[] EncodeFromULong(long value)
        {
            return Encode((int)value);
        }

        protected override byte[] EncodeFromDouble(double value)
        {
            return Encode((short)value);
        }

        protected override byte[] EncodeFromString(string value)
        {
            return base.EncodeFromString(value);
        }
        #endregion

        #region decoding
        public override int Decode(byte[] dptData, int offset = 0)
        {
            if (dptData.Length - offset < DataSize)
                throw new Exceptions.KnxTranslationException("Data size invalid");
            var value = (int)(dptData[0] << 8 + dptData[1]);
            if (this == DPT_DELTATIME_10MS)
                return value * 10;
            if (this == DPT_DELTATIME_100MS)
                return value * 100;
            return value;
        }

        public override string DecodeString(byte[] dptData, int offset = 0, bool withUnit = false, bool invariant = false)
        {
            var value = Decode(dptData, offset);
            if (this == DPT_DELTATIME_10MS)
                value *= 10;
            else if (this == DPT_DELTATIME_100MS)
                value *= 100;
            var ext = (withUnit & !String.IsNullOrEmpty(Unit)) ? " " + Unit : "";
            return String.Format(invariant ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture, "{0}{1}", value, ext);
        }
        #endregion



        #region specific xlator instances
        public static readonly DPType8 DPT_VALUE_2_COUNT = new("8.001", "16bit Counter", -32768, 32767, "pulses");
        public static readonly DPType8 DPT_DELTATIME_1MS = new("8.002", "Time Delta 1ms resolution", -32768, 32767, "ms");
        public static readonly DPType8 DPT_DELTATIME_10MS = new("8.003", "Time Delta 10ms resolution", -327680, 327670, "ms");
        public static readonly DPType8 DPT_DELTATIME_100MS = new("8.004", "Time Delta 100ms resolution", -3276800, 3276700, "ms");
        public static readonly DPType8 DPT_DELTATIME_SEC = new("8.005", "Time Delta Seconds", -32768, 32767, "sec");
        public static readonly DPType8 DPT_TIMEPERIOD_MIN = new("8.006", "Time Delta Minutes", -32768, 32767, "min");
        public static readonly DPType8 DPT_TIMEPERIOD_HRS = new("8.007", "Time Delta Hours", -32768, 32767, "hrs");
        public static readonly DPType8 DPT_ROTATION_ANGLE = new("8.011", "Rotation Angle", -32768, 32767, "°");

        // Datatype 8.010 not yet implemented

        internal static void Init()
        {
            DatapointTypesList.AddOrReplace(DPT_VALUE_2_COUNT);
            DatapointTypesList.AddOrReplace(DPT_DELTATIME_1MS);
            DatapointTypesList.AddOrReplace(DPT_DELTATIME_10MS);
            DatapointTypesList.AddOrReplace(DPT_DELTATIME_100MS);
            DatapointTypesList.AddOrReplace(DPT_DELTATIME_SEC);
            DatapointTypesList.AddOrReplace(DPT_TIMEPERIOD_MIN);
            DatapointTypesList.AddOrReplace(DPT_TIMEPERIOD_HRS);
            DatapointTypesList.AddOrReplace(DPT_ROTATION_ANGLE);
        }
        #endregion
    }
}
