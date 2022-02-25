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
    public class DeviceConnectionSession
    {
        public DeviceConnectionSession(IndividualAddress address, bool keepAlive)
        {
            Address = address;
            KeepAlive = keepAlive;

            _timer = new System.Timers.Timer(KnxConstants.DeviceConnectionTimeout);
            _timer.AutoReset = false;
            _timer.Elapsed += _timer_Elapsed;
        }

        public int SeqNoSend { get; private set; } = 0;
        public int SeqNoReceive { get; private set; } = 0;
        public IndividualAddress Address { get; }
        public bool KeepAlive { get; }
        public ManagementConnectionState State { get; private set; } = ManagementConnectionState.Closed;

        public void IncSeqNoSend() => SeqNoSend = ++SeqNoSend & 0x0f;
        public void IncSeqNoReceive() => SeqNoReceive = ++SeqNoReceive & 0x0f;

        private System.Timers.Timer _timer;

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

    }


}
