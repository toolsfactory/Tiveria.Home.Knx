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
using System.Collections.ObjectModel;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.IP.Structures
{
    public class ServiceFamiliesDIB : KnxDataElement
    {
        public static readonly byte DIB_TYPE = 0x02;
        public ReadOnlyCollection<(byte Family, byte Version)> ServiceFamilies { get; init; }

        public ServiceFamiliesDIB(Collection<(byte Family, byte Version)> families)
        {
            ServiceFamilies = new ReadOnlyCollection<(byte Family, byte Version)>(families);
            Size = (ushort)(2 + (ServiceFamilies.Count * 2));
        }

        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write((byte)Size);
            writer.Write(DIB_TYPE);
            foreach (var family in ServiceFamilies)
            {
                writer.Write(family.Family);
                writer.Write(family.Version);
            }
        }

        public static ServiceFamiliesDIB Parse(BigEndianBinaryReader reader)
        {
            var size = reader.ReadByte();
            if ((size < 2) || (size % 2 == 1))
                throw BufferSizeException.WrongSize("ServiceFamiliesDIB.Size", size);

            var type = reader.ReadByte();
            if (type != ServiceFamiliesDIB.DIB_TYPE)
                throw BufferFieldException.WrongValue("DIBTYPE", ServiceFamiliesDIB.DIB_TYPE, type);

            var families = ParseFamilies(reader, (size - 2));
            return new ServiceFamiliesDIB(families);

            static  Collection<(byte Family, byte Version)> ParseFamilies(BigEndianBinaryReader reader, int size)
            {
                var itemscount = size / 2;
                var items = new Collection<(byte Family, byte Version)>();
                for (var i = 0; i < itemscount; i++)
                {
                    items.Add((reader.ReadByte(), reader.ReadByte()));
                }
                return items;
            }
        }
    }

}
