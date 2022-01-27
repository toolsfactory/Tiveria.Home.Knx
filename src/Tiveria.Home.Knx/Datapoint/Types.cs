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


namespace Tiveria.Home.Knx.Datapoint
{
    public enum SceneControlMode
    {
        Activate,
        Learn
    }

    public enum DayOfWeek
    {
        NoDay = 0,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public struct TimeOfDay
    {
        private byte _hour;
        private byte _minute;
        private byte _second;

        public DayOfWeek Day { get; set; }
        public byte Hour { get => _hour; set => _hour = (value < 24) ? value : (byte)0; }
        public byte Minute { get => _minute; set => _minute = (value < 60) ? value : (byte)0; }
        public byte Second { get => _second; set => _second = (value < 60) ? value : (byte)0; }

        public TimeOfDay(DayOfWeek day, byte hour = 0, byte minute = 0, byte second = 0)
        {
            _hour = 0;
            _minute = 0;
            _second = 0;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
        }

        public override string ToString()
        {
            return $"{Day}, {Hour}:{Minute:00}:{Second:00}";
        }
    }

    public struct ComplexDateTime
    {
        private byte _minute;
        private byte _second;
        private short _year;
        private byte _month;
        private byte _dayOfMonth;
        private byte _hour;

        public ComplexDateTime(DateTime dt) : this()
        {
            _year = (byte)dt.Year;
            _month = (byte)dt.Month;
            _dayOfMonth = (byte)dt.Day;
            _hour = (byte)dt.Hour;
            _minute = (byte)dt.Minute;
            _second = (byte)dt.Second;
            DayOfWeek = (DayOfWeek)dt.DayOfWeek;
        }

        public short Year { get => _year; set => _year = value; }
        public byte Month { get => _month; set => _month = value; }
        public byte DayOfMonth { get => _dayOfMonth; set => _dayOfMonth = value; }
        public DayOfWeek DayOfWeek { get; set; }
        public byte HourOfDay { get => _hour; set => _hour = (value < 24) ? value : (byte)0; }
        public byte Minutes { get => _minute; set => _minute = (value < 60) ? value : (byte)0; }
        public byte Seconds { get => _second; set => _second = (value < 60) ? value : (byte)0; }
        public bool Fault { get; set; }
        public bool WorkingDay { get; set; }
        public bool NoWorkingDay { get; set; }
        public bool NoYear { get; set; }
        public bool NoDate { get; set; }
        public bool NoDayOfWeek { get; set; }
        public bool NoTime { get; set; }
        public bool StandardSummerTime { get; set; }
        public bool QualityOfClock { get; set; }
    }
}
