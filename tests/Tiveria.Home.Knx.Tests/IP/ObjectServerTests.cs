using NUnit.Framework;
using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Common.Extensions;
using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.ObjectServer;

namespace Tiveria.Home.Knx.Tests.IP
{
    [TestFixture]
    public class ObjectServerTests
    {
        [Test]
        public void ParseHeaderFromBytesUnknownServiceTypeIdentifier()
        {
            //Standard body for Connection_Request but ServiceTypeIdentifier set to 0x00ff
            var data = "0620F080001504000000F006000100010001030101".ToByteArray();
            var result = FrameHeader.Parse(data, 0);
            Assert.AreEqual(ServiceTypeIdentifier.OBJECTSERVER, result.ServiceTypeIdentifier);
            Assert.AreEqual(0xf080, result.ServiceTypeRaw);
        }

        [Test]
        public void ParseIpFrame()
        {
            var data = "0620F080001504000000F006000100010001030101".ToByteArray();
            var frame = KnxNetIPFrame.Parse(data);
            Assert.AreEqual(ServiceTypeIdentifier.OBJECTSERVER, frame.Header.ServiceTypeIdentifier);
            Assert.AreEqual(0xf080, frame.Header.ServiceTypeRaw);
        }

        [Test]
        public void ParseMultiIpFrame()
        {
            KnxNetIPFrame frame1, frame2, frame3;
            var data = "0620F080001504000000F0060035000100350301010620F080001504000000F00600870001008703010100".ToByteArray();
            bool ok1 = KnxNetIPFrame.TryParse(out frame1, data);
            bool ok2 = KnxNetIPFrame.TryParse(out frame2, data, frame1.Header.TotalLength);
            bool ok3 = KnxNetIPFrame.TryParse(out frame3, data, frame1.Header.TotalLength + frame2.Header.TotalLength);
            Assert.AreEqual(ServiceTypeIdentifier.OBJECTSERVER, frame1.Header.ServiceTypeIdentifier);
            Assert.AreEqual(0xf080, frame2.Header.ServiceTypeRaw);
        }

        [Test]
        public void ParseSetDataPointReq()
        {
            KnxNetIPFrame frame1, frame2, frame3;
            var data = "F006003500010035030101".ToByteArray();
            var dp = new SetDatapointValueReqService(data);
            Assert.AreEqual(1, dp.NumberOfDataPoints);
        }

        [Test]
        public void ParseMultiSetDataPointReq()
        {
            KnxNetIPFrame frame1, frame2, frame3;
            var data = "F006 0035 0003 0035030101 00360303010203 0035030101".RemoveAll(' ').ToByteArray();
            var dp = new SetDatapointValueReqService(data);
            var data2 = dp.ToBytes();
            Assert.AreEqual(data, data2);
            Assert.AreEqual(3, dp.NumberOfDataPoints);
        }

        [Test]
        public void BuildSetDataPointRes()
        {
            var expected = "F086 0035 0000 00".RemoveAll(' ').ToByteArray();
            var dp = new SetDatapointValueResService(53);
            var answer = dp.ToBytes();
            Assert.AreEqual(expected, answer);
        }

    }
}

//0620F080001504000000F0060035000100350301010620F080001504000000F00600870001008703010100
