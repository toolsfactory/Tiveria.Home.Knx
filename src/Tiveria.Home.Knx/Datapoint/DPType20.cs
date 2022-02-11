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

using System.Reflection;

namespace Tiveria.Home.Knx.Datapoint
{

    public abstract class KnxEnum
    {
        internal static System.Collections.Generic.Dictionary<int, string> _values = new Dictionary<int, string>();
        internal static void Initialize(Type type)
        {
            foreach (var element in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (element.FieldType == typeof(Int32))
                {
                    var item = element.GetValue(null);
                    if (item !=null)
                        _values.Add((int) item, element.Name);
                }
            }
        }

        public static bool HasValue(int value)
        {
            return _values.ContainsKey(value);
        }

        public static string GetName(int value)
        {
            if (HasValue(value))
                return _values[value];
            else
                return "unknown";
        }

        public static int GetValue(string name)
        {
            name = name.ToLower();
            foreach(var item in _values)
            {
                if (item.Value.ToLower() == name)
                    return item.Key;
            }
            return -1;
            //var item = _values.FirstOrDefault(x => x.Value == name);
            //return item.Key;
        }
    }

    public sealed class DPT_SCLOModes : KnxEnum
    {
        public static int Autonomous = 0;
        public static int Slave = 1;
        public static int Master = 2;

        static DPT_SCLOModes()
        {
            Initialize(typeof(DPT_SCLOModes));
        }
    }

    public class DPType20 : DPType<byte>
    {
        public DPType20(string id, string name, byte min, byte max, string unit = "", string description = "") : base(id, name, min, max, unit, description)
        {
            DataSize = 1;
        }

        public override byte[] Encode(byte value)
        {
            if (value < Minimum || value > Maximum)
                throw new Exceptions.KnxTranslationException($"translation error, value '{value}' not allowed for {Id}");
            return new byte[1] { value };
        }

        public override byte Decode(byte[] dptData, int offset = 0)
        {
            base.Decode(dptData, offset);
            return dptData[offset];
        }

        #region specific xlator instances
        public enum SystemClockMode : byte
        {
            Autonomous = 0,
            Slave = 1,
            Master = 2
        }
        public static DPType20 DPT_SCLOMode = new DPType20("20.001", "System clock mode", 0, 2);

        public enum BuildingMode : byte
        {
            InUse = 0,
            NotUsed = 1,
            Protection = 2
        }
        public static DPType20 DPT_BuildingMode = new DPType20("20.002", "Building mode", 0, 2);

        public enum OccupancyMode : byte
        {
            Occupied = 0,
            Standby = 1,
            NotOccupied = 2
        }
        public static DPType20 DPT_OCCMode = new DPType20("20.003", "Occupancy mode", 0, 2);

        public enum Priority : byte
        {
            High = 0,
            Medium = 1,
            Low = 2,
            Void = 3
        }
        public static DPType20 DPT_Priority = new DPType20("20.004", "Priority", 0, 3);

        public enum LightApplicationMode : byte
        {
            Normal = 0,
            PresenceSimulation = 1,
            NightRound = 2
        }
        public static DPType20 DPT_LightApplicationMode = new DPType20("20.005", "Light application mode", 0, 2);

        public enum ApplicationArea : byte
        {
            NoFault = 0,
            CommonInterest = 1,
            HvacGeneralFBs = 10,
            HvacHotWaterHeating = 11,
            HvacDirectElectricalHeating = 12,
            HvacTerminalUnits = 13,
            HvacVac = 14
        }
        public static DPType20 DPT_ApplicationArea = new DPType20("20.006", "Application Arera", 0, 14);

        static DPType20()
        {
            DatapointTypesList.AddOrReplace(DPT_SCLOMode);
            DatapointTypesList.AddOrReplace(DPT_BuildingMode);
            DatapointTypesList.AddOrReplace(DPT_OCCMode);
            DatapointTypesList.AddOrReplace(DPT_Priority);
            DatapointTypesList.AddOrReplace(DPT_LightApplicationMode);
            DatapointTypesList.AddOrReplace(DPT_ApplicationArea);
        }
        #endregion
    }
}