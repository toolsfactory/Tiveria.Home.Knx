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

using Tiveria.Home.Knx.Primitives;


namespace Tiveria.Home.Knx.Management
{
    /// <summary>
    /// 
    /// </summary>
    public interface INetworkManagement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IndividualAddress> ReadIndividualAddressAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        Task<IndividualAddress> ReadIndividualAddressAsync(SerialNumber serialNumber);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task WriteIndividualAddressAsync(IndividualAddress address);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        Task WriteIndividualAddressAsync(SerialNumber serialNumber, IndividualAddress address);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="pid"></param>
        /// <param name="testinfo"></param>
        /// <returns></returns>
        Task<byte[]> ReadNetworkParameterAsync(int objType, byte pid, byte[] testinfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="pid"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task WriteNetworkParameterAsync(int objType, byte pid, byte[] value);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<byte[]> ReadDomainAddressAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        Task ReadDomainAddressAsync(SerialNumber serialNumber);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainAddress"></param>
        /// <returns></returns>
        Task WriteDomainAddressAsync(byte[] domainAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="domainAddress"></param>
        /// <returns></returns>
        Task WriteDomainAddressAsync(SerialNumber serialNumber, byte[] domainAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="pid"></param>
        /// <param name="testinfo"></param>
        /// <returns></returns>
        Task<byte[]> ReadSystemNetworkParameterAsync(int objType, int pid, byte[] testinfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="pid"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task WriteSystemNetworkParameterAsync(int objType, int pid, byte[] value);

    }
}
