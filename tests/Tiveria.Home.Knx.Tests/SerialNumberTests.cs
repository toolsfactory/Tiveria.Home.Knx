using NUnit.Framework;
using Tiveria.Home.Knx.Primitives;

namespace Tiveria.Home.Knx.Tests
{
    [TestFixture]
    public class SerialNumberTests
    {
        static object[] TestData = new object[]
        {
            new object[] { (ulong) 0x0011_2233_4455, "0011:22334455", new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 } },
            new object[] { (ulong) 0xaabb_ccdd_eeff, "aabb:ccddeeff", new byte[] { 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff } },
            new object[] { (ulong) 0x0083_53b8_485f, "0083:53b8485f", new byte[] { 0x00, 0x83, 0x53, 0xb8, 0x48, 0x5f } },
        };

        [TestCaseSource(nameof(TestData))]
        [Test]
        public void Basics(ulong serno, string representation, byte[] bytes)
        {
            var data = new SerialNumber(serno);
            Assert.AreEqual(serno, data.Value);
            Assert.AreEqual(representation, data.ToString());
            Assert.AreEqual(bytes, data.ToBytes());
        }

        [Test]
        public void Zero()
        {
            var data = SerialNumber.Zero;
            Assert.AreEqual(0, data.Value);
            Assert.AreEqual("0000:00000000", data.ToString());
        }
    } 
}
