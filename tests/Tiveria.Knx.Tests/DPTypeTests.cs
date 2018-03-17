using System;
using System.Net;
using NUnit.Framework;
using Tiveria.Knx.Cemi;
using Tiveria.Common.Extensions;
using Tiveria.Common.IO;

namespace Tiveria.Knx.Tests
{
    [TestFixture]
    class DPTypeTests
    {
        [Test]
        public void Decode1()
        {
            //            MEEEEMMM MMMMMMMM
            // 0C7E  23,0 00001100 01111110
            // 8B82 -23,0 10001011 10000010
            // 0C6F  22,7 00001100 01101111
            // 8B61 -22,7 10001011 10010001
            // 0C51  22,1 00001100 01010001
            var val1 = Encode(-23.0);
            var val2 = Encode(-22.7);
            var val3 = Encode(-22.1);
            Assert.AreEqual(23.0, DecodeOrig(0x0c, 0x7e));
            Assert.AreEqual(23.0, DecodeNew(0x0c, 0x7e));
            Assert.AreEqual(-23.0, DecodeOrig(0x8b, 0x82));
            Assert.AreEqual(-23.0, DecodeNew(0x8b, 0x82));
        }

        private double DecodeOrig(byte msb, byte lsb)
        {
            int v = ((msb & 0x80) << 24) | ((msb & 0x7) << 28) | (lsb << 20);
            // normalize
            v >>= 20;
            int exp = (msb & 0x78) >> 3;
            var multi = (1 << exp);
            var mant = v * 0.01;
            return mant * multi;
        }

        private double DecodeNew(byte msb, byte lsb)
        {
            var sign = (msb & 0b1000_0000) << 24;
            var mant = (msb & 0b0000_0111) << 28 | (lsb << 20) | sign;
            var exp  = (msb & 0b0111_1000) >> 3;
            return (1 << exp) * (mant >> 20) *0.01;
        }

        private (byte msb, byte lsb) Encode(double value)
        {
            double v = value * 100.0f;
            int e = 0;
            for (; v < -2048.0; v /= 2)
                e++;
            for (; v > 2047.0; v /= 2)
                e++;
            var m = (int)Math.Round(v) & 0x7FF;
            var msb = (short)(e << 3 | m >> 8);
            if (value < 0.0)
                msb |= 0x80;
            var lsb = (byte)m;
            return ((byte)msb, lsb);
        }
        
    }
}
