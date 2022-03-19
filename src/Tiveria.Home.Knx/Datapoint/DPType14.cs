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
        public static readonly DPType14 DPT_ACCELERATION = new ("14.000", "Acceleration", "ms\u207B²");
        public static readonly DPType14 DPT_ACCELERATION_ANGULAR = new ("14.001", "Acceleration, angular", "rad s\u207B²");
        public static readonly DPType14 DPT_ACTIVATION_ENERGY = new("14.002", "Activation energy", "J/mol");
        public static readonly DPType14 DPT_ACTIVITY = new("14.003", "Activity", "s\u207B¹");
        public static readonly DPType14 DPT_MOL = new("14.004", "Mol", "mol");
        public static readonly DPType14 DPT_AMPLITUDE = new("14.005", "Amplitude");
        public static readonly DPType14 DPT_ANGLE_RAD = new("14.006", "Angle", "rad");
        public static readonly DPType14 DPT_ANGLE_DEG = new("14.007", "Angle", "°");
        public static readonly DPType14 DPT_ANGULAR_MOMENTUM = new("14.008", "Momentum", "Js");
        public static readonly DPType14 DPT_ANGULAR_VELOCITY = new("14.009", "Angular velocity", "rad/s");
        public static readonly DPType14 DPT_AREA = new("14.010", "Area",  "m²");
        public static readonly DPType14 DPT_CAPACITANCE = new("14.011", "Capacitance", "F");
        public static readonly DPType14 DPT_CHARGE_DENSITY_SURFACE = new("14.012", "Charge density (surface)",  "C m\u207B²");
        public static readonly DPType14 DPT_CHARGE_DENSITY_VOLUME = new("14.013", "Charge density (volume)",  "C m\u207B³");
        public static readonly DPType14 DPT_COMPRESSIBILITY = new("14.014", "Compressibility", "m²/N");
        public static readonly DPType14 DPT_CONDUCTANCE = new("14.015", "Conductance",  "Ω\u207B¹");
        public static readonly DPType14 DPT_ELECTRICAL_CONDUCTIVITY = new("14.016","Conductivity, electrical", "Ω\u207B¹m\u207B¹");
        public static readonly DPType14 DPT_DENSITY = new("14.017", "Density", "kg m\u207B³");
        public static readonly DPType14 DPT_ELECTRIC_CHARGE = new("14.018", "Electric charge", "C");
        public static readonly DPType14 DPT_ELECTRIC_CURRENT = new("14.019", "Electric current", "A");
        public static readonly DPType14 DPT_ELECTRIC_CURRENTDENSITY = new("14.020", "Electric current density", "A m\u207B²");
        public static readonly DPType14 DPT_ELECTRIC_DIPOLEMOMENT = new("14.021", "Electric dipole moment", "Cm");
        public static readonly DPType14 DPT_ELECTRIC_DISPLACEMENT = new("14.022", "Electric displacement", "C m\u207B²");
        public static readonly DPType14 DPT_ELECTRIC_FIELDSTRENGTH = new("14.023", "Electric field strength", "V/m");
        public static readonly DPType14 DPT_ELECTRIC_FLUX = new("14.024", "Electric flux", "Vm");
        public static readonly DPType14 DPT_ELECTRIC_FLUX_DENSITY = new("14.025", "Electric flux density", "C m\u207B²");
        public static readonly DPType14 DPT_ELECTRIC_POLARIZATION = new("14.026", "Electric polarization", "C m\u207B²");
        public static readonly DPType14 DPT_ELECTRIC_POTENTIAL = new("14.027", "Electric potential", "V");
        public static readonly DPType14 DPT_ELECTRIC_POTENTIAL_DIFFERENCE = new("14.028", "Electric potential difference", "V");
        public static readonly DPType14 DPT_ELECTROMAGNETIC_MOMENT = new("14.029", "Electromagnetic moment", "A m²");
        public static readonly DPType14 DPT_ELECTROMOTIVE_FORCE = new("14.030", "Electromotive force", "V");
        public static readonly DPType14 DPT_ENERGY = new("14.031", "Energy", "J");
        public static readonly DPType14 DPT_FORCE = new("14.032", "Force", "N");
        public static readonly DPType14 DPT_FREQUENCY = new("14.033", "Frequency", "Hz");
        public static readonly DPType14 DPT_ANGULAR_FREQUENCY = new("14.034", "Frequency, angular","rad/s");
        public static readonly DPType14 DPT_HEAT_CAPACITY = new("14.035", "Heat capacity", "J/K");
        public static readonly DPType14 DPT_HEAT_FLOWRATE = new("14.036", "Heat flow rate", "W");
        public static readonly DPType14 DPT_HEAT_QUANTITY = new("14.037", "Heat quantity", "J");
        public static readonly DPType14 DPT_IMPEDANCE = new("14.038", "Impedance", "Ω");
        public static readonly DPType14 DPT_LENGTH = new("14.039", "Length", "m");
        public static readonly DPType14 DPT_LIGHT_QUANTITY = new("14.040", "Quantity of Light","J");
        public static readonly DPType14 DPT_LUMINANCE = new("14.041", "Luminance", "cd m\u207B²");
        public static readonly DPType14 DPT_LUMINOUS_FLUX = new("14.042", "Luminous flux", "lm");
        public static readonly DPType14 DPT_LUMINOUS_INTENSITY = new("14.043", "Luminous intensity", "cd");
        public static readonly DPType14 DPT_MAGNETIC_FIELDSTRENGTH = new("14.044", "Magnetic field strength", "A/m");
        public static readonly DPType14 DPT_MAGNETIC_FLUX = new("14.045", "Magnetic flux", "Wb");
        public static readonly DPType14 DPT_MAGNETIC_FLUX_DENSITY = new("14.046", "Magnetic flux density", "T");
        public static readonly DPType14 DPT_MAGNETIC_MOMENT = new("14.047", "Magnetic moment", "A m²");
        public static readonly DPType14 DPT_MAGNETIC_POLARIZATION = new("14.048", "Magnetic polarization", "T");
        public static readonly DPType14 DPT_MAGNETIZATION = new("14.049", "Magnetization", "A/m");
        public static readonly DPType14 DPT_MAGNETOMOTIVE_FORCE = new("14.050", "Magneto motive force", "A");
        public static readonly DPType14 DPT_MASS = new("14.051", "Mass",  "kg");
        public static readonly DPType14 DPT_MASS_FLUX = new("14.052", "Mass flux",  "kg/s");
        public static readonly DPType14 DPT_MOMENTUM = new("14.053", "Momentum", "N/s");
        public static readonly DPType14 DPT_PHASE_ANGLE_RAD = new("14.054", "Phase angle, radiant", "rad");
        public static readonly DPType14 DPT_PHASE_ANGLE_DEG = new("14.055", "Phase angle, degree", "°");
        public static readonly DPType14 DPT_POWER = new("14.056", "Power", "W");
        public static readonly DPType14 DPT_POWER_FACTOR = new("14.057", "Power factor");
        public static readonly DPType14 DPT_PRESSURE = new("14.058", "Pressure",  "Pa");
        public static readonly DPType14 DPT_REACTANCE = new("14.059", "Reactance", "Ω");
        public static readonly DPType14 DPT_RESISTANCE = new("14.060", "Resistance", "Ω");
        public static readonly DPType14 DPT_RESISTIVITY = new("14.061", "Resistivity", "Ωm");
        public static readonly DPType14 DPT_SELF_INDUCTANCE = new("14.062", "Self inductance", "H");
        public static readonly DPType14 DPT_SOLID_ANGLE = new("14.063", "Solid angle", "sr");
        public static readonly DPType14 DPT_SOUND_INTENSITY = new("14.064", "Sound intensity", "W m\u207B²");
        public static readonly DPType14 DPT_SPEED = new("14.065", "Speed", "m/s");
        public static readonly DPType14 DPT_STRESS = new("14.066", "Stress", "Pa");
        public static readonly DPType14 DPT_SURFACE_TENSION = new("14.067", "Surface tension", "N/m");
        public static readonly DPType14 DPT_COMMON_TEMPERATURE = new("14.068", "Temperature in Celsius Degree", "°C");
        public static readonly DPType14 DPT_ABSOLUTE_TEMPERATURE = new("14.069", "Temperature, absolute", "K");
        public static readonly DPType14 DPT_TEMPERATURE_DIFFERENCE = new("14.070", "Temperature difference", "K");
        public static readonly DPType14 DPT_THERMAL_CAPACITY = new("14.071", "Thermal capacity", "J/K");
        public static readonly DPType14 DPT_THERMAL_CONDUCTIVITY = new("14.072", "Thermal conductivity", "W/m K\u207B¹");
        public static readonly DPType14 DPT_THERMOELECTRIC_POWER = new("14.073", "Thermoelectric power", "V/K");
        public static readonly DPType14 DPT_TIME = new("14.074", "Time", "s");
        public static readonly DPType14 DPT_TORQUE = new("14.075", "Torque", "Nm");
        public static readonly DPType14 DPT_VOLUME = new("14.076", "Volume", "m³");
        public static readonly DPType14 DPT_VOLUME_FLUX = new("14.077", "Volume flux", "m³/s");
        public static readonly DPType14 DPT_WEIGHT = new("14.078", "Weight", "N");
        public static readonly DPType14 DPT_WORK = new("14.079", "Work",  "J");


        // regex used for transform: "public static readonly DPType14 (DPT_[A-Z0-9_]*).*\;" ==> "DatapointTypesList.AddOrReplace($1);"
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
