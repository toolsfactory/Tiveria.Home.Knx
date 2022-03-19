/*
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

using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.Datapoint
{
    public class DPType28 : DPType<string>
    {
        public static readonly int MaxLength = 1024 * 1024;

        protected DPType28(string id, string name, string unit = "", string description = "") : base(id, name, "", "", unit, description)
        {
            DataSize = -1;
        }

        public override byte[] Encode(string value)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(value);
            if (bytes.Length > MaxLength)
                throw new KnxTranslationException("String to long");
            var result = new byte[bytes.Length + 1];
            bytes.CopyTo(result, 0);
            result[^1] = 0;
            return result;
        }

        public override string Decode(byte[] dptData, int offset = 0)
        {
            if (dptData.Length - offset < 1)
                throw KnxBufferSizeException.TooSmall("DPType28/String");
            var len = 0;
            for (var i = offset; (i < dptData.Length) && (dptData[i] != 0); i++)
                len++;
            var str = new byte[len];
            for (var i = 0; i < len; i++)
                str[i] = dptData[offset + i];
            return System.Text.Encoding.UTF8.GetString(str);
        }

        #region specific xlator instances
        public static readonly DPType28 DTP_UTF8 = new("28.001", "UTF8 string");

        internal static void Init()
        {
            DatapointTypesList.AddOrReplace(DTP_UTF8);
        }
        #endregion
    }

    public class DPType29 : DPType<long>
    {
        protected DPType29(string id, string name, long min, long max, string unit = "", string description = "") : base(id, name, min, max, unit, description)
        {
        }

        public override byte[] Encode(long value)
        {
            throw new NotImplementedException();
        }

        public override long Decode(byte[] dptData, int offset = 0)
        {
            return base.Decode(dptData, offset);
        }

    }
}