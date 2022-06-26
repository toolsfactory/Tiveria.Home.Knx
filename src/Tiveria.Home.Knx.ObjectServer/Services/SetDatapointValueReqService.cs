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

    /// <summary>
    /// Represents the Device Information DIB block described in chapter 7.5.4.2 of the spec 3/8/2 KnxIPNet core.
    /// </summary>
    /// <code>
    /// +--------+--------+--------+--------+--------+--------+
    /// | byte 1 | byte 2 | byte 3 | byte 4 | byte 5 | byte 6 |
    /// +--------+--------+--------+--------+--------+--------+
    /// | Main   | Sub    | Start Datapoint | Number of Data  |
    /// | SvC    | SVC    |                 | points          |
    /// +--------+--------+-----------------+-----------------+
    /// 
    /// +--------+--------+--------+--------+--------+--------+
    /// | byte 8 | byte 9 | byte 10| byte 11| byte 12 - 12+len|
    /// +--------+--------+--------+--------+-----------------+
    /// | First DP ID     | FirstDP| FirstDP| FirstDP         |
    /// |                 | CMD    | Length | Value           |
    /// +-----------------+--------+--------+-----------------+
    /// 
    /// Second block repeated for all Datapoints 
    /// </code>
    public class SetDatapointValueReqService : IObjectServerService
    {
        public const byte MainServiceId = 0xF0;
        public const byte SubServiceId = 0x06;

        public IReadOnlyList<DataPoint> DataPoints => _dataPoints;
        public ushort StartDataPoint { get; private set; }
        public ushort NumberOfDataPoints { get; private set; }
        public int Size { get; private set; }
        public ushort ServiceTypeIdentifier => 0xF006;

        public byte MainService => MainServiceId;

        public byte SubService => SubServiceId;

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
