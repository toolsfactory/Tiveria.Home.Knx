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

namespace Tiveria.Home.Knx.IP.Enums
{
    /// <summary>
    /// List of Search Request Parameter types
    /// </summary>
    public enum SrpType : byte
    {
        /// <summary>
        /// Used for testing KnxNetIP routers. Should not be actively used in production!
        /// </summary>
        Invalid = 0x00,

        /// <summary>
        /// Only KnxNetIP routers should answer to a broadcast search request which are 
        /// currently in programming mode.
        /// </summary>
        SelectByProgrammingMode = 0x01,

        /// <summary>
        /// Only a KnxNetIP router with the specified MAC address should respond to the 
        /// search request.
        /// </summary>
        SelectByMacAddress = 0x02,

        /// <summary>
        /// Only KnxNetIP routers supporting the given service family & version combination 
        /// should answer to the search request.
        /// </summary>
        SelectByService = 0x03,

        /// <summary>
        /// Only KnxNetIP routers with the indicated description types should answer.
        /// </summary>
        RequestDibs = 0x04
    }
}
