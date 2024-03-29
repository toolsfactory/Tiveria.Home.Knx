﻿/*
    Tiveria.Home.Knx - a .Net Core base KNX library
    Copyright (c) 2018-2022 M. Geissler

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    Linking this library statically or dynamically with other modules is
    making a combined work based on this library. Thus, the terms and
    conditions of the GNU Lesser General Public License cover the whole
    combination.
*/

using System.Globalization;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.Datapoint
{
    public abstract class DPType<TValue> : IDatapointType
    {

        #region public properties
        public string Id { get; }
        public int MainCategory { get; private set; }
        public int SubCategory { get; private set; }
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
            MainCategory = int.Parse(segments[0]);
            SubCategory = int.Parse(segments[1]);
            if (MainCategory < 0)
                throw new ArgumentOutOfRangeException("Id must not be negative");
            return id.Trim();
        }
        #endregion

        #region standard overwrites

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as DPType<TValue>);
        }

        public bool Equals(IDatapointType? other)
        {
            var typedother = (DPType<TValue>?)other;
            return (typedother is not null) && (Id == typedother.Id);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
        }
        #endregion

        #region operators
        /// <inheritdoc/>
        public static bool operator ==(DPType<TValue> type1, DPType<TValue> type2)
        {
            return EqualityComparer<DPType<TValue>>.Default.Equals(type1, type2);
        }

        /// <inheritdoc/>
        public static bool operator !=(DPType<TValue> type1, DPType<TValue> type2)
        {
            return !(type1 == type2);
        }
        #endregion

        /// <summary>
        /// Checks whether the value is equal to the main category of the DPType
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public virtual bool IsMainCategory(int category)
        {
            return category == MainCategory;
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
            throw new Exceptions.KnxTranslationException("Translation from long not supported for type " + Id);
        }

        protected virtual byte[] EncodeFromULong(long value)
        {
            throw new Exceptions.KnxTranslationException("Translation from ulong not supported for type " + Id);
        }

        protected virtual byte[] EncodeFromBool(bool value)
        {
            throw new Exceptions.KnxTranslationException("Translation from bool not supported for type " + Id);
        }

        protected virtual byte[] EncodeFromDouble(double value)
        {
            throw new Exceptions.KnxTranslationException("Translation from double not supported for type " + Id);
        }

        protected virtual byte[] EncodeFromObject(object value)
        {
            throw new Exceptions.KnxTranslationException($"Translation from '{value.GetType()}' not supported for type {Id}");
        }
    
        protected virtual byte[] EncodeFromString(string value)
        {
            throw new Exceptions.KnxTranslationException("Translation from string not supported for type " + Id);
        }
        #endregion

        #region decoding DPT
        /// <summary>
        /// Docodes the binary dpt data representation into a string
        /// </summary>
        /// <param name="dptData">The binary data</param>
        /// <param name="offset">position within the byte array where the dpt data starts</param>
        /// <param name="withUnit">specifies whether the unit is included in the string or not</param>
        /// <param name="invariant">specifies if the invariant string conversion should be used</param>
        /// <returns></returns>
        public virtual string DecodeString(byte[] dptData, int offset = 0, bool withUnit = false, bool invariant = false)
        {
            var ext = (withUnit & !String.IsNullOrEmpty(Unit)) ? " " + Unit : "";
            var obj = Decode(dptData, offset);

            return (!invariant) ? obj + ext : String.Format(CultureInfo.InvariantCulture, "{0}{1}", obj, ext);
        }

        /// <summary>
        /// Decodes the dpt data into an object of the coresponding type
        /// </summary>
        /// <param name="dptData">The binary data</param>
        /// <param name="offset">position within the byte array where the dpt data starts</param>
        /// <returns></returns>
        public virtual object? DecodeObject(byte[] dptData, int offset = 0)
        {
            return Decode(dptData, offset);
        }

        /// <summary>
        /// Decodes the dpt data into the type specified
        /// </summary>
        /// <param name="dptData">The binary data</param>
        /// <param name="offset">position within the byte array where the dpt data starts</param>
        /// <returns></returns>
        public virtual TValue? Decode(byte[] dptData, int offset = 0)
        {
            if (DataSize != -1 && (dptData.Length - offset < DataSize))
                throw KnxBufferSizeException.TooSmall("DPType");
            return default;
        }
        #endregion
    }
}
