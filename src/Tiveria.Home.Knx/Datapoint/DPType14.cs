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

namespace Tiveria.Home.Knx.Datapoint
{
    public class DPType14 : DPType<float>
    {
        public DPType14(string id, string name, string unit = "", string description = "") : base(id, name, float.MinValue, float.MaxValue, unit, description)
        {
            DataSize = 4;
        }

        public override byte[] Encode(float value)
        {
            return BitConverter.GetBytes(value);
        }

        public override float Decode(byte[] dptData, int offset = 0)
        {
            base.Decode(dptData, offset);
            var data = new byte[4];
            Array.Copy(dptData, offset, data, 0, 4);
            Array.Reverse(data);
            return BitConverter.ToSingle(data, offset);
        }

        #region specific xlator instances
        public static DPType14 DPT_ACCELERATION = new DPType14("14.000", "Acceleration", "ms\u207B²");
        public static DPType14 DPT_ACCELERATION_ANGULAR = new DPType14("14.001", "Acceleration, angular", "rad s\u207B²");
        public static DPType14 DPT_ACTIVATION_ENERGY = new DPType14("14.002", "Activation energy", "J/mol");
        public static DPType14 DPT_ACTIVITY = new DPType14("14.003", "Activity", "s\u207B¹");
        public static DPType14 DPT_MOL = new DPType14("14.004", "Mol", "mol");
        public static DPType14 DPT_AMPLITUDE = new DPType14("14.005", "Amplitude");
        public static DPType14 DPT_ANGLE_RAD = new DPType14("14.006", "Angle", "rad");
        public static DPType14 DPT_ANGLE_DEG = new DPType14("14.007", "Angle", "°");
        public static DPType14 DPT_ANGULAR_MOMENTUM = new DPType14("14.008", "Momentum", "Js");
        public static DPType14 DPT_ANGULAR_VELOCITY = new DPType14("14.009", "Angular velocity", "rad/s");
        public static DPType14 DPT_AREA = new DPType14("14.010", "Area",  "m²");
        public static DPType14 DPT_CAPACITANCE = new DPType14("14.011", "Capacitance", "F");
        public static DPType14 DPT_CHARGE_DENSITY_SURFACE = new DPType14("14.012", "Charge density (surface)",  "C m\u207B²");
        public static DPType14 DPT_CHARGE_DENSITY_VOLUME = new DPType14("14.013", "Charge density (volume)",  "C m\u207B³");
        public static DPType14 DPT_COMPRESSIBILITY = new DPType14("14.014", "Compressibility", "m²/N");
        public static DPType14 DPT_CONDUCTANCE = new DPType14("14.015", "Conductance",  "Ω\u207B¹");
        public static DPType14 DPT_ELECTRICAL_CONDUCTIVITY = new DPType14("14.016","Conductivity, electrical", "Ω\u207B¹m\u207B¹");
        public static DPType14 DPT_DENSITY = new DPType14("14.017", "Density", "kg m\u207B³");
        public static DPType14 DPT_ELECTRIC_CHARGE = new DPType14("14.018", "Electric charge", "C");
        public static DPType14 DPT_ELECTRIC_CURRENT = new DPType14("14.019", "Electric current", "A");
        public static DPType14 DPT_ELECTRIC_CURRENTDENSITY = new DPType14("14.020", "Electric current density", "A m\u207B²");
        public static DPType14 DPT_ELECTRIC_DIPOLEMOMENT = new DPType14("14.021", "Electric dipole moment", "Cm");
        public static DPType14 DPT_ELECTRIC_DISPLACEMENT = new DPType14("14.022", "Electric displacement", "C m\u207B²");
        public static DPType14 DPT_ELECTRIC_FIELDSTRENGTH = new DPType14("14.023", "Electric field strength", "V/m");
        public static DPType14 DPT_ELECTRIC_FLUX = new DPType14("14.024", "Electric flux", "Vm");
        public static DPType14 DPT_ELECTRIC_FLUX_DENSITY = new DPType14("14.025", "Electric flux density", "C m\u207B²");
        public static DPType14 DPT_ELECTRIC_POLARIZATION = new DPType14("14.026", "Electric polarization", "C m\u207B²");
        public static DPType14 DPT_ELECTRIC_POTENTIAL = new DPType14("14.027", "Electric potential", "V");
        public static DPType14 DPT_ELECTRIC_POTENTIAL_DIFFERENCE = new DPType14("14.028", "Electric potential difference", "V");
        public static DPType14 DPT_ELECTROMAGNETIC_MOMENT = new DPType14("14.029", "Electromagnetic moment", "A m²");
        public static DPType14 DPT_ELECTROMOTIVE_FORCE = new DPType14("14.030", "Electromotive force", "V");
        public static DPType14 DPT_ENERGY = new DPType14("14.031", "Energy", "J");
        public static DPType14 DPT_FORCE = new DPType14("14.032", "Force", "N");
        public static DPType14 DPT_FREQUENCY = new DPType14("14.033", "Frequency", "Hz");
        public static DPType14 DPT_ANGULAR_FREQUENCY = new DPType14("14.034", "Frequency, angular","rad/s");
        public static DPType14 DPT_HEAT_CAPACITY = new DPType14("14.035", "Heat capacity", "J/K");
        public static DPType14 DPT_HEAT_FLOWRATE = new DPType14("14.036", "Heat flow rate", "W");
        public static DPType14 DPT_HEAT_QUANTITY = new DPType14("14.037", "Heat quantity", "J");
        public static DPType14 DPT_IMPEDANCE = new DPType14("14.038", "Impedance", "Ω");
        public static DPType14 DPT_LENGTH = new DPType14("14.039", "Length", "m");
        public static DPType14 DPT_LIGHT_QUANTITY = new DPType14("14.040", "Quantity of Light","J");
        public static DPType14 DPT_LUMINANCE = new DPType14("14.041", "Luminance", "cd m\u207B²");
        public static DPType14 DPT_LUMINOUS_FLUX = new DPType14("14.042", "Luminous flux", "lm");
        public static DPType14 DPT_LUMINOUS_INTENSITY = new DPType14("14.043", "Luminous intensity", "cd");
        public static DPType14 DPT_MAGNETIC_FIELDSTRENGTH = new DPType14("14.044", "Magnetic field strength", "A/m");
        public static DPType14 DPT_MAGNETIC_FLUX = new DPType14("14.045", "Magnetic flux", "Wb");
        public static DPType14 DPT_MAGNETIC_FLUX_DENSITY = new DPType14("14.046", "Magnetic flux density", "T");
        public static DPType14 DPT_MAGNETIC_MOMENT = new DPType14("14.047", "Magnetic moment", "A m²");
        public static DPType14 DPT_MAGNETIC_POLARIZATION = new DPType14("14.048", "Magnetic polarization", "T");
        public static DPType14 DPT_MAGNETIZATION = new DPType14("14.049", "Magnetization", "A/m");
        public static DPType14 DPT_MAGNETOMOTIVE_FORCE = new DPType14("14.050", "Magneto motive force", "A");
        public static DPType14 DPT_MASS = new DPType14("14.051", "Mass",  "kg");
        public static DPType14 DPT_MASS_FLUX = new DPType14("14.052", "Mass flux",  "kg/s");
        public static DPType14 DPT_MOMENTUM = new DPType14("14.053", "Momentum", "N/s");
        public static DPType14 DPT_PHASE_ANGLE_RAD = new DPType14("14.054", "Phase angle, radiant", "rad");
        public static DPType14 DPT_PHASE_ANGLE_DEG = new DPType14("14.055", "Phase angle, degree", "°");
        public static DPType14 DPT_POWER = new DPType14("14.056", "Power", "W");
        public static DPType14 DPT_POWER_FACTOR = new DPType14("14.057", "Power factor");
        public static DPType14 DPT_PRESSURE = new DPType14("14.058", "Pressure",  "Pa");
        public static DPType14 DPT_REACTANCE = new DPType14("14.059", "Reactance", "Ω");
        public static DPType14 DPT_RESISTANCE = new DPType14("14.060", "Resistance", "Ω");
        public static DPType14 DPT_RESISTIVITY = new DPType14("14.061", "Resistivity", "Ωm");
        public static DPType14 DPT_SELF_INDUCTANCE = new DPType14("14.062", "Self inductance", "H");
        public static DPType14 DPT_SOLID_ANGLE = new DPType14("14.063", "Solid angle", "sr");
        public static DPType14 DPT_SOUND_INTENSITY = new DPType14("14.064", "Sound intensity", "W m\u207B²");
        public static DPType14 DPT_SPEED = new DPType14("14.065", "Speed", "m/s");
        public static DPType14 DPT_STRESS = new DPType14("14.066", "Stress", "Pa");
        public static DPType14 DPT_SURFACE_TENSION = new DPType14("14.067", "Surface tension", "N/m");
        public static DPType14 DPT_COMMON_TEMPERATURE = new DPType14("14.068", "Temperature in Celsius Degree", "°C");
        public static DPType14 DPT_ABSOLUTE_TEMPERATURE = new DPType14("14.069", "Temperature, absolute", "K");
        public static DPType14 DPT_TEMPERATURE_DIFFERENCE = new DPType14("14.070", "Temperature difference", "K");
        public static DPType14 DPT_THERMAL_CAPACITY = new DPType14("14.071", "Thermal capacity", "J/K");
        public static DPType14 DPT_THERMAL_CONDUCTIVITY = new DPType14("14.072", "Thermal conductivity", "W/m K\u207B¹");
        public static DPType14 DPT_THERMOELECTRIC_POWER = new DPType14("14.073", "Thermoelectric power", "V/K");
        public static DPType14 DPT_TIME = new DPType14("14.074", "Time", "s");
        public static DPType14 DPT_TORQUE = new DPType14("14.075", "Torque", "Nm");
        public static DPType14 DPT_VOLUME = new DPType14("14.076", "Volume", "m³");
        public static DPType14 DPT_VOLUME_FLUX = new DPType14("14.077", "Volume flux", "m³/s");
        public static DPType14 DPT_WEIGHT = new DPType14("14.078", "Weight", "N");
        public static DPType14 DPT_WORK = new DPType14("14.079", "Work",  "J");


