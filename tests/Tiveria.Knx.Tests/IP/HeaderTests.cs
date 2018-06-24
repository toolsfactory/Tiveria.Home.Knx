using System;
using System.Net;
using NUnit.Framework;
using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Common.Extensions;

namespace Tiveria.Home.Knx.Tests.IP
{
    [TestFixture]
    public class HeaderTests
    {
        [Test]
        public void ParseHeaderFromBytesOK()
        {
            var data = "0610020500180801c0a80278e1f60801c0a80278e1f70203".ToByteArray(); //Standard body for Connection_Request

            var result = FrameHeader.Parse(data, 0);
            Assert.AreEqual(result.Size, FrameHeader.HEADER_SIZE_10);
            Assert.AreEqual(result.Version, FrameHeader.KNXNETIP_VERSION_10);
            Assert.AreEqual(result.ServiceTypeIdentifier, ServiceTypeIdentifier.CONNECT_REQUEST);
            Assert.AreEqual(result.TotalLength, 18 + FrameHeader.HEADER_SIZE_10);
        }

        [Test]
        public void ParseHeaderFromBytesWrongHeaderLength()
        {
            //Standard body for Connection_Request but length set to 8
            var data = "0810020500180801c0a80278e1f60801c0a80278e1f70203".ToByteArray(); 
            Assert.Catch(typeof(BufferFieldException), () => FrameHeader.Parse(data, 0));
        }

        [Test]
        public void ParseHeaderFromBytesWrongHeaderVersion()
        {
            //Standard body for Connection_Request but Version changed from 0x10 to 0x20
            var data = "0620020500180801c0a80278e1f60801c0a80278e1f70203".ToByteArray(); 
            Assert.Catch(typeof(BufferFieldException), () => FrameHeader.Parse(data, 0));
        }

        [Test]
        public void ParseHeaderFromBytesWrongTotalLength()
        {
            //Standard body for Connection_Request but totallength set wrong 0x20 instead of 0x18
            var data = "0610020500200801c0a80278e1f60801c0a80278e1f70203".ToByteArray(); 
            Assert.Catch(typeof(BufferSizeException), () => FrameHeader.Parse(data, 0));
        }

        [Test]
        public void ParseHeaderFromBytesUnknownServiceTypeIdentifier()
        {
            //Standard body for Connection_Request but ServiceTypeIdentifier set to 0x00ff
            var data = "061000ff00200801c0a80278e1f60801c0a80278e1f70203".ToByteArray();
            var result = FrameHeader.Parse(data, 0);
            Assert.AreEqual(ServiceTypeIdentifier.UNKNOWN, result.ServiceTypeIdentifier);
            Assert.AreEqual(0x00ff, result.ServiceTypeRaw);
        }

        [Test]
        public void CreateHeaderOk()
        {
            var result = new FrameHeader(ServiceTypeIdentifier.CONNECT_REQUEST, 18);

            Assert.AreEqual(result.Size, FrameHeader.HEADER_SIZE_10);
            Assert.AreEqual(result.Version, FrameHeader.KNXNETIP_VERSION_10);
            Assert.AreEqual(result.ServiceTypeIdentifier, ServiceTypeIdentifier.CONNECT_REQUEST);
            Assert.AreEqual(result.TotalLength, 18 + FrameHeader.HEADER_SIZE_10);
        }
    }
}
