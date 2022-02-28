using System;
using System.Collections.Generic;
using System.Text;
using Tiveria.Common.IO;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.IP.Services.Serializers;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.ObjectServer
{

    public class SetDatapointValueReqService : IObjectServerService
    {
        public const byte MainService = 0xF0;
        public const byte SubService = 0x06;

        public IReadOnlyList<DataPoint> DataPoints => _dataPoints;
        public ushort StartDataPoint { get; private set; }
        public ushort NumberOfDataPoints { get; private set; }
        public int Size { get; private set; }
        public ushort ServiceTypeIdentifier => 0xF006;

        byte IObjectServerService.MainService => MainService;

        byte IObjectServerService.SubService => SubService;

        private readonly List<DataPoint> _dataPoints = new List<DataPoint>();

        public SetDatapointValueReqService(ushort startDataPoint, IList<DataPoint> datapoints)
        {
            StartDataPoint = startDataPoint;
            _dataPoints = datapoints.OrderBy(dp => dp.ID).ToList();
            NumberOfDataPoints = (ushort) _dataPoints.Count;
            Size = 6 + CalcDPSize();
        }

        private int CalcDPSize()
        {
            var size = 0;
            foreach (var dp in _dataPoints)
                size += 4 + dp.Length;
            return size;
        }
    }
}