        // regex used for transform: "public static DPType14 (DPT_[A-Z0-9_]*).*\;" ==> "DatapointTypesList.AddOrReplace($1);"
        internal static void Init()
        {
            DatapointTypesList.AddOrReplace(DPT_ACCELERATION);
            DatapointTypesList.AddOrReplace(DPT_ACCELERATION_ANGULAR);
            DatapointTypesList.AddOrReplace(DPT_ACTIVATION_ENERGY);
            DatapointTypesList.AddOrReplace(DPT_ACTIVITY);
            DatapointTypesList.AddOrReplace(DPT_MOL);
            DatapointTypesList.AddOrReplace(DPT_AMPLITUDE);
            DatapointTypesList.AddOrReplace(DPT_ANGLE_RAD);
            DatapointTypesList.AddOrReplace(DPT_ANGLE_DEG);
            DatapointTypesList.AddOrReplace(DPT_ANGULAR_MOMENTUM);
            DatapointTypesList.AddOrReplace(DPT_ANGULAR_VELOCITY);
            DatapointTypesList.AddOrReplace(DPT_AREA);
            DatapointTypesList.AddOrReplace(DPT_CAPACITANCE);
            DatapointTypesList.AddOrReplace(DPT_CHARGE_DENSITY_SURFACE);
            DatapointTypesList.AddOrReplace(DPT_CHARGE_DENSITY_VOLUME);
            DatapointTypesList.AddOrReplace(DPT_COMPRESSIBILITY);
            DatapointTypesList.AddOrReplace(DPT_CONDUCTANCE);
            DatapointTypesList.AddOrReplace(DPT_ELECTRICAL_CONDUCTIVITY);
            DatapointTypesList.AddOrReplace(DPT_DENSITY);
            DatapointTypesList.AddOrReplace(DPT_ELECTRIC_CHARGE);
            DatapointTypesList.AddOrReplace(DPT_ELECTRIC_CURRENT);
            DatapointTypesList.AddOrReplace(DPT_ELECTRIC_CURRENTDENSITY);
            DatapointTypesList.AddOrReplace(DPT_ELECTRIC_DIPOLEMOMENT);
            DatapointTypesList.AddOrReplace(DPT_ELECTRIC_DISPLACEMENT);
            DatapointTypesList.AddOrReplace(DPT_ELECTRIC_FIELDSTRENGTH);
            DatapointTypesList.AddOrReplace(DPT_ELECTRIC_FLUX);
            DatapointTypesList.AddOrReplace(DPT_ELECTRIC_FLUX_DENSITY);
            DatapointTypesList.AddOrReplace(DPT_ELECTRIC_POLARIZATION);
            DatapointTypesList.AddOrReplace(DPT_ELECTRIC_POTENTIAL);
            DatapointTypesList.AddOrReplace(DPT_ELECTRIC_POTENTIAL_DIFFERENCE);
            DatapointTypesList.AddOrReplace(DPT_ELECTROMAGNETIC_MOMENT);
            DatapointTypesList.AddOrReplace(DPT_ELECTROMOTIVE_FORCE);
            DatapointTypesList.AddOrReplace(DPT_ENERGY);
            DatapointTypesList.AddOrReplace(DPT_FORCE);
            DatapointTypesList.AddOrReplace(DPT_FREQUENCY);
            DatapointTypesList.AddOrReplace(DPT_ANGULAR_FREQUENCY);
            DatapointTypesList.AddOrReplace(DPT_HEAT_CAPACITY);
            DatapointTypesList.AddOrReplace(DPT_HEAT_FLOWRATE);
            DatapointTypesList.AddOrReplace(DPT_HEAT_QUANTITY);
            DatapointTypesList.AddOrReplace(DPT_IMPEDANCE);
            DatapointTypesList.AddOrReplace(DPT_LENGTH);
            DatapointTypesList.AddOrReplace(DPT_LIGHT_QUANTITY);
            DatapointTypesList.AddOrReplace(DPT_LUMINANCE);
            DatapointTypesList.AddOrReplace(DPT_LUMINOUS_FLUX);
            DatapointTypesList.AddOrReplace(DPT_LUMINOUS_INTENSITY);
            DatapointTypesList.AddOrReplace(DPT_MAGNETIC_FIELDSTRENGTH);
            DatapointTypesList.AddOrReplace(DPT_MAGNETIC_FLUX);
            DatapointTypesList.AddOrReplace(DPT_MAGNETIC_FLUX_DENSITY);
            DatapointTypesList.AddOrReplace(DPT_MAGNETIC_MOMENT);
            DatapointTypesList.AddOrReplace(DPT_MAGNETIC_POLARIZATION);
            DatapointTypesList.AddOrReplace(DPT_MAGNETIZATION);
            DatapointTypesList.AddOrReplace(DPT_MAGNETOMOTIVE_FORCE);
            DatapointTypesList.AddOrReplace(DPT_MASS);
            DatapointTypesList.AddOrReplace(DPT_MASS_FLUX);
            DatapointTypesList.AddOrReplace(DPT_MOMENTUM);
            DatapointTypesList.AddOrReplace(DPT_PHASE_ANGLE_RAD);
            DatapointTypesList.AddOrReplace(DPT_PHASE_ANGLE_DEG);
            DatapointTypesList.AddOrReplace(DPT_POWER);
            DatapointTypesList.AddOrReplace(DPT_POWER_FACTOR);
            DatapointTypesList.AddOrReplace(DPT_PRESSURE);
            DatapointTypesList.AddOrReplace(DPT_REACTANCE);
            DatapointTypesList.AddOrReplace(DPT_RESISTANCE);
            DatapointTypesList.AddOrReplace(DPT_RESISTIVITY);
            DatapointTypesList.AddOrReplace(DPT_SELF_INDUCTANCE);
            DatapointTypesList.AddOrReplace(DPT_SOLID_ANGLE);
            DatapointTypesList.AddOrReplace(DPT_SOUND_INTENSITY);
            DatapointTypesList.AddOrReplace(DPT_SPEED);
            DatapointTypesList.AddOrReplace(DPT_STRESS);
            DatapointTypesList.AddOrReplace(DPT_SURFACE_TENSION);
            DatapointTypesList.AddOrReplace(DPT_COMMON_TEMPERATURE);
            DatapointTypesList.AddOrReplace(DPT_ABSOLUTE_TEMPERATURE);
            DatapointTypesList.AddOrReplace(DPT_TEMPERATURE_DIFFERENCE);
            DatapointTypesList.AddOrReplace(DPT_THERMAL_CAPACITY);
            DatapointTypesList.AddOrReplace(DPT_THERMAL_CONDUCTIVITY);
            DatapointTypesList.AddOrReplace(DPT_THERMOELECTRIC_POWER);
            DatapointTypesList.AddOrReplace(DPT_TIME);
            DatapointTypesList.AddOrReplace(DPT_TORQUE);
            DatapointTypesList.AddOrReplace(DPT_VOLUME);
            DatapointTypesList.AddOrReplace(DPT_VOLUME_FLUX);
            DatapointTypesList.AddOrReplace(DPT_WEIGHT);
            DatapointTypesList.AddOrReplace(DPT_WORK);
        }
    }
    #endregion
}
