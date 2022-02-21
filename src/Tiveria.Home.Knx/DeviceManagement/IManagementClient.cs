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
using Tiveria.Home.Knx.Cemi;
using Tiveria.Common;
using Tiveria.Home.Knx.Exceptions;


namespace Tiveria.Home.Knx.DeviceManagement
{
    /// <summary>
    /// Interface describing all capabilities a management client class should expose
    /// </summary>
    public interface IManagementClient
    {
        /// <summary>
        /// Connection status
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Detailed connection state
        /// </summary>
        ManagementConnectionState State { get; }

        /// <summary>
        /// Address of remote device the management client is connected to
        /// </summary>
        public IndividualAddress RemoteAddress { get; }

        /// <summary>
        /// Establishes the connection to a specific device
        /// </summary>
        /// <returns>The awaitable <see cref="Task"/></returns>
        Task ConnectAsync();

        /// <summary>
        /// Disconnects from the specific device
        /// </summary>
        /// <returns>The awaitable <see cref="Task"/></returns>
        Task DisconnectAsync();

        /// <summary>
        /// Reads Knx Device Descriptor bytes from the device
        /// </summary>
        /// <param name="descriptorType">Which descriptor to read</param>
        /// <returns>The data returned</returns>
        Task<byte[]> ReadDeviceDescriptorAsync(byte descriptorType = 0);

        /// <summary>
        /// Sends a restart request to the device
        /// </summary>
        /// <returns>The awaitable <see cref="Task"/></returns>
        Task RestartDeviceAsync();

        /// <summary>
        /// Sends a reset request to a device. Use with care!
        /// </summary>
        /// <param name="code">The erase code to be used</param>
        /// <param name="channelid">The channel to be reset</param>
        /// <param name="areYouSure">A confirmation flag</param>
        /// <returns></returns>
        Task<(byte ErrorCode, int ProcessTime)> ResetDeviceAsync(EraseCode code, int channelid, bool areYouSure);

        /// <summary>
        /// Read the description for a specific property from the device
        /// </summary>
        /// <param name="objIdx">Index of the object in the device</param>
        /// <param name="propId">Id of the property withing the object of the device</param>
        /// <returns>Description of the property</returns>
        Task<PropertyDescription> ReadPropertyDescriptionAsync(byte objIdx, byte propId);

        /// <summary>
        /// Read the description for a specific property from the device
        /// </summary>
        /// <param name="objIdx">Index of the object in the device</param>
        /// <param name="index">Index of the property withing the object of the device</param>
        /// <returns>Description of the property</returns>
        Task<PropertyDescription> ReadPropertyDescriptionByIndexAsync(byte objIdx, byte index);

        /// <summary>
        /// Reads the value of a property from the device
        /// </summary>
        /// <param name="objIdx">Index of the object in the device</param>
        /// <param name="propId">Id of the property withing the object of the device</param>
        /// <returns>THe property value</returns>
        Task<byte[]> ReadPropertyAsync(byte objIdx, byte propId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objIdx"></param>
        /// <param name="propId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<byte[]> ReadPropertiesAsync(byte objIdx, byte propId, int start, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objIdx"></param>
        /// <param name="propId"></param>
        /// <param name="startIndex"></param>
        /// <param name="elements"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task WritePropertyAsync(byte objIdx, byte propId, int startIndex, int elements, byte[] data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<byte[]> ReadMemoryAsync(int offset, int count);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task WriteMemoryAsync(int offset, byte[] data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="repeats"></param>
        /// <returns></returns>
        Task<int> ReadADCAsync(byte channel, byte repeats);

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
        Task<IndividualAddress> ReadAddressAsync(SerialNumber serialNumber);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IndividualAddress[]> ReadAllAddressesAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        Task WriteAddressAsync(SerialNumber serialNumber, IndividualAddress address);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<byte[]> ReadDomainAddressAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainaddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        Task<IList<byte[]>> ReadDomainAddressesAsync(byte[] domainaddress, IndividualAddress startAddress, int range);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IList<byte[]>> ReadAllDomainAddressAsync();

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
    }
}
