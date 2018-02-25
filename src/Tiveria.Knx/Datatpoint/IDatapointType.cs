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
        byte[] ToData(long value);
        
        string ToStringValue(byte[] data);
        double ToDoubleValue(byte[] data);
        long ToLongValue(byte[] data);
        bool ToBoolValue(byte[] data);
    }

    public interface IDatapointType<TValue> : IDatapointType
    {
        TValue Maximum { get; }
        TValue Minimum { get; }

        byte[] ToData(TValue value);
        TValue ToValue(byte[] data);
    }
}