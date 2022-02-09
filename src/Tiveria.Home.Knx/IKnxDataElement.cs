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

namespace Tiveria.Home.Knx
{
    /// <summary>
    /// Baseline interface for all elements of a Knx message
    /// </summary>
    public interface IKnxDataElement
    {
        /// <summary>
        /// Size of the data element in its binary representation of the Knx protocol
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Converts the data element to a byte array
        /// </summary>
        /// <returns>The byte array</returns>
        byte[] ToBytes();

        /// <summary>
        /// Writes the data element to a <see cref="MemoryStream"/> using the provided <see cref="BigEndianBinaryWriter"/>
        /// </summary>
        /// <param name="writer">The writer</param>
        void Write(BigEndianBinaryWriter writer);
    }
}
