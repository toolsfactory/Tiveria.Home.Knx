using NUnit.Framework;
using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Common.Extensions;
using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.ObjectServer;
using Tiveria.Common.IO;

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
            var reader = new BigEndianBinaryReader(new MemoryStream(data));
            var result = FrameHeader.Parse(reader);
            Assert.AreEqual(ServiceTypeIdentifier.ObjectServer, result.ServiceTypeIdentifier);
            Assert.AreEqual(0xf080, result.ServiceTypeIdentifierRaw);
        }

        [Test]
        public void ParseIpFrame()
        {
            var data = "0620F080001504000000F006000100010001030101".ToByteArray();
            var serializer = KnxNetIPFrameSerializerFactory.Instance.Create(ServiceTypeIdentifier.Unknown);
            var frame = serializer.Deserialize(data);
            Assert.AreEqual(ServiceTypeIdentifier.ObjectServer, frame.FrameHeader.ServiceTypeIdentifier);
            Assert.AreEqual(0xf080, frame.FrameHeader.ServiceTypeIdentifierRaw);
        }

        /*
        [Test]
        public void ParseMultiIpFrame()
        {
            OldIPFrame frame1, frame2, frame3;
            var data = "0620F080001504000000F0060035000100350301010620F080001504000000F00600870001008703010100".ToByteArray();
            var serializer = KnxNetIPFrameSerializerFactory.Instance.Create(ServiceTypeIdentifier.UNKNOWN);
            var frame = serializer.Deserialize(data);
            bool ok1 = serializer.TryParse(out frame1, data);
            bool ok2 = OldIPFrame.TryParse(out frame2, data, frame1.FrameHeader.TotalLength);
            bool ok3 = OldIPFrame.TryParse(out frame3, data, frame1.FrameHeader.TotalLength + frame2.FrameHeader.TotalLength);
            Assert.AreEqual(ServiceTypeIdentifier.OBJECTSERVER, frame1.FrameHeader.ServiceTypeIdentifier);
            Assert.AreEqual(0xf080, frame2.FrameHeader.ServiceTypeRaw);
        }
        */

        [Test]
        public void ParseSetDataPointReq()
        {
            var data = "F006003500010035030101".ToByteArray();
            var dp = new SetDatapointValueReqService(data);
            Assert.AreEqual(1, dp.NumberOfDataPoints);
        }

        [Test]
        public void ParseMultiSetDataPointReq()
        {
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
