/*
Tiveria.Knx - a .Net Core base KNX library
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

using Tiveria.Knx.Exceptions;

namespace Tiveria.Knx.Datapoint
{
    public class DPType3BitControl : DPType<(byte Value, bool Control)>
    {
        protected DPType3BitControl(string id, string name, (byte Value, bool Control) min = default, (byte Value, bool Control) max = default, string unit = "", string description = "") : base(id, name, min, max, unit, description)
        {
        }

        public override bool ToBoolValue(byte[] data)
        {
            throw new TranslationExcception("Translation not supported");
        }

        public override double ToDoubleValue(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public override long ToLongValue(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public override string ToStringValue(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public override (byte Value, bool Control) ToValue(byte[] data)
        {
            throw new System.NotImplementedException();
        }


        public override byte[] ToData((byte Value, bool Control) value)
        {
            throw new System.NotImplementedException();
        }

        public override byte[] ToData(string value)
        {
            throw new System.NotImplementedException();
        }

        public override byte[] ToData(double value)
        {
            throw new System.NotImplementedException();
        }

        public override byte[] ToData(long value)
        {
            throw new System.NotImplementedException();
        }

    }
}
