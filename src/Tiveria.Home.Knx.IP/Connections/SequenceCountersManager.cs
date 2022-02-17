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

namespace Tiveria.Home.Knx.IP.Connections
{
    internal class SequenceCountersManager
    {
        private byte _rcvSeqCounter = 0;
        private byte _sndSeqCounter = 0;
        private object _seqLock = new object();

        internal byte RcvSeqCounter => _rcvSeqCounter;

        internal byte SndSeqCounter => _sndSeqCounter;

        #region Sequence Counters
        internal void ResetCounters()
        {
            lock (_seqLock)
            {
                _rcvSeqCounter = 0;
                _sndSeqCounter = 0;
            }
        }

        internal byte IncRcvSeqCounter()
        {
            byte result = 0;
            lock (_seqLock)
            {
                _rcvSeqCounter++;
                result = _rcvSeqCounter;
            }
            return result;
        }


        internal byte IncSndSeqCounter()
        {
            byte result = 0;
            lock (_seqLock)
            {
                _sndSeqCounter++;
                result = _sndSeqCounter;
            }
            return result;
        }

        internal bool ValidateReqSequenceCounter(byte rcvSeq, bool resyncCounters)
        {
            // copied from Calimero
            // Workaround for missed request problem (not part of the knxnet/ip tunneling spec):
            // we missed a single request, hence, the receive sequence is one behind. If the remote
            // endpoint didn't terminate the connection, but continues to send requests, this workaround
            // re-syncs with the sequence of the sender.
            var expSeq = _rcvSeqCounter;
            var missed = (expSeq - 1 == expSeq);
            if (missed && resyncCounters)
            {
                IncRcvSeqCounter();
                expSeq++;
            }
            IncRcvSeqCounter();
            return (rcvSeq == expSeq) || (rcvSeq == ++expSeq);
        }

        #endregion
    }
}