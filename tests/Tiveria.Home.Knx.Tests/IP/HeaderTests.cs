using System;
using System.Net;
using NUnit.Framework;
using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Common.Extensions;
using Tiveria.Common.IO;

namespace Tiveria.Home.Knx.Tests.IP
{
    [TestFixture]
    public class HeaderTests
    {
        [Test]
        public void ParseHeaderFromBytesOK()
        {
            var data = "0610020500180801c0a80278e1f60801c0a80278e1f70203".ToByteArray(); //Standard body for Connection_Request

            var reader = new BigEndianBinaryReader(new MemoryStream(data));
            var result = FrameHeader.Parse(reader);
            Assert.AreEqual(result.Size, KnxNetIPVersion.Version10.HeaderLength);
            Assert.AreEqual(result.Version, KnxNetIPVersion.Version10.Identifier);
            Assert.AreEqual(result.ServiceTypeIdentifier, ServiceTypeIdentifier.CONNECT_REQUEST);
            Assert.AreEqual(result.TotalLength, 18 + KnxNetIPVersion.Version10.HeaderLength);
        }

        [Test]
        public void ParseHeaderFromBytesWrongHeaderLength()
        {
            //Standard body for Connection_Request but length set to 8
            var data = "0810020500180801c0a80278e1f60801c0a80278e1f70203".ToByteArray();

            var reader = new BigEndianBinaryReader(new MemoryStream(data));
            Assert.Catch(typeof(BufferFieldException), () => FrameHeader.Parse(reader));
        }

        [Test]
        public void ParseHeaderFromBytesWrongHeaderVersion()
        {
            //Standard body for Connection_Request but Version changed from 0x10 to 0x20
            var data = "0620020500180801c0a80278e1f60801c0a80278e1f70203".ToByteArray();
            var reader = new BigEndianBinaryReader(new MemoryStream(data));
            Assert.Catch(typeof(BufferFieldException), () => FrameHeader.Parse(reader));
        }

        [Test]
        public void ParseHeaderFromBytesWrongTotalLength()
        {
            //Standard body for Connection_Request but totallength set wrong 0x20 instead of 0x18
            var data = "0610020500200801c0a80278e1f60801c0a80278e1f70203".ToByteArray();
            var reader = new BigEndianBinaryReader(new MemoryStream(data));
            Assert.Catch(typeof(BufferSizeException), () => FrameHeader.Parse(reader));
        }

        [Test]
        public void ParseHeaderFromBytesUnknownServiceTypeIdentifier()
        {
            //Standard body for Connection_Request but ServiceTypeIdentifier set to 0x00ff
            var data = "061000ff00200801c0a80278e1f60801c0a80278e1f70203".ToByteArray();
            var reader = new BigEndianBinaryReader(new MemoryStream(data));
            var result = FrameHeader.Parse(reader);
            Assert.AreEqual(ServiceTypeIdentifier.UNKNOWN, result.ServiceTypeIdentifier);
            Assert.AreEqual(0x00ff, result.ServiceTypeIdentifierRaw);
        }

        [Test]
        public void CreateHeaderOk()
        {
            var result = new FrameHeader(ServiceTypeIdentifier.CONNECT_REQUEST, 18);

            Assert.AreEqual(result.Size, KnxNetIPVersion.Version10.HeaderLength);
            Assert.AreEqual(result.Version, KnxNetIPVersion.Version10.Identifier);
            Assert.AreEqual(result.ServiceTypeIdentifier, ServiceTypeIdentifier.CONNECT_REQUEST);
            Assert.AreEqual(result.TotalLength, 18 + KnxNetIPVersion.Version10.HeaderLength);
        }
    }

}
