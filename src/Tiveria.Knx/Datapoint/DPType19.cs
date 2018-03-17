namespace Tiveria.Knx.Datapoint
{
    public class DPType19 : DPType<ComplexDateTime>
    {
        protected DPType19(string id, string name, string unit = "", string description = "") : base(id, name, default, default, unit, description)
        {
            DataSize = 8;
        }

        public override byte[] Encode(ComplexDateTime value)
        {
            var data = new byte[DataSize];
            data[0] = (byte)(value.Year - 1900);
            data[1] = value.Month;
            data[2] = value.DayOfMonth;
            data[3] = (byte)(((byte)value.DayOfWeek << 5) | (value.HourOfDay & 0b0001_1111));
            data[4] = value.Minutes;
            data[5] = value.Seconds;

            data[6] = (byte)(data[6] | (value.Fault ? 0b1000_0000 : 0));
            data[6] = (byte)(data[6] | (value.WorkingDay ? 0b0100_0000 : 0));
            data[6] = (byte)(data[6] | (value.NoWorkingDay ? 0b0010_0000 : 0));
            data[6] = (byte)(data[6] | (value.NoYear ? 0b0001_0000 : 0));
            data[6] = (byte)(data[6] | (value.NoDate ? 0b0000_1000 : 0));
            data[6] = (byte)(data[6] | (value.NoDayOfWeek ? 0b0000_0100 : 0));
            data[6] = (byte)(data[6] | (value.NoTime ? 0b0000_0010 : 0));
            data[6] = (byte)(data[6] | (value.StandardSummerTime ? 0b0000_0001 : 0));
            data[7] = (byte)(data[7] | (value.QualityOfClock ? 0b1000_0000 : 0));
            return data;
        }

        public override ComplexDateTime Decode(byte[] dptData, int offset = 0)
        {
            base.Decode(dptData, offset);
            var result = new ComplexDateTime();
            result.Year  = (short) (dptData[offset] + 1900);
            result.Month = (byte)  (dptData[offset + 1] & 0b0000_1111);
            result.DayOfMonth = (byte) (dptData[offset + 2] & 0b0001_1111);
            result.DayOfWeek = (DayOfWeek) ((dptData[offset + 3] & 0b1110_0000) >> 5);
            result.HourOfDay = (byte) (dptData[offset + 3] & 0b0001_1111);
            result.Minutes = (byte)(dptData[offset + 4] & 0b0011_1111);
            result.Seconds = (byte)(dptData[offset + 5] & 0b0011_1111);

            result.Fault        = (dptData[offset + 6] & 0b1000_0000) != 0;
            result.WorkingDay   = (dptData[offset + 6] & 0b0100_0000) != 0;
            result.NoWorkingDay = (dptData[offset + 6] & 0b0010_0000) != 0;
            result.NoYear       = (dptData[offset + 6] & 0b0001_0000) != 0;
            result.NoDate       = (dptData[offset + 6] & 0b0000_1000) != 0;
            result.NoDayOfWeek  = (dptData[offset + 6] & 0b0000_0100) != 0;
            result.NoTime       = (dptData[offset + 6] & 0b0000_0010) != 0;
            result.StandardSummerTime = (dptData[offset + 6] & 0b0000_0001) != 0;
            result.QualityOfClock     = (dptData[offset + 7] & 0b1000_0000) != 0;
            return result;
        }

        #region specific xlator instances
        public static DPType19 DPT_DATETIME = new DPType19("19.001", "Date with time");

        static DPType19()
        {
            DatapointTypesList.AddOrReplace(DPT_DATETIME);
        }
        #endregion
    }
}
