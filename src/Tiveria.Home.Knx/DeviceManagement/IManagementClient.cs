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

using Tiveria.Home.Knx.BaseTypes;
using Tiveria.Home.Knx.Cemi;

namespace Tiveria.Home.Knx.DeviceManagement
{
    public interface IManagementClient
    {
        bool IsConnected { get; }
        ManagementConnectionState State { get; }
        public IndividualAddress RemoteAddress { get; }
        Task ConnectAsync();
        Task DisconnectAsync();

        Task<byte[]> ReadDeviceDescriptorAsync(byte descriptorType = 0);
        Task RestartDeviceAsync();
        Task<(byte ErrorCode, int ProcessTime)> ResetDeviceAsync(EraseCode code, int channelid, bool areYouSure);

        Task<PropertyDescription> ReadPropertyDescriptionAsync(byte objIdx, byte propId);
        Task<PropertyDescription> ReadPropertyDescriptionByIndexAsync(byte objIdx, byte index);
        Task<byte[]> ReadPropertyAsync(byte objIdx, byte propId);
        Task<byte[]> ReadPropertiesAsync(byte objIdx, byte propId, int start, int count);
        Task WritePropertyAsync(byte objIdx, byte propId, int startIndex, int elements, byte[] data);

        Task<byte[]> ReadMemoryAsync(int offset, int count);
        Task WriteMemoryAsync(int offset, byte[] data);

        Task<int> ReadADCAsync(byte channel, byte repeats);

        Task<IndividualAddress> ReadIndividualAddressAsync();
        Task<IndividualAddress> ReadAddressAsync(SerialNumber serialNumber);
        Task<IndividualAddress[]> ReadAllAddressesAsync();
        Task WriteAddressAsync(SerialNumber serialNumber, IndividualAddress address);


        Task<byte[]> ReadDomainAddressAsync();
        Task<IList<byte[]>> ReadDomainAddressesAsync(byte[] domainaddress, IndividualAddress startAddress, int range);
        Task<IList<byte[]>> ReadAllDomainAddressAsync();
        Task WriteDomainAddressAsync(byte[] domainAddress);
        Task WriteDomainAddressAsync(SerialNumber serialNumber, byte[] domainAddress);
    }

    public enum ManagementConnectionState
    {
        Closed,
        OpenIdle,
        OpenWait,
        Connecting
    }
}
