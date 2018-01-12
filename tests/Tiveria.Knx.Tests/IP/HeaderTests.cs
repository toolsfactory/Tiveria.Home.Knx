using System;
using System.Net;
using NUnit.Framework;
using Tiveria.Knx.IP.Structures;
using Tiveria.Knx.IP.Utils;
using Tiveria.Common.Extensions;

namespace Tiverias.Knx.Tests.IP
{
    [TestFixture]
    public class HeaderTests
    {
        [Test]
        public void ParseHeaderFromBytesOK()
        {
            var data = "0610020500180801c0a80278e1f60801c0a80278e1f70203".ToByteArray(); //Standard body for Connection_Request

            var result = Header.FromBuffer(ref data, 0);
            Assert.AreEqual(result.Size, Header.HEADER_SIZE_10);
            Assert.AreEqual(result.Version, Header.KNXNETIP_VERSION_10);
            Assert.AreEqual(result.ServiceTypeIdentifier, ServiceTypeIdentifier.CONNECT_REQUEST);
            Assert.AreEqual(result.TotalLength, 18 + Header.HEADER_SIZE_10);
        }

        [Test]
        public void ParseHeaderFromBytesWrongHeaderLength()
        {
            //Standard body for Connection_Request but length set to 8
            var data = "0810020500180801c0a80278e1f60801c0a80278e1f70203".ToByteArray(); 
            Assert.Catch(typeof(ArgumentException), () => Header.FromBuffer(ref data, 0));
        }

        [Test]
        public void ParseHeaderFromBytesWrongHeaderVersion()
        {
            //Standard body for Connection_Request but Version changed from 0x10 to 0x20
            var data = "0820020500180801c0a80278e1f60801c0a80278e1f70203".ToByteArray(); 
            Assert.Catch(typeof(ArgumentException), () => Header.FromBuffer(ref data, 0));
        }

        [Test]
        public void ParseHeaderFromBytesWrongTotalLength()
        {
            //Standard body for Connection_Request but totallength set wrong 0x20 instead of 0x18
            var data = "0610020500200801c0a80278e1f60801c0a80278e1f70203".ToByteArray(); 
            Assert.Catch(typeof(ArgumentException), () => Header.FromBuffer(ref data, 0));
        }

        [Test]
        public void ParseHeaderFromBytesWrongServiceTypeIdentifier()
        {
            //Standard body for Connection_Request but ServiceTypeIdentifier set to 0x00ff
            var data = "061000ff00200801c0a80278e1f60801c0a80278e1f70203".ToByteArray();
            Assert.Catch(typeof(ArgumentException), () => Header.FromBuffer(ref data, 0));
        }

        [Test]
        public void CreateHeaderOk()
        {
            var result = new Header(ServiceTypeIdentifier.CONNECT_REQUEST, 18);

            Assert.AreEqual(result.Size, Header.HEADER_SIZE_10);
            Assert.AreEqual(result.Version, Header.KNXNETIP_VERSION_10);
            Assert.AreEqual(result.ServiceTypeIdentifier, ServiceTypeIdentifier.CONNECT_REQUEST);
            Assert.AreEqual(result.TotalLength, 18 + Header.HEADER_SIZE_10);
        }
    }
}
