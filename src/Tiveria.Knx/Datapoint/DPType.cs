/*
    Tiveria.Knx - a .Net Core base KNX library
    Copyright (c) 2018 M. Geissler

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    Linking this library statically or dynamically with other modules is
    making a combined work based on this library. Thus, the terms and
    conditions of the GNU General Public License cover the whole
    combination.
*/
    
using System;
using System.Collections.Generic;
using Tiveria.Knx.Exceptions;

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

        public virtual TValue Decode(byte[] dptData, int offset = 0)
        {
            if (DataSize != -1 && (dptData.Length - offset < DataSize))
                throw BufferSizeException.TooSmall("DPType");
            return default;
        }
        #endregion
    }
}
