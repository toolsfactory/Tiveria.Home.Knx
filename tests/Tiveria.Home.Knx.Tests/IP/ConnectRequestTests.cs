using System.Net;
using NUnit.Framework;
using Tiveria.Common.Extensions;
using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.Tests.IP
{
    [TestFixture]
    public class ConnectRequest
    {
        [Test]
        public void TestMethod()
        {
/*
            var tunnelingrequestbody = new ConnectionRequest(IPAddress.Parse("192.168.2.120"), 57846, 57847);
            var reference = "0801c0a80278e1f60801c0a80278e1f704040200".ToByteArray();
            var data = tunnelingrequestbody.ToBytes();
            Assert.AreEqual(data, reference);
*/
        }

        [Test]
        public void TestMethod2()
        {
            var request = "061004200017041300002900bce011a28d000300800c7e".ToByteArray();
            var reader = new Common.IO.BigEndianBinaryReader(request);
            var header = FrameHeader.Parse(reader);
            var parser = KnxNetIPFrameSerializerFactory.Instance.Create(header.ServiceTypeIdentifier);
            reader.Seek(0);
            var frame = parser.Deserialize(reader);

            Assert.IsNotNull(frame);
        }
    }
}
