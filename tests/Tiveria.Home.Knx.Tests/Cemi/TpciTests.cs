using NUnit.Framework;
using Tiveria.Home.Knx.Cemi;

namespace Tiveria.Home.Knx.Tests
{
    [TestFixture]
    public class TpciTests
    {
        /// +-----------------------------------------------------------------------+
        /// |          NPDU Byte 1: 6 bit TPCI & 2 bit APCI                         |
        /// +--------+--------+--------+--------+--------+--------+--------+--------+
        /// | bit 0  | bit 1  | bit 2  | bit 3  | bit 4  | bit 5  | bit 6  | bit 7  |
        /// +--------+--------+--------+--------+--------+--------+--------+--------+
        /// |                       TPCI Data                     |    APCI         |
        /// +--------+--------+--------+--------+--------+--------+--------+--------+
        /// | Packet | Sequ-  |          Sequence Number          |                 |
        /// | Type   | encing |                                   |                 |
        /// | Flag   | Flag   |                                   |                 |
        /// +--------+--------+--------+--------+--------+--------+--------+--------+
        static object[] TestDataOk =
        {
            new object[] { PacketType.Data,    SequenceType.UnNumbered, 0, ControlType.None,       0b0_0_0000_00 },
            new object[] { PacketType.Control, SequenceType.UnNumbered, 0, ControlType.None,       0b1_0_0000_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,   0, ControlType.None,       0b1_1_0000_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,   1, ControlType.None,       0b1_1_0001_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,   2, ControlType.None,       0b1_1_0010_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,   3, ControlType.None,       0b1_1_0011_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,   4, ControlType.None,       0b1_1_0100_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,   5, ControlType.None,       0b1_1_0101_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,   6, ControlType.None,       0b1_1_0110_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,   7, ControlType.None,       0b1_1_0111_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,   8, ControlType.None,       0b1_1_1000_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,   9, ControlType.None,       0b1_1_1001_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,  10, ControlType.None,       0b1_1_1010_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,  11, ControlType.None,       0b1_1_1011_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,  12, ControlType.None,       0b1_1_1100_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,  13, ControlType.None,       0b1_1_1101_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,  14, ControlType.None,       0b1_1_1110_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,  15, ControlType.None,       0b1_1_1111_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,  15, ControlType.Connect,    0b1_1_1111_00 },
            new object[] { PacketType.Control, SequenceType.Numbered,  15, ControlType.Disconnect, 0b1_1_1111_01 },
            new object[] { PacketType.Control, SequenceType.Numbered,  15, ControlType.Ack,        0b1_1_1111_10 },
            new object[] { PacketType.Control, SequenceType.Numbered,  15, ControlType.NAck,       0b1_1_1111_11 },
        };

        [TestCaseSource(nameof(TestDataOk))]
        [Test]
        public void Build(PacketType packet, SequenceType sequence, int no, ControlType ctrl, int expected)
        {
            var tpci = new Tpci(packet, sequence, (byte)no, ctrl);
            var data = tpci.ToByte();
            Assert.AreEqual((byte)expected, data);
        }

        [TestCaseSource(nameof(TestDataOk))]
        [Test]
        public void Parse(PacketType packet, SequenceType sequence, int no, ControlType ctrl, int expected)
        {
            var tpci = new Tpci((byte)expected);
            var data = tpci.ToByte();
            Assert.AreEqual(packet, tpci.PacketType);
            Assert.AreEqual(sequence, tpci.SequenceType);
            Assert.AreEqual(no, tpci.SequenceNumber);
            Assert.AreEqual(ctrl, tpci.ControlType);
            Assert.AreEqual((byte)expected, data);
        }

        [Test]
        public void Default()
        {
            var tpci = new Tpci();
            var data = tpci.ToByte();
            Assert.AreEqual(PacketType.Data, tpci.PacketType);
            Assert.AreEqual(SequenceType.UnNumbered, tpci.SequenceType);
            Assert.AreEqual(0, tpci.SequenceNumber);
            Assert.AreEqual(ControlType.None, tpci.ControlType);
            Assert.AreEqual((byte)0, data);

        }

    }
}
