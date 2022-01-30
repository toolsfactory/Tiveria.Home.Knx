using NUnit.Framework;
using Tiveria.Home.Knx.Cemi;

namespace Tiveria.Home.Knx.Tests
{
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// | bit 7  | bit 6  | bit 5  | bit 4  | bit 3  | bit 2  | bit 1  | bit 0  |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    /// |  Frame | empty  | Repeat | System |Priority         | Acknow | Confirm|
    /// |  Type  | 0      |        | Brdcst |                 | ledge  |        |
    /// +--------+--------+--------+--------+--------+--------+--------+--------+
    [TestFixture]
    class ControlField1Tests
    {
        static object[] TestDataOk =
        {
            // messagegode, Flags, ..., expected   (only mc LDATA_REQ has impact as it flips repeat)
            new object[] { MessageCode.LDATA_IND, false, false, BroadcastType.Normal, Priority.Normal, false, ConfirmType.NoError, 0b10_11_01_00 },

            new object[] { MessageCode.LDATA_IND, true,  false, BroadcastType.Normal, Priority.Normal, false, ConfirmType.NoError, 0b00_11_01_00 },
            new object[] { MessageCode.LDATA_IND, false, true,  BroadcastType.Normal, Priority.Normal, false, ConfirmType.NoError, 0b10_01_01_00 },
            new object[] { MessageCode.LDATA_IND, false, false, BroadcastType.System, Priority.Normal, false, ConfirmType.NoError, 0b10_10_01_00 },
            new object[] { MessageCode.LDATA_IND, false, false, BroadcastType.Normal, Priority.Low   , false, ConfirmType.NoError, 0b10_11_11_00 },
            new object[] { MessageCode.LDATA_IND, false, false, BroadcastType.Normal, Priority.Normal, false, ConfirmType.NoError, 0b10_11_01_00 },
            new object[] { MessageCode.LDATA_IND, false, false, BroadcastType.Normal, Priority.Urgent, false, ConfirmType.NoError, 0b10_11_10_00 },
            new object[] { MessageCode.LDATA_IND, false, false, BroadcastType.Normal, Priority.System, false, ConfirmType.NoError, 0b10_11_00_00 },
            new object[] { MessageCode.LDATA_IND, false, false, BroadcastType.Normal, Priority.Normal, true,  ConfirmType.NoError, 0b10_11_01_10 },
            new object[] { MessageCode.LDATA_IND, false, false, BroadcastType.Normal, Priority.Normal, false, ConfirmType.Error,   0b10_11_01_01 },


            new object[] { MessageCode.LDATA_REQ, true,  false, BroadcastType.Normal, Priority.Normal, false, ConfirmType.NoError, 0b00_01_01_00 },
            new object[] { MessageCode.LDATA_REQ, false, true,  BroadcastType.Normal, Priority.Normal, false, ConfirmType.NoError, 0b10_11_01_00 },
            new object[] { MessageCode.LDATA_REQ, false, false, BroadcastType.System, Priority.Normal, false, ConfirmType.NoError, 0b10_00_01_00 },
            new object[] { MessageCode.LDATA_REQ, false, false, BroadcastType.Normal, Priority.Low   , false, ConfirmType.NoError, 0b10_01_11_00 },
            new object[] { MessageCode.LDATA_REQ, false, false, BroadcastType.Normal, Priority.Normal, false, ConfirmType.NoError, 0b10_01_01_00 },
            new object[] { MessageCode.LDATA_REQ, false, false, BroadcastType.Normal, Priority.Urgent, false, ConfirmType.NoError, 0b10_01_10_00 },
            new object[] { MessageCode.LDATA_REQ, false, false, BroadcastType.Normal, Priority.System, false, ConfirmType.NoError, 0b10_01_00_00 },
            new object[] { MessageCode.LDATA_REQ, false, false, BroadcastType.Normal, Priority.Normal, true,  ConfirmType.NoError, 0b10_01_01_10 },
            new object[] { MessageCode.LDATA_REQ, false, false, BroadcastType.Normal, Priority.Normal, false, ConfirmType.Error,   0b10_01_01_01 },
        };

        [TestCaseSource(nameof(TestDataOk))]
        [Test]
        public void Build(MessageCode mc, bool extFrame, bool repeat, BroadcastType brdcst, Priority prio, bool ack, ConfirmType confirm, int expected)
        {
            var ctrl1 = new ControlField1(extFrame, prio, repeat, brdcst, ack, confirm);
            var data = ctrl1.ToByte(mc);
            Assert.AreEqual((byte)expected, data);
        }

        [TestCaseSource(nameof(TestDataOk))]
        [Test]
        public void Parse(MessageCode mc, bool extFrame, bool repeat, BroadcastType brdcst, Priority prio, bool ack, ConfirmType confirm, int expected)
        {
            var ctrl1 = new ControlField1(mc,(byte)expected);
            var data = ctrl1.ToByte(mc);
            Assert.AreEqual(extFrame, ctrl1.ExtendedFrame);
            Assert.AreEqual(prio, ctrl1.Priority);
            Assert.AreEqual(repeat, ctrl1.Repeat);
            Assert.AreEqual(brdcst, ctrl1.Broadcast);
            Assert.AreEqual(ack, ctrl1.AcknowledgeRequest);
            Assert.AreEqual(confirm, ctrl1.Confirm);
            Assert.AreEqual((byte)expected, data);
        }

        [Test]
        public void Default()
        {
            var ctrl1 = new ControlField1();
            var data = ctrl1.ToByte();
            Assert.AreEqual(false, ctrl1.ExtendedFrame);
            Assert.AreEqual(Priority.System, ctrl1.Priority);
            Assert.AreEqual(false, ctrl1.Repeat);
            Assert.AreEqual(BroadcastType.Normal, ctrl1.Broadcast);
            Assert.AreEqual(false, ctrl1.AcknowledgeRequest);
            Assert.AreEqual(ConfirmType.NoError, ctrl1.Confirm);
            Assert.AreEqual(0b10_01_00_00, data);
        }

    }
}
