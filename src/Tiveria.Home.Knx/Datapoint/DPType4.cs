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

namespace Tiveria.Home.Knx.Datapoint
{
    public class DPType4 : DPType<char>
    {
        protected DPType4(string id, string name, char min, char max, string unit = "", string description = "") : base(id, name, min, max, unit, description)
        {
            DataSize = 1;
        }

        #region decoding DPT
        public override char Decode(byte[] dptData, int offset = 0)
        {
            if (dptData == null || dptData.Length - offset < 1)
                throw new ArgumentException();
            if (this == DPT_ASCII)
                return System.Text.Encoding.ASCII.GetChars(dptData, offset, 1)[0];
            else
                return System.Text.Encoding.GetEncoding("iso-8859-1").GetChars(dptData, offset, 1)[0];
        }
        #endregion

        #region Encoding DPT
        public override byte[] Encode(char value)
        {
            byte[] bytes = (this == DPT_ASCII) ?
                System.Text.Encoding.ASCII.GetBytes(new char[] { value }) :  System.Text.Encoding.GetEncoding("iso-8859-1").GetBytes(new char[] { value });

            if (bytes == null || bytes.Length != 1)
                throw new ArgumentException();

            return bytes;
        }

        protected override byte[] EncodeFromLong(long value)
        {
            return Encode((char)value);
        }

        protected override byte[] EncodeFromULong(long value)
        {
            return Encode((char)value);
        }

        protected override byte[] EncodeFromDouble(double value)
        {
            return Encode((char)value);
        }

        protected override byte[] EncodeFromString(string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException("value is null or empty");
            return Encode(value[0]);
        }


        #endregion

        #region specific xlator instances
        public static readonly DPType4 DPT_ASCII = new DPType4("4.001", "ASCII Character", (char) 0, (char) 127);
        public static readonly DPType4 DPT_ISO8859_1 = new DPType4("4.002", "ISO 8859-1 Character", (char)0, (char)255);

        static DPType4()
        {
            DatapointTypesList.AddOrReplace(DPT_ASCII);
            DatapointTypesList.AddOrReplace(DPT_ISO8859_1);
        }
        #endregion

    }
}
