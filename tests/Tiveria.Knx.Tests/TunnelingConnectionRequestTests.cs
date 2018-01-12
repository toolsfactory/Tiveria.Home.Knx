using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tiveria.Knx.IP.ServiceTypes;
using Tiveria.Knx.IP;
using Tiveria.Knx.IP.Utils;
using Tiveria.Common.Extensions;

namespace Tiveria.Knx.Tests
{
    [TestFixture]
    public class TunnelingConnectionRequestTests
    {
        [Test]
        public void TestTunnelingRequestBody()
        {
            var tunnelingrequestbody = new TunnelingConnectionRequest(IPAddress.Parse("192.168.2.120"), 57846, 57847);
            var reference = "0801c0a80278e1f60801c0a80278e1f704040200".ToByteArray();
            var data = tunnelingrequestbody.ToBytes();
            Assert.AreEqual(data, reference);
        }

        [Test]
        public void TestKnxIpHeaderTunnelingConnectionRequest()
        {
            var tunnelingrequestbody = new TunnelingConnectionRequest(IPAddress.Parse("192.168.2.120"), 57846, 57847);
            var ipframe = new KnxNetIPFrame(ServiceTypeIdentifier.CONNECT_REQUEST, tunnelingrequestbody.ToBytes());
            var reference = "06100205001a0801c0a80278e1f60801c0a80278e1f704040200".ToByteArray();
            var data = ipframe.ToBytes();
            Assert.AreEqual(data, reference);
        }
    }
}
