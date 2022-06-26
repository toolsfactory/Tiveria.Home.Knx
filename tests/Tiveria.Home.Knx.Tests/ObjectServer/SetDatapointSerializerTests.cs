using NUnit.Framework;
using Tiveria.Common.Extensions;
using Tiveria.Common.IO;
using Tiveria.Home.Knx.ObjectServer.Services.Serializers;
using Tiveria.Home.Knx.ObjectServer;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.Tests.ObjectServer
{
    [TestFixture]
    public class SetDatapointSerializerTests
    {
        #region Testcases for SetDatapointValue.Req

        static object[] TestData_SetDatapointValueReq = new object[]
        {
            // input, mainservice, subservice, StartDP, NumDP
            new object[] { "F00600060001000603020F12",  0xF0, 0x06, 0x06, 0x01, new DataPoint[]{ new DataPoint(0x06, 0x03, 0x02, new byte[] { 0x0f, 0x12 }) } },
            new object[] { "F006000100010001030101",    0xF0, 0x06, 0x01, 0x01, new DataPoint[]{ new DataPoint(0x01, 0x03, 0x01, new byte[] { 0x01 }) } },
            new object[] { "F006003500010035030101",    0xF0, 0x06, 0x35, 0x01, new DataPoint[]{ new DataPoint(0x35, 0x03, 0x01, new byte[] { 0x01 }) } },
            new object[] { "F00600070001000703021645",  0xF0, 0x06, 0x07, 0x01, new DataPoint[]{ new DataPoint(0x07, 0x03, 0x02, new byte[] { 0x16, 0x45 }) } },
            new object[] { "F006000700020007030216450035030101",  0xF0, 0x06, 0x07, 0x02, new DataPoint[]{ new DataPoint(0x07, 0x03, 0x02, new byte[] { 0x16, 0x45 }), new DataPoint(0x35, 0x03, 0x01, new byte[] { 0x01 }) } }
        };

        [TestCaseSource(nameof(TestData_SetDatapointValueReq))]
        [Test]
        public void ParseReq_Default_Ok(string input, int main, int sub, int startdp, int numdp, DataPoint[] dps)
        {
            var reader = new BigEndianBinaryReader(input.RemoveAll(' ').ToByteArray());
            var deserializer = new SetDatapointValueReqServiceSerializer();
            var req = deserializer.Deserialize(reader);
            Assert.IsNotNull(req);
            Assert.AreEqual(main, req.MainService);
            Assert.AreEqual(sub, req.SubService);
            Assert.AreEqual(startdp, req.StartDataPoint);
            Assert.AreEqual(numdp, req.NumberOfDataPoints);
            Assert.AreEqual(dps.Length, numdp);
            for (var i = 0; i < numdp; i++)
            {
                Assert.AreEqual(req.DataPoints[i].ID, dps[i].ID);
                Assert.AreEqual(req.DataPoints[i].Length, dps[i].Length);
                Assert.AreEqual(req.DataPoints[i].Command, dps[i].Command);
                Assert.AreEqual(req.DataPoints[i].Value, dps[i].Value);
            }
        }

        [Test]
        public void ParseReq_NoDP_Ok()
        {
            var input = "F00600010000";
            var reader = new BigEndianBinaryReader(input.RemoveAll(' ').ToByteArray());
            var deserializer = new SetDatapointValueReqServiceSerializer();
            var req = deserializer.Deserialize(reader);
            Assert.IsNotNull(req);
            Assert.AreEqual(0xF0, req.MainService);
            Assert.AreEqual(0x06, req.SubService);
            Assert.AreEqual(1, req.StartDataPoint);
            Assert.AreEqual(0, req.NumberOfDataPoints);
        }

        [Test]
        public void ParseReq_InvalidMainService()
        {
            var input = "A006000100010001030101"; // A0 instead of F0 as main service
            var reader = new BigEndianBinaryReader(input.RemoveAll(' ').ToByteArray());
            var deserializer = new SetDatapointValueReqServiceSerializer();
            Assert.Throws<KnxBufferFieldException>(() => deserializer.Deserialize(reader));
        }

        [Test]
        public void ParseReq_InvalidSubService()
        {
            var input = "F011000100010001030101"; // 11 instead of 06 as main service
            var reader = new BigEndianBinaryReader(input.RemoveAll(' ').ToByteArray());
            var deserializer = new SetDatapointValueReqServiceSerializer();
            Assert.Throws<KnxBufferFieldException>(() => deserializer.Deserialize(reader));
        }

        [Test]
        public void ParseReq_DPCountBufferSizeMissmatch1()
        {
            var input = "F00600010001"; // One DP should exist - but no data there (Buffer empty)
            var reader = new BigEndianBinaryReader(input.RemoveAll(' ').ToByteArray());
            var deserializer = new SetDatapointValueReqServiceSerializer();
            Assert.Throws<KnxBufferSizeException>(() => deserializer.Deserialize(reader));
        }

        [Test]
        public void ParseReq_DPCountBufferSizeMissmatch2()
        {
            var input = "F006000100000001030101"; // No DP should exist - but data there
            var reader = new BigEndianBinaryReader(input.RemoveAll(' ').ToByteArray());
            var deserializer = new SetDatapointValueReqServiceSerializer();
            Assert.Throws<KnxBufferSizeException>(() => deserializer.Deserialize(reader));
        }
        #endregion
    }
}
