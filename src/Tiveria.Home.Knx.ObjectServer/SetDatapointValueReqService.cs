using System;
using System.Collections.Generic;
using System.Text;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.ObjectServer
{

    public class SetDatapointValueReqService : IObjectServerService
    {
        public const byte MainService = 0xF0;
        public const byte SubService = 0x06;

        public IReadOnlyList<DataPoint> DataPoints => _dataPoints; 
        public ushort StartDataPoint { get; private set; }
        public ushort NumberOfDataPoints { get; private set; }
        public int Size => _size;
        public ushort ServiceIdentifier => 0xF006;

        private int _size = 0;
        private readonly List<DataPoint> _dataPoints = new List<DataPoint>();

        public SetDatapointValueReqService(Span<byte> rawdata)
        {
            ParseHeader(rawdata);
            ParseDatapoints(rawdata.Slice(6));
        }

        private void ParseHeader(Span<byte> rawdata)
        {
            if (rawdata.Length < 6)
                KnxBufferSizeException.TooSmall("SetDatapointReqService");

            if (rawdata[0] != MainService || rawdata[1] != SubService)
                KnxBufferFieldException.WrongValueAt("SetDatapointReqService", "Main/Subervice", 0);

            StartDataPoint = (ushort)((rawdata[2] << 8) + rawdata[3]);
            NumberOfDataPoints = (ushort)((rawdata[4] << 8) + rawdata[5]);

            if (NumberOfDataPoints == 0 && rawdata.Length > 6)
                KnxBufferSizeException.TooBig("SetDatapointReqService");

            if (NumberOfDataPoints > 0 && rawdata.Length == 6)
                KnxBufferSizeException.TooSmall("SetDatapointReqService");

            _size = 6;
        }

        private void ParseDatapoints(Span<byte> data)
        {
            if (NumberOfDataPoints == 0)
                return;
            int pos = 0;
            for (int index = 0; index < NumberOfDataPoints; index++)
            {
                ushort dpid = (ushort)((data[pos + 0] << 8) + data[pos + 1]);
                byte dpcmd = data[pos + 2];
                byte dplen = data[pos + 3];
                byte[] dpdata = data.Slice(pos + 4, dplen).ToArray();
                pos = pos + 4 + dplen;
                _dataPoints.Add(new DataPoint(dpid, dpcmd, dplen, dpdata));
            }
            _size = _size + pos;
        }

        public byte[] ToBytes()
        {
            if (_size == 0)
                return null;
            var buffer = new byte[_size];
            WriteToBuffer(buffer);
            return buffer;
        }

        public void WriteToBuffer(Span<byte> buffer)
        {
            if (_size == 0)
                return;
            if (buffer.Length < _size)
                KnxBufferSizeException.TooSmall("SetDatapointValueReqService");

            buffer[0] = SetDatapointValueReqService.MainService;
            buffer[1] = SetDatapointValueReqService.SubService;
            buffer[2] = (byte)(StartDataPoint >> 8);
            buffer[3] = (byte)StartDataPoint;
            buffer[4] = (byte)(NumberOfDataPoints >> 8);
            buffer[5] = (byte)NumberOfDataPoints;
            int pos = 6;
            foreach (var dp in DataPoints)
            {
                buffer[pos + 0] = (byte)(dp.ID >> 8);
                buffer[pos + 1] = (byte)(dp.ID);
                buffer[pos + 2] = dp.Command;
                buffer[pos + 3] = dp.Length;
                ((Span<byte>)(dp.Value)).CopyTo(buffer.Slice(pos + 4));
                pos = pos + 4 + dp.Length;
            }
        }
    }
}
