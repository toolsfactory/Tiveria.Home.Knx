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
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    Linking this library statically or dynamically with other modules is
    making a combined work based on this library. Thus, the terms and
    conditions of the GNU General Public License cover the whole
    combination.
*/

namespace Tiveria.Home.Knx.IP
{
    /// <summary>
    /// Structure representing the KNX Version information
    /// </summary>
    public class KnxNetIPVersion
    {
        #region static defaults
        /// <summary>
        /// Constant representing Version 1.0 of KnxNetIP
        /// </summary>
        public static readonly KnxNetIPVersion Version10 = new KnxNetIPVersion("KnxNetIP Version 1.0", 0x10, 0x06);
        /// <summary>
        /// Constant representing Version 2.0 of KnxNetIP
        /// </summary>
        public static readonly KnxNetIPVersion Version20 = new KnxNetIPVersion("KnxNetIP Version 2.0", 0x20, 0x06);
        /// <summary>
        /// List of versions supported by this library 
        /// </summary>
        public static readonly KnxNetIPVersion[] SupportedVersions = new KnxNetIPVersion[2] { Version10, Version20 };
        /// <summary>
        /// Default Version used in Frames when not specified otherwise
        /// </summary>
        public static KnxNetIPVersion DefaultVersion = Version10;
        #endregion

        #region public properties
        /// <summary>
        /// The descriptive name of that version
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// KNX Identifier of that version
        /// </summary>
        public byte Identifier { get; init; }
        /// <summary>
        /// Headerlength of that version
        /// </summary>
        public byte HeaderLength { get; init; }
        /// <summary>
        /// Main version number based on the Knx standard scheme
        /// </summary>
        public int VersionMajor => Identifier / 0x10;
        /// <summary>
        /// Sub version number based on the Knx standard scheme
        /// </summary>
        public int VersionMinor => Identifier % 0x10;
        /// <summary>
        /// String representation of this version based on the Knx standard scheme
        /// </summary>
        public string VersionString => $"{VersionMajor}.{VersionMinor}";
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new KNX Version object
        /// </summary>
        /// <param name="name">Readable name of the version</param>
        /// <param name="identifier">Identifier representing thjis version in the KnxNetIP header</param>
        /// <param name="headerlength">Size of the KnxNetIP header in this version</param>
        public KnxNetIPVersion(string name, byte identifier, byte headerlength)
        {
            Name = name;
            Identifier = identifier;
            HeaderLength = headerlength;
        }
        #endregion

        #region static helpers
        /// <summary>
        /// Checks whether a version object provided matches one of the predefined supported versions
        /// </summary>
        /// <param name="ver"></param>
        /// <returns></returns>
        public static bool IsSupportedVersion(KnxNetIPVersion ver)
        {
            return FindSupportedVersion(ver.Identifier, ver.HeaderLength, out _);
        }

        /// <summary>
        /// Try to find a supported version that matches the provided criteria
        /// </summary>
        /// <param name="identifier">Identifier in the header</param>
        /// <param name="headersize">Size of the header</param>
        /// <param name="version">The version object that matches</param>
        /// <returns>true in case a matching version was found. false in all other cases </returns>
        public static bool FindSupportedVersion(byte identifier, byte headersize, out KnxNetIPVersion? version)
        {
            version = SupportedVersions
                .Where(x => (x.Identifier == identifier) && (x.HeaderLength == headersize))
                .FirstOrDefault();
            return version != null; 
        }

        /// <summary>
        /// Try to find a supported version that matches the provided criteria
        /// </summary>
        /// <param name="identifier">Identifier in the header</param>
        /// <param name="version">The version object that matches</param>
        /// <returns>true in case a matching version was found. false in all other cases </returns>
        public static bool FindSupportedVersion(byte identifier, out KnxNetIPVersion? version)
        {
            version = SupportedVersions
                .Where(x => (x.Identifier == identifier))
                .FirstOrDefault();
            return version != null;
        }
        #endregion
    }
}
