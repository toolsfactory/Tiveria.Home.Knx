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

namespace Tiveria.Home.Knx.IP.Enums
{
    /// <summary>
    /// Enumeration with all available connection types mapped to the correct byte encoding. 
    /// <seealso cref="EnumExtensions"/>
    /// </summary>
    public enum ConnectionType : byte
    {
        DEVICE_MGMT_CONNECTION = 0x03,
        TUNNEL_CONNECTION = 0x04,
        REMLOG_CONNECTION = 0x06,
        REMCONF_CONNECTION = 0x07,
        OBJSVR_CONNECTION = 0x08
    }
}
