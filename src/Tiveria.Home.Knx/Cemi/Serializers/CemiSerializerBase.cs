using Tiveria.Common.IO;
using Tiveria.Home.Knx.Primitives;
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

namespace Tiveria.Home.Knx.Cemi.Serializers
{
    public abstract class CemiSerializerBase<T> : IKnxCemiSerializer<T> where T : class, ICemiMessage
    {
        #region Serialization & Deserialization abstracts
        public abstract void Serialize(T cemiMessage, BigEndianBinaryWriter writer);
        public abstract T Deserialize(BigEndianBinaryReader reader, int size);
        #endregion

        #region Serialization & Deserialization default implementations
        public T Deserialize(byte[] buffer)
        {
            return Deserialize(new BigEndianBinaryReader(buffer), buffer.Length);
        }

        public byte[] Serialize(T cemiMessage)
        {
            var buffer = new byte[cemiMessage.Size];
            Serialize(cemiMessage, new BigEndianBinaryWriter(new MemoryStream(buffer)));
            return buffer;
        }

        byte[] IKnxCemiSerializer.Serialize(ICemiMessage cemiMessage)
        {
            var buffer = new byte[cemiMessage.Size];
            Serialize((T)cemiMessage, new BigEndianBinaryWriter(new MemoryStream(buffer)));
            return buffer;
        }

        void IKnxCemiSerializer.Serialize(ICemiMessage cemiMessage, BigEndianBinaryWriter writer)
        {
            Serialize((T)cemiMessage, writer);
        }

        public bool TryDeserialize(byte[] buffer, out T? cemiMessage)
        {
            try
            {
                cemiMessage = Deserialize(buffer);
                return true;
            }
            catch
            {
                cemiMessage = null;
                return false;
            }
        }

        public bool TryDeserialize(BigEndianBinaryReader reader, int size, out T? cemiMessage)
        {
            try
            {
                cemiMessage = Deserialize(reader, size);
                return true;
            }
            catch
            {
                cemiMessage = null;
                return false;
            }
        }

        ICemiMessage IKnxCemiSerializer.Deserialize(byte[] buffer)
        {
            return Deserialize(new BigEndianBinaryReader(buffer), buffer.Length);
        }

        ICemiMessage IKnxCemiSerializer.Deserialize(BigEndianBinaryReader reader, int size)
        {
            return Deserialize(reader, size);
        }
        #endregion

        #region Helpers for certain standard field types
        protected virtual List<AdditionalInformationField> ReadAdditionalInfoFields(BigEndianBinaryReader br, byte additionalInfoLength)
        {
            List<AdditionalInformationField> list = new();
            var count = 0;
            while (count < additionalInfoLength)
            {
                var field = AdditionalInformationField.Parse(br);
                list.Add(field);
                count += field.Size;
            }
            return list;
        }

        protected virtual IndividualAddress ReadSourceAddress(BigEndianBinaryReader br)
        {
            var src = br.ReadUInt16();
            return new IndividualAddress(src);
        }


        protected virtual Address ReadDestinationAddress(BigEndianBinaryReader br, bool isGroupAddress)
        {
            var dst = br.ReadUInt16();
            if (isGroupAddress)
                return new GroupAddress(dst);
            else
                return new IndividualAddress(dst);
        }

        #endregion
    }
}