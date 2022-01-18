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
using Tiveria.Home.Knx.IP.Enums;

namespace Tiveria.Home.Knx.IP.Frames.Serializers
{
    public abstract class FrameSerializerBase<T> : IKnxNetIPFrameSerializer<T> where T : class, IKnxNetIPFrame
    {

        public Type FrameType => typeof(T);
        public abstract ServiceTypeIdentifier ServiceTypeIdentifier { get; }

        public abstract T Deserialize(BigEndianBinaryReader reader);
        public abstract void Serialize(T frame, BigEndianBinaryWriter writer);

        public virtual byte[] Serialize(T frame)
        {
            var buffer = new byte[frame.FrameHeader.TotalLength];
            Serialize(frame, new BigEndianBinaryWriter(new MemoryStream(buffer)));
            return buffer;
        }

        public virtual T Deserialize(byte[] buffer)
        {
            return Deserialize(new BigEndianBinaryReader(new MemoryStream(buffer)));
        }

        public virtual bool TryDeserialize(byte[] buffer, out T? frame)
        {
            try
            {
                frame = Deserialize(buffer);
                return true;
            }
            catch
            {
                frame = null;
                return false;
            }
        }

        public virtual bool TryDeserialize(BigEndianBinaryReader reader, out T? frame)
        {
            try
            {
                frame = Deserialize(reader);
                return true;
            }
            catch
            {
                frame = null;
                return false;
            }
        }

        IKnxNetIPFrame IKnxNetIPFrameSerializer.Deserialize(byte[] buffer)
        {
            return Deserialize(buffer);
        }

        IKnxNetIPFrame IKnxNetIPFrameSerializer.Deserialize(BigEndianBinaryReader reader)
        {
            return Deserialize(reader);
        }

        byte[] IKnxNetIPFrameSerializer.Serialize(IKnxNetIPFrame frame)
        {
            if (frame.GetType() != typeof(T))
                throw new InvalidCastException();
            return Serialize((T)frame);
        }

        void IKnxNetIPFrameSerializer.Serialize(IKnxNetIPFrame frame, BigEndianBinaryWriter writer)
        {
            Serialize((T)frame, writer);
        }
    }
}
