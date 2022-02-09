/*, ushort start
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

using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Common.IO;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.IP.Structures
{
    public class SRP : KnxDataElement
    {
        private static int HEADER_SIZE = 2;

        #region public properties
        public byte[] Data { get; init; }
        public SrpType Type { get; init; }
        public bool Mandatory { get; init; }
        #endregion

        #region constructors
        public SRP(SrpType type, byte[] data, bool mandatory = true)
        {
            Type = type;
            Mandatory = mandatory;

            switch (Type)
            {
                case SrpType.Invalid:
                case SrpType.SelectByProgrammingMode:
                    Size = HEADER_SIZE;
                    Data = Array.Empty<byte>();
                    break;
                case SrpType.SelectByMacAddress:
                    if (data == null || data.Length != 6)
                        throw new ArgumentException("Data size does not match expected value of 6 for a MAC Address filter", nameof(data));
                    Size = HEADER_SIZE + 6;
                    Data = data;
                    break;
                case SrpType.SelectByService:
                    if (data == null || data.Length != 2)
                        throw new ArgumentException("Data size does not match expected value of 2 for a Service type and version filter", nameof(data));
                    Size = HEADER_SIZE + 2;
                    Data = data;
                    break;
                case SrpType.RequestDibs:
                    if (data == null || data.Length < 1)
                        throw new ArgumentException("Data size does not match minimum value of 1 for a DIB filter", nameof(data));
                    Data = NormalizeDataForRequestDibs(data);
                    Size = HEADER_SIZE + Data.Length;
                    break;
                default:
                    throw new ArgumentException("Unknown SrpType value");
            }
        }
        #endregion

        private byte[] NormalizeDataForRequestDibs(byte[] data)
        {
                if (data == null || data.Length == 0)
                    throw new ArgumentException("Data cannot be null/empty for a DIB filter", nameof(data));
                var len = (data.Length % 2 == 0) ? data.Length : data.Length + 1;
                var internalData = new byte[len];
                data.CopyTo(internalData, 0);
                return internalData;
        }

        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write((byte)Size);
            if (Mandatory)
                writer.Write((byte)((byte)Type | 0x80));
            else
                writer.Write((byte)Type);
            writer.Write(Data);
        }

        public static SRP Parse(BigEndianBinaryReader reader)
        {
            var size = reader.ReadByte();
            var type = reader.ReadByte();
            var mandatory = (type & 0x80) == 0x80;
            type = (byte)(type & 0x7f);
            if (!Enum.IsDefined(typeof(SrpType), type))
                throw KnxBufferFieldException.TypeUnknown($"SRP.Type", type);
            var data = reader.ReadBytes(size-2);
            return new SRP((SrpType) type, data, mandatory);
        }

        public static SRP CreateWithMacAddress(byte[] mac, bool mandatory=true)
        {
            return new SRP(SrpType.SelectByMacAddress, mac, mandatory);
        }
    }
}
