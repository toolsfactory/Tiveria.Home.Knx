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

namespace Tiveria.Home.Knx.Cemi
{
    /// <summary>
    /// Represents the Control Field 1 of a cEMI structure.
    /// Details can be found in "03_06_03_EMI_IMI V01.03.03 AS.PDF" Chapter 4.1.5.3 
    /// <code>
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// | bit 7  | bit 6  | bit 5  | bit 4  | bit 3  | bit 2  | bit 1  | bit 0  |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// |  Frame | empty  | Repeat | System |Priority         | Acknow | Confirm|
    /// |  Type  | 0      |        | Brdcst |                 | ledge  |        |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// </code>
    /// 
    /// Frame Type:         This shall specify the Frame Type that shall be used for transmission or reception of the frame. 
    ///                     0 = extended frame, 1 = normal frame
    /// Repeat Flag:        Repeat, not valid for all media 
    ///                     0 = repeat frame on medium if error, 1 = do not repeat
    /// System Broadcast:   This shall specify whether the frame is transmitted using system broadcast communication mode or broadcast communication mode(applicable only on open media)
    ///                     0 = system broadcast 1 = broadcast
    /// Priority:           This shall specify that Priority that shall be used for transmission or reception of the frame 
    ///                     See Priority Class
    /// Acknowledge:        This shall specify whether a L2-acknowledge shall be requested for the L_Data.req frame or not.This is not valid for all media. 
    ///                     0 = no acknowledge is requested, 1 = acknowledge requested
    /// Confirm:            In L_Data.con this shall indicate whether there has been any error in the transmitted frame.
    ///                     0 = no error, 1 = error
    /// </summary>

    public class ControlField1
    {

        #region public properties
        public bool ExtendedFrame { get; set; }
        public bool Repeat { get; set; }
        public BroadcastType Broadcast { get; set; }
        public Priority Priority { get; set; }
        public bool AcknowledgeRequest { get; set; }
        public ConfirmType Confirm { get; set; }
        #endregion

        #region constructors
        /// <summary>
        ///  Creates a ControlField1 class from an encoded byte that is parsed to the according properties
        /// </summary>
        /// <param name="messageCode">The cemi message code the controlfield was sent with</param>
        /// <param name="data">ControlField1 as byte representation</param>
        public ControlField1(MessageCode messageCode, byte data)
        {
            Broadcast = (data & 0x10) == 0x10 ? BroadcastType.Normal : BroadcastType.System;
            Confirm = (data & 0x01) == 0 ? ConfirmType.NoError : ConfirmType.Error;
            ExtendedFrame = (data & 0x80) == 0;
            AcknowledgeRequest = (data & 0x02) != 0;

            if (messageCode == MessageCode.LDATA_IND)
                // ind: flag 0 = repeated frame, 1 = not repeated
                Repeat = (data & 0x20) == 0;
            else
                // req, (con): flag 0 = do not repeat, 1 = default behavior
                Repeat = (data & 0x20) == 0x20;

            var bits = data >> 2 & 0x03;
            Priority = (Priority)bits;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="repeat"></param>
        /// <param name="broadcast"></param>
        /// <param name="ack"></param>
        /// <param name="confirm"></param>
        public ControlField1(bool extendedFrame = false, Priority priority = Priority.Normal, bool repeat = true, BroadcastType broadcast = BroadcastType.Normal, bool ack = true, ConfirmType confirm = ConfirmType.NoError)
        {
            ExtendedFrame = extendedFrame;
            Repeat = repeat;
            Broadcast = broadcast;
            Priority = priority;
            AcknowledgeRequest = ack;
            Confirm = confirm;
        }
        #endregion

        /// <summary>
        /// Convert the properties to the corresponding byte representation
        /// </summary>
        public byte ToByte(MessageCode messageCode = MessageCode.LDATA_REQ)
        {
            var raw = (byte) (((byte)Priority & 0x03) << 2);

            if (!ExtendedFrame)
                raw |= 0b1000_0000;

            var repflag = messageCode == MessageCode.LDATA_IND ? !Repeat : Repeat;
            if (repflag)
                raw |= 0b0010_0000;

            if (Broadcast == BroadcastType.Normal)
                raw |= 0x10;

            if (AcknowledgeRequest)
                raw |= 0x02;

            if (Confirm == ConfirmType.Error)
                raw |= 0x01;
            return raw;
        }

        public override string ToString()
        {
            return $"(Ext {ExtendedFrame}, Rep {Repeat}, BCast {Broadcast}, Prio {Priority}, Ack {AcknowledgeRequest}, C {Confirm})";
        }
    }
}