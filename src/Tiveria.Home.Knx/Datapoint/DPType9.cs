/*
    Tiveria.Home.Knx - a .Net Core base KNX library
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
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.Datapoint
{
    public class DPType9 : DPType<double>
    {
        protected DPType9(string id, string name, double min, double max, string unit = "", string description = "") : base(id, name, min, max, unit, description)
        {
        }

        public override double Decode(byte[] dptData, int offset = 0)
        {
            var sign = (dptData[offset] & 0b1000_0000) << 24;
            var mant = (dptData[offset] & 0b0000_0111) << 28 | (dptData[offset + 1] << 20) | sign;
            var exp  = (dptData[offset] & 0b0111_1000) >> 3;
            return (1 << exp) * (mant >> 20) * 0.01;
        }

        public override byte[] Encode(double value)
        {
            double v = value * 100.0f;
            int e = 0;
            for (; v < -2048.0; v /= 2)
                e++;
            for (; v > 2047.0; v /= 2)
                e++;
            var m = (int)Math.Round(v) & 0x7FF;
            var msb = (short)(e << 3 | m >> 8);
            if (value < 0.0)
                msb |= 0x80;
            var lsb = (byte)m;
            return new byte[2] { (byte)msb, lsb };
        }

        #region specific xlator instances
        public static DPType9 DPT_TEMPERATURE = new DPType9("9.001", "Temperature", -273.0, 670760.0, "°C");
        public static DPType9 DPT_TEMPERATURE_DIFFERENCE = new DPType9("9.002", "Temperature difference", -670760, 670760, "K");
        public static DPType9 DPT_TEMPERATURE_GRADIENT = new DPType9("9.003", "Temperature gradient", -670760, 670760, "K/h");
        public static DPType9 DPT_INTENSITY_OF_LIGHT = new DPType9("9.004", "Light intensity", 0, 670760, "lx");
        public static DPType9 DPT_WIND_SPEED = new DPType9("9.005", "Wind speed", 0, 670760, "m/s");
        public static DPType9 DPT_AIR_PRESSURE = new DPType9("9.006", "Air pressure", 0, 670760, "Pa");
        public static DPType9 DPT_HUMIDITY = new DPType9("9.007", "Humidity", 0, 670760, "%");
        public static DPType9 DPT_AIRQUALITY = new DPType9("9.008", "Air quality", 0, 670760, "ppm");
        public static DPType9 DPT_AIR_FLOW = new DPType9("9.009", "Air flow", -670760, 670760, "m³/h");
        public static DPType9 DPT_TIME_DIFFERENCE1 = new DPType9("9.010", "Time difference 1", -670760, 670760, "s");
        public static DPType9 DPT_TIME_DIFFERENCE2 = new DPType9("9.011", "Time difference 2", -670760, 670760, "ms");
        public static DPType9 DPT_VOLTAGE = new DPType9("9.020", "Voltage", -670760, 670760, "mV");
        public static DPType9 DPT_ELECTRICAL_CURRENT = new DPType9("9.021", "Electrical current", -670760, 670760, "mA");
        public static DPType9 DPT_POWERDENSITY = new DPType9("9.022", "Power density", -670760, 670760, "W/m²");
        public static DPType9 DPT_KELVIN_PER_PERCENT = new DPType9("9.023", "Kelvin/percent", -670760, 670760, "K/%");
        public static DPType9 DPT_POWER = new DPType9("9.024", "Power", -670760, 670760, "kW");
        public static DPType9 DPT_VOLUME_FLOW = new DPType9("9.025", "Volume flow", -670760, 670760, "l/h");
        public static DPType9 DPT_RAIN_AMOUNT = new DPType9("9.026", "Rain amount", -671088.64, 670760.96, "l/m²");
        public static DPType9 DPT_TEMP_F = new DPType9("9.027", "Temperature", -459.6, 670760.96, "°F");
        public static DPType9 DPT_WIND_SPEED_KMH = new DPType9("9.028", "Wind speed", 0, 670760.96, "km/h");

        static DPType9()
        {
            DatapointTypesList.AddOrReplace(DPT_TEMPERATURE);
            DatapointTypesList.AddOrReplace(DPT_TEMPERATURE_DIFFERENCE);
            DatapointTypesList.AddOrReplace(DPT_TEMPERATURE_GRADIENT);
            DatapointTypesList.AddOrReplace(DPT_INTENSITY_OF_LIGHT);
            DatapointTypesList.AddOrReplace(DPT_WIND_SPEED);
            DatapointTypesList.AddOrReplace(DPT_AIR_PRESSURE);
            DatapointTypesList.AddOrReplace(DPT_HUMIDITY);
            DatapointTypesList.AddOrReplace(DPT_AIRQUALITY);
            DatapointTypesList.AddOrReplace(DPT_AIR_FLOW);
            DatapointTypesList.AddOrReplace(DPT_TIME_DIFFERENCE1);
            DatapointTypesList.AddOrReplace(DPT_TIME_DIFFERENCE2);
            DatapointTypesList.AddOrReplace(DPT_VOLTAGE);
            DatapointTypesList.AddOrReplace(DPT_ELECTRICAL_CURRENT);
            DatapointTypesList.AddOrReplace(DPT_POWERDENSITY);
            DatapointTypesList.AddOrReplace(DPT_KELVIN_PER_PERCENT);
            DatapointTypesList.AddOrReplace(DPT_POWER);
            DatapointTypesList.AddOrReplace(DPT_VOLUME_FLOW);
            DatapointTypesList.AddOrReplace(DPT_RAIN_AMOUNT);
            DatapointTypesList.AddOrReplace(DPT_TEMP_F);
            DatapointTypesList.AddOrReplace(DPT_WIND_SPEED_KMH);
        }
        #endregion

    }
}
