using NUnit.Framework;
using Tiveria.Home.Knx.Cemi;

namespace Tiveria.Home.Knx.Tests
{
    [TestFixture]
    class ControlField2Tests
    {
        static object[] TestDataOk =
        {
            // Groupaddress, Hopcount, ExtendeFrameFormat
            new object[] { true,  0, 0, 0b1_000_0000 },
            new object[] { true,  2, 0, 0b1_010_0000 },
            new object[] { true,  6, 0, 0b1_110_0000 },
            new object[] { true,  7, 0, 0b1_111_0000 },
            new object[] { true,  7, 8, 0b1_111_1000 },
            new object[] { true,  7,15, 0b1_111_1111 },
            new object[] { false, 0, 0, 0b0_000_0000 },
            new object[] { false, 2, 0, 0b0_010_0000 },
            new object[] { false, 6, 0, 0b0_110_0000 },
            new object[] { false, 7, 0, 0b0_111_0000 },
            new object[] { false, 7, 8, 0b0_111_1000 },
            new object[] { false, 7,15, 0b0_111_1111 },
        };

        [TestCaseSource(nameof(TestDataOk))]
        [Test]
        public void Build(bool groupadd, int hops, int ext, int expected)
        {
            var ctrl2 = new ControlField2(groupadd, hops, ext);
            var data = ctrl2.ToByte();
            Assert.AreEqual((byte)expected, data);
        }

        [TestCaseSource(nameof(TestDataOk))]
        [Test]
        public void Parse(bool groupadd, int hops, int ext, int expected)
        {
            var ctrl2 = new ControlField2((byte)expected);
            var data = ctrl2.ToByte();
            Assert.AreEqual(groupadd, ctrl2.DestinationAddressType == Adresses.AddressType.GroupAddress);
            Assert.AreEqual(ext, ctrl2.ExtendedFrameFormat);
            Assert.AreEqual(hops, ctrl2.HopCount);
            Assert.AreEqual((byte)expected, data);
        }
    }
}
