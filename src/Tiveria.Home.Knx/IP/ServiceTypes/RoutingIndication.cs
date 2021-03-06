﻿/*
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

using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Common.IO;

namespace Tiveria.Home.Knx.IP.ServiceTypes
{
    /// <summary>
    /// Implementation of the ServiceType RoutingIndication
    /// </summary>
    public class RoutingIndication : ServiceTypeBase
    {
        private readonly ConnectionHeader _connectionHeader;
        private readonly Cemi.CemiLData _cemiFrame;

        public ConnectionHeader ConnectionHeader => _connectionHeader;
        public Cemi.CemiLData CemiFrame => _cemiFrame;

        protected RoutingIndication()
            : base(ServiceTypeIdentifier.ROUTING_IND)
        { }

        protected RoutingIndication(IndividualEndianessBinaryReader br)
            : this()
        {
            _connectionHeader = ConnectionHeader.Parse(br);
            _cemiFrame = Cemi.CemiLData.Parse(br);
        }

        public RoutingIndication(ConnectionHeader header, Cemi.CemiLData cemiFrame)
            : this()
        {
            _connectionHeader = header;
            _cemiFrame = cemiFrame;
        }

        public override void WriteToByteArray(byte[] buffer, int offset = 0)
        {
            base.WriteToByteArray(buffer, offset);
            _connectionHeader.WriteToByteArray(buffer, offset);
            _cemiFrame.WriteToByteArray(buffer, offset + _connectionHeader.Size);
        }


        #region static methods
        /// <summary>
        /// Parses a part of a buffer and creates a CemiLData class from it
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static RoutingIndication Parse(byte[] buffer, int offset, int length)
        {
            return new RoutingIndication(new IndividualEndianessBinaryReader(buffer, offset, length));
        }

        public static bool TryParse(out RoutingIndication tunnelRequest, byte[] buffer, int offset, int length)
        {
            bool result = false;
            try
            {
                tunnelRequest = new RoutingIndication(new IndividualEndianessBinaryReader(buffer, offset, length));
                result = true;
            }
            catch
            {
                tunnelRequest = null;
                result = false;
            }
            return result;
        }
        #endregion
    }

}
