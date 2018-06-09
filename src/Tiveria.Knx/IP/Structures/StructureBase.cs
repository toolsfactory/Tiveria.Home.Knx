/*
    Tiveria.Knx - a .Net Core base KNX library
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

using System;
using Tiveria.Common.IO;

namespace Tiveria.Knx.IP.Structures
{
    public abstract class StructureBase : IStructure
    {
        protected int _size;
        public int Size { get => _size; }

        public byte[] ToBytes()
        {
            var data = new byte[Size];
            WriteToByteArray(data, 0);
            return data;
        }

        public virtual void Write(IndividualEndianessBinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer is null");
        }

        public virtual void WriteToByteArray(byte[] buffer, int offset = 0)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");
            if (offset + _size > buffer.Length)
                throw new ArgumentOutOfRangeException("buffer too small");
        }
    }
}
