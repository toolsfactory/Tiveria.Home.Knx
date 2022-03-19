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

using Tiveria.Common.IO;
using Tiveria.Home.Knx.Cemi;

namespace Tiveria.Home.Knx
{
    /// <summary>
    /// Provides standard capabilities to serialize and deserialize typed Cemi messages
    /// </summary>
    public interface IKnxCemiSerializer<T> : IKnxCemiSerializer where T : class, ICemiMessage
    {                                                                                                                                                                                           
        /// <summary>
        /// Tries to deserialize a byte array into a <see cref="ICemiMessage"/>
        /// </summary>
        /// <param name="buffer">The binary representation of the Cemi message as byte array</param>
        /// <param name="cemiMessage">Typed response</param>
        /// <returns><c>True</c> in case the data was sucessfully deserialized</returns>
        bool TryDeserialize(byte[] buffer, out T? cemiMessage);

        /// <summary>
        /// Tries to deserialize data using a <see cref="BigEndianBinaryReader"/>
        /// </summary>
        /// <param name="reader">The <see cref="BigEndianBinaryReader"/> to read thed data from0</param>
        /// <param name="size"></param>
        /// <param name="cemiMessage"></param>
        /// <returns></returns>
        bool TryDeserialize(BigEndianBinaryReader reader, int size, out T? cemiMessage);

        /// <summary>
        /// Deserializes a byte array into a <see cref="ICemiMessage"/>
        /// </summary>
        /// <param name="buffer">The binary representation of the Cemi message as byte array</param>
        /// <returns>The parsed and typed <see cref="ICemiMessage"/> based implementation class</returns>
        new T Deserialize(byte[] buffer);

        /// <summary>
        /// Deserializes a <see cref="ICemiMessage"/> by reading the data using the provided <see cref="BigEndianBinaryReader"/>
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="size">maximum amount of data to read</param>
        /// <returns>the cemi messages as object of a class implementing <see cref="ICemiMessage"/></returns>
        new T Deserialize(BigEndianBinaryReader reader, int size);

        void Serialize(T frame, BigEndianBinaryWriter writer);
        byte[] Serialize(T frame);
    }
}
