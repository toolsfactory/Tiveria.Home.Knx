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

using Tiveria.Common.Extensions;

namespace Tiveria.Home.Knx.Cemi
{
    /// <summary>
    /// Class representing a CEMI Frame for a service that is not fully implemented here.
    /// For details please read: "03_06_03_EMI_IMI V01.03.03 AS.PDF" chapter 4 from KNX Association
    /// <code>
    /// +--------+-----------------+
    /// | byte 1 | byte 2 - n      |
    /// +--------+-----------------+
    /// |  Msg   |  Raw Payload    |
    /// | Code   |                 |
    /// +--------+-----------------+
    /// </code>
    ///
    /// </summary>
    /// <remarks> 
    /// This class does NOT support additional info!
    /// </remarks>    
    public class CemiRaw : ICemiMessage
    {
        public int Size { get; init; }
        public byte[] Payload { get; init; }
        public MessageCode MessageCode { get; init; }

        public CemiRaw(MessageCode messageCode, byte[] payload)
        {
            Payload = payload ?? Array.Empty<byte>();
            Size = 1 + Payload.Length;
            MessageCode = messageCode;
        }

        public string ToDescription(int padding = 4) 
            => $"CemiRaw: MC = {MessageCode}, Payload = {BitConverter.ToString(Payload)}".AddPrefixSpaces(padding);
    }

}