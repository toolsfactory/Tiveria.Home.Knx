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
    /// Provides standard capabilities to serialize and deserialize Cemi messages
    /// </summary>
    public interface IKnxCemiSerializer
    {
        /// <summary>
        /// Deserializes a byte array into a <see cref="ICemiMessage"/>
        /// </summary>
        /// <param name="buffer">The binary representation of the Cemi message as byte array</param>
        /// <returns>the cemi messages as object of type <see cref="ICemiMessage"/></returns>
        ICemiMessage Deserialize(byte[] buffer);

        /// <summary>
        /// Deserializes a <see cref="ICemiMessage"/> by reading the data using the provided <see cref="BigEndianBinaryReader"/>
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="size">maximum amount of data to read</param>
        /// <returns></returns>
        ICemiMessage Deserialize(BigEndianBinaryReader reader, int size = -1);

        /// <summary>
        /// Serialize an <see cref="ICemiMessage"/> into a byte array
        /// </summary>
        /// <param name="cemiMessage">The message to serialize</param>
        /// <returns>The resulting byte array</returns>
        byte[] Serialize(ICemiMessage cemiMessage);

        /// <summary>
        /// Serialize a <see cref="ICemiMessage"/> using a <see cref="BigEndianBinaryWriter"/>
        /// </summary>
        /// <param name="cemiMessage">The message to serialize</param>
        /// <param name="writer">The writer used for serialization</param>
        void Serialize(ICemiMessage cemiMessage, BigEndianBinaryWriter writer);
    }
}
