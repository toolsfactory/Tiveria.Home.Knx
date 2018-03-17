using System;
using System.Collections.Generic;
using System.Text;

namespace Tiveria.Knx.Datapoint
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
