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

using System.Net;

namespace Tiveria.Home.Knx.IP
{
    public delegate void PacketReceivedDelegate(DateTime timestamp, IPEndPoint source, IPEndPoint receiver, byte[] data);
    public delegate void KnxFrameReceivedDelegate(DateTime timestamp, IPEndPoint source, IPEndPoint receiver, IKnxNetIPFrame frame);

    public class FrameReceivedEventArgs : EventArgs
    {
        private readonly IKnxNetIPFrame _frame;
        private readonly DateTime _timestamp;
        private readonly bool _handled;

        public IKnxNetIPFrame Frame => _frame; 
        public DateTime TimeStamp => _timestamp;
        public bool Handled => _handled;
        public FrameReceivedEventArgs(DateTime timestamp, IKnxNetIPFrame frame, bool handled)
        {
            _timestamp = timestamp;
            _frame = frame;
            _handled = handled;
        }
    }
}
