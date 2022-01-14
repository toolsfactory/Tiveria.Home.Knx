using NUnit.Framework;
using System.Net;
using Tiveria.Common.Extensions;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Common.IO;

namespace Tiveria.Home.Knx.Tests.IP
{
    [TestFixture]
    public class StructureTests
    {
        [Test]
        public void HPAI_Constructor_Ok()
        {
            var expected = "0801c0a80278e1f6".ToByteArray(); // Type: IPV4 UDP, IP: 192.168.2.120, Port: 57846
            var hpai = new Hpai(HPAIEndpointType.IPV4_UDP, IPAddress.Parse("192.168.2.120"), 57846);
            Assert.AreEqual(expected, hpai.ToBytes());
        }

        [Test]
        public void HPAI_FromBytes_Ok()
        {
            var data = "0801c0a80278e1f6".ToByteArray(); // Type: IPV4 UDP, IP: 192.168.2.120, Port: 57846
            var reader = new BigEndianBinaryReader(new MemoryStream(data));
            var hpai = Hpai.Parse(reader);
            Assert.AreEqual(hpai.Ip.ToString(), "192.168.2.120");
            Assert.IsTrue(hpai.Port == 57846);
            Assert.IsTrue(hpai.EndpointType == HPAIEndpointType.IPV4_UDP);
        }

        [Test]
        public void HPAI_FromBytes_WrongEndpointTypeByte()
        {
            var data = "0822c0a80278e1f6".ToByteArray(); // EnpointType byte set to 0x22
            var reader = new BigEndianBinaryReader(new MemoryStream(data));
            Assert.Catch<BufferFieldException>(() => Hpai.Parse(reader));
        }

        [Test]
        public void HPAI_FromBytes_WrongSizeByte()
        {
            var data = "0701c0a80278e1f6".ToByteArray(); // Size set to 0x07 (first byte)
            var reader = new BigEndianBinaryReader(new MemoryStream(data));
            Assert.Catch<BufferFieldException>(() => Hpai.Parse(reader));
        }

        [Test]
        public void HPAI_FromBytes_WrongBufferSize()
        {
            var data = "0801c0a80278e1".ToByteArray(); // Buffer one byte too short
            var reader = new BigEndianBinaryReader(new MemoryStream(data));
            Assert.Catch<EndOfStreamException>(() => Hpai.Parse(reader));
        }

        [Test]
        public void CRI_Constructor1_Ok()
        {
            var cri = new CRI(ConnectionType.Tunnel);
            Assert.IsTrue(cri.ConnectionType == ConnectionType.Tunnel);
            Assert.IsTrue(cri.Size == 2);
            var data = cri.ToBytes();
            Assert.IsTrue(data.Length == 2);
            Assert.IsTrue(data[0] == 2);
            Assert.IsTrue(data[1] == (byte)ConnectionType.Tunnel);
        }

        [Test]
        public void CRI_Constructor2_Ok()
        {
            var opt = new byte[4] { 1, 2, 3, 4 };
            var cri = new CRI(ConnectionType.Tunnel, opt);
            Assert.IsTrue(cri.ConnectionType == ConnectionType.Tunnel);
            Assert.IsTrue(cri.Size == 6);
            var data = cri.ToBytes();
            Assert.IsTrue(data.Length == 6);
            Assert.IsTrue(data[0] == 6);
            Assert.IsTrue(data[1] == (byte)ConnectionType.Tunnel);
            Assert.IsTrue(data[2] == 1);
            Assert.IsTrue(data[3] == 2);
            Assert.IsTrue(data[4] == 3);
            Assert.IsTrue(data[5] == 4);
        }

        [Test]
        public void CRI_FromBytes_Ok()
        {
            var source = "04060200".ToByteArray(); // Size: 4, Type: Remlog Connection, Layer: Link
            var reader = new BigEndianBinaryReader(new MemoryStream(source));
            var cri = CRI.Parse(reader);
            Assert.IsTrue(cri.Size == 4);
            Assert.IsTrue(cri.ConnectionType == ConnectionType.RemLog);
            var data = cri.ToBytes();
            Assert.IsTrue(data.Length == 4);
            Assert.IsTrue(data[0] == 4);
            Assert.IsTrue(data[1] == (byte)ConnectionType.RemLog);
            Assert.IsTrue(data[2] == 2);
            Assert.IsTrue(data[3] == 0);
        }

        [Test]
        public void CRI_FromBytes_WrongConnectionTypeByte()
        {
            var buffer = "04ff0200".ToByteArray(); // Size: 4, Type byte set to 0xff
            var reader = new BigEndianBinaryReader(new MemoryStream(buffer));
            Assert.Catch<BufferFieldException>(() => CRI.Parse(reader));
        }
    }
}
