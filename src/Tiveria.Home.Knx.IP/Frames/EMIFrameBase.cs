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
using Tiveria.Home.Knx.EMI;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.IP.Frames
{
    public abstract class EMIFrameBase : FrameBase, IKnxNetIPFrame
    {
        public ConnectionHeader ConnectionHeader { get; init; }
        public ICemi CemiMessage { get; init; }

        protected EMIFrameBase(FrameHeader frameHeader, ServiceTypeIdentifier serviceTypeIdentifier, ConnectionHeader connectionHeader, ICemi cemi)
            : base(frameHeader, serviceTypeIdentifier, connectionHeader.Size + cemi.Size)
        {
            ConnectionHeader = connectionHeader;
            CemiMessage = cemi;
        }

        protected EMIFrameBase(ServiceTypeIdentifier serviceTypeIdentifier, ConnectionHeader connectionHeader, ICemi cemi)
            : base(serviceTypeIdentifier, connectionHeader.Size + cemi.Size)
        {
            ConnectionHeader = connectionHeader;
            CemiMessage = cemi;
        }
    }
}