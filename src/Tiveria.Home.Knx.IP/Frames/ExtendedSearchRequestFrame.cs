﻿/*
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

using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.IP.Frames
{
    public class ExtendedSearchRequestFrame : FrameBase
    {
        public override ServiceTypeIdentifier ServiceTypeIdentifier => ServiceTypeIdentifier.ExtendedSearchRequest;
        public Hpai DiscoveryEndpoint { get; init; }
        public SRP SearchRequestParameter { get; init; }

        public ExtendedSearchRequestFrame(Hpai discoveryEndpoint, SRP searchRequestParameter)
            : this(new FrameHeader(ServiceTypeIdentifier.ExtendedSearchRequest, discoveryEndpoint.Size + searchRequestParameter.Size), discoveryEndpoint, searchRequestParameter)
        { }

        public ExtendedSearchRequestFrame(FrameHeader frameHeader, Hpai discoveryEndpoint, SRP searchRequestParameter)
            : base(frameHeader, ServiceTypeIdentifier.ExtendedSearchRequest, discoveryEndpoint.Size + searchRequestParameter.Size)
        {
            DiscoveryEndpoint = discoveryEndpoint;
            SearchRequestParameter = searchRequestParameter;
        }
    }
}