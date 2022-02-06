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

namespace Tiveria.Home.Knx.Primitives
{
    public abstract class Address : KnxDataElement, IKnxAddress, IEquatable<Address>
    {
        #region Protected Constructors
        protected Address(AddressType addressType, ushort address)
        {
            AddressType = addressType;
            RawAddress = address;
            Size = 2;
        }
        #endregion Protected Constructors

        #region Public Properties
        /// <summary>
        /// Type of the address <see cref="AddressType"/>
        /// </summary>
        public AddressType AddressType { get; init; }

        /// <summary>
        /// Address as raw ushort value
        /// </summary>
        public ushort RawAddress { get; init; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>
        /// Clones the address type and value
        /// </summary>
        /// <returns>New address object of the respective type</returns>
        public abstract object Clone();

        /// <summary>
        /// Compares two addresses on type and value level
        /// </summary>
        /// <param name="other">address to compare with</param>
        /// <returns>True if the other address and has the same type and value</returns>
        public bool Equals(Address? other)
        {
            if (other == null)
                return false;
            return (other.AddressType == AddressType) && (RawAddress == other.RawAddress);
        }

        /// <summary>
        /// Checks first if the object to compare with is a valid <see cref="Address"/> type. Then compares on type and value level.
        /// </summary>
        /// <param name="obj">object to compare with</param>
        /// <returns>True if the provided object is an address and has the same type and value</returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as Address);
        }

        /// <summary>
        /// Generates a hashcode that incooperates also the <see cref="AddressType"/>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return RawAddress.GetHashCode() * (AddressType == AddressType.IndividualAddress ? 3 : 94);
        }

        /// <summary>
        /// Checks if an address is a <see cref="GroupAddress"/> or a <see cref="IndividualAddress"/>
        /// </summary>
        /// <returns>True in case of a <see cref="GroupAddress"/></returns>
        public bool IsGroupAddress()
        {
            return AddressType == AddressType.GroupAddress;
        }

        /// <summary>
        /// Checks if an address is a <see cref="GroupAddress"/> or a <see cref="IndividualAddress"/>
        /// </summary>
        /// <returns>True in case of a <see cref="IndividualAddress"/></returns>
        public bool IsIndividualAddress()
        {
            return AddressType == AddressType.IndividualAddress;
        }

        /// <summary>
        /// Writes the Address in raw format using the provided Writer
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.Write(RawAddress);
        }
        #endregion Public Methods
    }
}
