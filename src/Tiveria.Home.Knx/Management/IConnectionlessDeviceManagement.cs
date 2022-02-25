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
    public interface IConnectionlessDeviceManagement
    {
        /// <summary>
        /// Sends a restart request to the device
        /// </summary>
        /// <param name="address">The <see cref="IndividualAddress"/> of the device the command should be sent to</param>
        /// <returns>The awaitable <see cref="Task"/></returns>
        Task RestartDeviceAsync(IndividualAddress address);

        /// <summary>
        /// Sends a reset request to a device. Use with care!
        /// </summary>
        /// <param name="address">The <see cref="IndividualAddress"/> of the device the command should be sent to</param>
        /// <param name="code">The erase code to be used</param>
        /// <param name="channelid">The channel to be reset</param>
        /// <param name="areYouSure">A confirmation flag</param>
        /// <returns></returns>
        Task<(byte ErrorCode, int ProcessTime)> ResetDeviceAsync(IndividualAddress address, EraseCode code, int channelid, bool areYouSure);

        /// <summary>
        /// Read the description for a specific property from the device
        /// </summary>
        /// <param name="address">The <see cref="IndividualAddress"/> of the device the command should be sent to</param>
        /// <param name="objIdx">Index of the object in the device</param>
        /// <param name="propId">Id of the property withing the object of the device</param>
        /// <returns>Description of the property</returns>
        Task<PropertyDescription> ReadPropertyDescriptionAsync(IndividualAddress address, byte objIdx, byte propId);

        /// <summary>
        /// Reads the value of a property from the device
        /// </summary>
        /// <param name="address">The <see cref="IndividualAddress"/> of the device the command should be sent to</param>
        /// <param name="objIdx">Index of the object in the device</param>
        /// <param name="propId">Id of the property withing the object of the device</param>
        /// <returns>THe property value</returns>
        Task<byte[]> ReadPropertyAsync(IndividualAddress address, byte objIdx, byte propId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">The <see cref="IndividualAddress"/> of the device the command should be sent to</param>
        /// <param name="objIdx">Index of the object in the device</param>
        /// <param name="propId">Id of the property withing the object of the device</param>
        /// <param name="startIndex"></param>
        /// <param name="elements"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task WritePropertyAsync(IndividualAddress address, byte objIdx, byte propId, int startIndex, int elements, byte[] data);
    }
}
