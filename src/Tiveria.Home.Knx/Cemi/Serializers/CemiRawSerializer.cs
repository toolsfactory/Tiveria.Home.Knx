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

using Tiveria.Common.IO;

namespace Tiveria.Home.Knx.Cemi.Serializers
{
    public class CemiRawSerializer : CemiSerializerBase<CemiRaw>
    {
        public override CemiRaw Deserialize(BigEndianBinaryReader reader, int size = -1)
        {
            if (size > reader.Available)
                throw new ArgumentOutOfRangeException(nameof(size), size, $"Size bigger than Data available {reader.Available}");

            var messageCode = reader.ReadByteEnum<MessageCode>("Cemi.MessageCode");
            var payload = (size < 0) ? reader.ReadBytesFull() : reader.ReadBytes(size - 1);
            return new CemiRaw(messageCode, payload);
        }

        public override void Serialize(CemiRaw cemiMessage, BigEndianBinaryWriter writer)
        {
            writer.Write((byte)cemiMessage.MessageCode);
            if (cemiMessage.Payload.Length > 0)
                writer.Write(cemiMessage.Payload);
        }
    }
}
