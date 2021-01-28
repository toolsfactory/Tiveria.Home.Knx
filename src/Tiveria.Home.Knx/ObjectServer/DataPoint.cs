namespace Tiveria.Home.Knx.ObjectServer
{
    public struct DataPoint
    {
        public DataPoint(ushort id, byte command, byte length, byte[] value)
        {
            ID = id;
            Command = command;
            Length = length;
            Value = value;
        }

        public ushort ID { get; }
        public byte Command { get; }
        public byte Length { get; }
        public byte[] Value { get; }
    }
}
