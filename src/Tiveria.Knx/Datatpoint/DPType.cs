using System;
using System.Collections.Generic;

namespace Tiveria.Knx.Datapoint
{
    public abstract class DPType<TValue> : IDatapointType<TValue>
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Unit { get; }
        public TValue Minimum { get; }
        public TValue Maximum { get; }
        public int DataSize { get; protected set; }

        protected DPType(string id, string name, TValue min = default, TValue max = default, string unit = "", string description = "")
        {
            Id = id;
            Name = name;
            Description = description;
            Unit = unit;
            Minimum = min;
            Maximum = max;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DPType<TValue>);
        }

        public bool Equals(IDatapointType other)
        {
            return other != null && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
        }

        public static bool operator ==(DPType<TValue> type1, DPType<TValue> type2)
        {
            return EqualityComparer<DPType<TValue>>.Default.Equals(type1, type2);
        }

        public static bool operator !=(DPType<TValue> type1, DPType<TValue> type2)
        {
            return !(type1 == type2);
        }

        public abstract byte[] ToData(TValue value);
        public abstract byte[] ToData(string value);
        public abstract byte[] ToData(double value);
        public abstract TValue ToValue(byte[] data);
        public abstract string ToStringValue(byte[] data);
        public abstract double ToDoubleValue(byte[] data);
    }
}
