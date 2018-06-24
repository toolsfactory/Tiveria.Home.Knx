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
using System;

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
        #region private fields
        private readonly CemiMessageCode _messageCode;
        private byte _rawData;
        private bool _extendedFrame;
        private bool _repeat;
        private BroadcastType _broadcast;
        private Priority _priority;
        private bool _acknowledgeRequest;
        private ConfirmType _confirm;
        #endregion

        #region public properties
        public byte RawData => _rawData;
        public bool ExtendedFrame => _extendedFrame;
        public bool Repeat => _repeat;
        public BroadcastType Broadcast => _broadcast;
        public Priority Priority => _priority;
        public bool AcknowledgeRequest => _acknowledgeRequest; 
        public ConfirmType Confirm => _confirm; 
        #endregion

        #region constructors
        /// <summary>
        ///  Creates a ControlField1 class from an encoded byte that is parsed to the according properties
        /// </summary>
        /// <param name="messageCode">The cemi message code the controlfield was sent with</param>
        /// <param name="data">ControlField1 as byte representation</param>
        public ControlField1(CemiMessageCode messageCode, byte data)
        {
            _messageCode = messageCode;
            _rawData = data;
            ParseData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mc"></param>
        /// <param name="prio"></param>
        /// <param name="repeat"></param>
        /// <param name="broadcast"></param>
        /// <param name="ack"></param>
        /// <param name="confirm"></param>
        public ControlField1(CemiMessageCode mc, bool extendedFrame =false, Priority prio = Priority.Normal, bool repeat = true, BroadcastType broadcast = BroadcastType.Normal, bool ack = true, ConfirmType confirm = ConfirmType.NoError)
        {
            _messageCode = mc;
            _extendedFrame = extendedFrame;
            _repeat = repeat;
            _broadcast = broadcast;
            _priority = prio;
            _acknowledgeRequest = ack;
            _confirm = confirm;
            ToByte();
        }
        #endregion

        #region private methods
        /// <summary>
        /// Convert the properties to the corresponding byte representation
        /// </summary>
        private void ToByte()
        {
            _rawData = 0;

            if (!_extendedFrame)
                _rawData |= 0b1000_0000;

            var repflag = _messageCode == CemiMessageCode.LDATA_IND ? !Repeat : Repeat;
            if (repflag)
                _rawData |= 0b0010_0000;

            if (_broadcast == BroadcastType.System)
                _rawData |= 0x10;

            if (_acknowledgeRequest)
                _rawData |= 0x02;

            if (_confirm == ConfirmType.Error)
                _rawData |= 0x01;
        }
        /// <summary>
        /// Parse the byte and set the properties accordingly
        /// </summary>
        private void ParseData()
        {
            _broadcast = (_rawData & 0x10) == 0x10 ? BroadcastType.Normal : BroadcastType.System;
            _confirm = (_rawData & 0x01) == 0 ? ConfirmType.NoError : ConfirmType.Error;
            _extendedFrame = (_rawData & 0x80) == 0;
            _acknowledgeRequest = (_rawData & 0x02) != 0;

            if (_messageCode == CemiMessageCode.LDATA_IND)
                // ind: flag 0 = repeated frame, 1 = not repeated
                _repeat = (_rawData & 0x20) == 0;
            else
                // req, (con): flag 0 = do not repeat, 1 = default behavior
                _repeat = (_rawData & 0x20) == 0x20;

            var bits = _rawData >> 2 & 0x03;
            _priority = (Priority)bits;
        }
        #endregion

        public string ToDescription(int padding)
        {
            var spaces = new String(' ', padding);
            return $"{spaces}Ctrl1: ExtendedFrame = {ExtendedFrame}, Repeat = {Repeat}, Broadcast = {Broadcast}, Priority = {Priority}, Acknowledge = {AcknowledgeRequest}, Confirm = {Confirm}";
        }

    }
}