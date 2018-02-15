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
using Tiveria.Knx.IP.Utils;
using Tiveria.Common.IO;

namespace Tiveria.Knx.IP.ServiceTypes
{
    /// <summary>
    /// ServiceType class used in case the service type is not known / directly supported 
    /// </summary>
    public class UnknownService : ServiceTypeBase, IServiceType
    {
        private byte[] _frameRaw;
        private ushort _serviceTypeRaw;

        public byte[] FrameRaw => _frameRaw;
        public ushort ServiceTypeRaw => _serviceTypeRaw;
        public UnknownService(ushort serviceTypeRaw, byte[] frameRaw) : base(ServiceTypeIdentifier.UNKNOWN)
        {
            if (frameRaw == null)
                throw new ArgumentNullException("frameRaw must not be null");
            _serviceTypeRaw = serviceTypeRaw;
            _frameRaw = frameRaw;
            _structureLength = _frameRaw.Length;
        }

        public override string ToString()
        {
            return $"UnknownService: {_serviceTypeRaw:x4} - Length: {_structureLength}";
        }

        public override void WriteToByteArray(byte[] buffer, int offset = 0)
        {
            base.WriteToByteArray(buffer, offset);
            Array.Copy(_frameRaw, 0, buffer, offset, _frameRaw.Length);
        }

        public static UnknownService Parse(EndianessAwareBinaryReader br, ushort serviceTypeRaw, int length)
        {
            return new UnknownService(serviceTypeRaw, br.ReadBytes(length));
        }
    }
}
