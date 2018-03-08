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
        int DataSize { get; }

        byte[] Encode(object value);
        string DecodeString(byte[] dptData, int offset = 0, bool withUnit = false);
        object DecodeObject(byte[] dptData, int offset = 0);

        bool IsMainCategory(int category);
    }
}