/*
    Tiveria.Home.Knx - a .Net Core base KNX library
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

using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.Datapoint
{
    public class DPType16 : DPType<string>
    {
        public static int StringLength = 14;
        public static char Replacement = '?';

        public DPType16(string id, string name, string unit = "", string description = "") : base(id, name, "", "", unit, description)
        {
            DataSize = -1;
        }

        public override byte[] Encode(string value)
        {
            if (value.Length > StringLength)
                throw new KnxTranslationException("maximum KNX string length is 14 characters");
            var data = new byte[StringLength];
            char rangeMax = '\u007f';
            if (this == DPT_STRING_8859_1)
                rangeMax = '\u00ff';
            for (var i = 0; i < StringLength; i++)
            {
                char c = value[i];
                data[i] = (byte)((c <= rangeMax) ? c : Replacement);
            }
            return data;
        }

        public override string Decode(byte[] dptData, int offset = 0)
        {
            if (dptData.Length - offset < 1)
                throw KnxBufferSizeException.TooSmall("DPType16/String");
            var characters = new char[StringLength];
            int i = 0;
            for (i = offset; (i < (offset + StringLength)) & dptData[i] != 0; i++)
            {
                characters[i - offset] = (char)dptData[i];
            }
            return new string(characters, 0, i - offset);
        }

        #region specific xlator instances
        public static DPType16 DPT_STRING_ASCII = new DPType16("16.000", "ASCII string");
        public static DPType16 DPT_STRING_8859_1 = new DPType16("16.001", "ISO-8859-1 string (Latin 1)");

        static DPType16()
        {
            DatapointTypesList.AddOrReplace(DPT_STRING_ASCII);
            DatapointTypesList.AddOrReplace(DPT_STRING_8859_1);
        }
        #endregion
    }
}
