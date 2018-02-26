using System;
using System.Collections.Generic;

namespace Tiveria.Knx.Datapoint
{
    public interface IDatapointType : IEquatable<IDatapointType>
    {
        string Description { get; }
        string Id { get; }
        string Name { get; }
        string Unit { get; }

        byte[] ToData(string value);
        byte[] ToData(double value);
        
        string ToStringValue(byte[] data, int offset = 0);
        double ToDoubleValue(byte[] data, int offset = 0);
    }

    public interface IDatapointType<TValue> : IDatapointType
    {
        TValue Maximum { get; }
        TValue Minimum { get; }

        byte[] ToData(TValue value);
        TValue ToValue(byte[] data, int offset = 0);
    }
}