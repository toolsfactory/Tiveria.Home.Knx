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

namespace Tiveria.Home.Knx.Management
{
    /// <summary>
    /// Erase codes used when sending a master reset restart apci code. <see cref="ApduType.RestartMasterReset_Request"/>
    /// </summary>
    public enum EraseCode : byte
    {
        ConfirmedRestart      = 0x01,
        FactoryReset          = 0x02,
        ResetIA               = 0x03,
        ResetAP               = 0x04,
        ResetParam            = 0x05,
        ResetLinks            = 0x06,
        FactoryResetwithoutIA = 0x07
    }

    public static class EraseCodeExtensions
    {
        public static string ToDescription(this EraseCode code) => code switch
        {
            EraseCode.ConfirmedRestart      => "Confirmed restart. No reset.",
            EraseCode.FactoryReset          => "Reset the device to the initialy factory state.",
            EraseCode.ResetIA               => "Reset the device individual adress to the default value.",
            EraseCode.ResetAP               => "Reset the application program memory of the device.",
            EraseCode.ResetParam            => "Reset application parameter memory to its defaults.",
            EraseCode.ResetLinks            => "Reset link information for group objects.",
            EraseCode.FactoryResetwithoutIA => "Reset the device to the initialy factory state keeping the current individual address.",
            _                               => throw new NotImplementedException()
        };
    }

}
