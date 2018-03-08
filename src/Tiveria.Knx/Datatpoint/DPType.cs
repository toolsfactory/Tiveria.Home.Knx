using System;
using System.Collections.Generic;

namespace Tiveria.Knx.Datapoint
{
    public abstract class DPType<TValue> : IDatapointType
    {
        protected int _mainCategory;
        protected int _subCategory;

        #region public properties
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Unit { get; }
        public TValue Minimum { get; }
        public TValue Maximum { get; }
        public int DataSize { get; protected set; }
        #endregion

        #region constructor
        protected DPType(string id, string name, TValue min, TValue max, string unit = "", string description = "")
        {
            if (min == null || max == null)
                throw new ArgumentNullException("Min/Max need to be set");
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Name must not be null or empty");
            (Minimum, Maximum) = ValidateMinMax(min, max);
            Id = ParseId(id);
            Name = name;
            Description = description;
            Unit = unit;
        }

        protected virtual (TValue, TValue) ValidateMinMax(TValue min, TValue max)
        {
            return (min, max);
        }

    private string ParseId(string id)
        {
            if(String.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("Id must not be null or empty");
            var segments = id.Split('.');
            if(segments == null || segments.Length != 2)
                throw new ArgumentException("Id parameter not correct. Must look like xx.yyy");
            _mainCategory = int.Parse(segments[0]);
            _subCategory = int.Parse(segments[1]);
            if (_mainCategory < 0)
                throw new ArgumentOutOfRangeException("Id must not be negative");
            return id.Trim();
        }
        #endregion

        #region standard overwrites
        public override bool Equals(object obj)
        {
            return Equals(obj as DPType<TValue>);
        }

        public bool Equals(IDatapointType other)
        {
            var typedother = (DPType<TValue>)other;
            return typedother != null && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
        }
        #endregion

        #region operators
        public static bool operator ==(DPType<TValue> type1, DPType<TValue> type2)
        {
            return EqualityComparer<DPType<TValue>>.Default.Equals(type1, type2);
        }

        public static bool operator !=(DPType<TValue> type1, DPType<TValue> type2)
        {
            return !(type1 == type2);
        }
        #endregion

        public virtual bool IsMainCategory(int category)
        {
            return category == _mainCategory;
        }

        #region encoding DPT
        public abstract byte[] Encode(TValue value);

        public virtual byte[] Encode(object value)
        {
            if (value is null)
                throw new ArgumentNullException("Value must not be null");
            if (value is string)
                return EncodeFromString((string)value);
            if (value is double || value is float)
                return EncodeFromDouble((double)value);
            if (value is int || value is short || value is long)
                return EncodeFromLong((long)value);
            if (value is uint || value is ushort || value is ulong)
                return EncodeFromULong((long)value);
            if (value is bool)
                return EncodeFromBool((bool)value);
            return EncodeFromObject(value);
        }

        protected virtual byte[] EncodeFromLong(long value)
        {
            throw new Exceptions.TranslationException("Translation from long not supported for type " + Id);
        }

        protected virtual byte[] EncodeFromULong(long value)
        {
            throw new Exceptions.TranslationException("Translation from ulong not supported for type " + Id);
        }

        protected virtual byte[] EncodeFromBool(bool value)
        {
            throw new Exceptions.TranslationException("Translation from bool not supported for type " + Id);
        }

        protected virtual byte[] EncodeFromDouble(double value)
        {
            throw new Exceptions.TranslationException("Translation from double not supported for type " + Id);
        }

        protected virtual byte[] EncodeFromObject(object value)
        {
            throw new Exceptions.TranslationException($"Translation from '{value.GetType()}' not supported for type {Id}");
        }
    
        protected virtual byte[] EncodeFromString(string value)
        {
            throw new Exceptions.TranslationException("Translation from string not supported for type " + Id);
        }
        #endregion

        #region decoding DPT
        public virtual string DecodeString(byte[] dptData, int offset = 0, bool withUnit = false)
        {
            return Decode(dptData, offset) + ((withUnit & !String.IsNullOrEmpty(Unit)) ? " " + Unit : "");
        }

        public virtual object DecodeObject(byte[] dptData, int offset = 0)
        {
            return Decode(dptData, offset);
        }

        public abstract TValue Decode(byte[] dptData, int offset = 0);
        #endregion
    }
}
