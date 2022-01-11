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

using Tiveria.Home.Knx.EMI;

namespace Tiveria.Home.Knx
{
    public interface IKnxExternalMessage
    {
        public IKnxAddress DestinationAddress { get; }
        ApciTypes ApciType { get; }

        int GetLength(EMIVersion version = EMIVersion.cEMI);

        /// <summary>
        /// Gets the Byte array representing the message using standard values for additional configuration fields
        /// </summary>
        /// <param name="version">specifies the EMI format to be used</param>
        /// <returns></returns>
        byte[] GetBytes(EMIVersion version = EMIVersion.cEMI);

        /// <summary>
        /// Gets the cEMI version of the message with customized control fields
        /// </summary>
        /// <param name="cf1">ControlField1 of cEMI</param>
        /// <param name="cf2">ControlField2 of cEMI</param>
        /// <returns></returns>
        byte[] GetBytes(ControlField1 cf1, ControlField2 cf2 = default); 

        int WriteBytes(Span<byte> buffer);
        int WriteBytes(Span<byte> buffer, ControlField1 cf1, ControlField2 cf2 = default);
    }
}
