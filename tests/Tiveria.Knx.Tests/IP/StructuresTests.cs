using NUnit.Framework;
using System.Net;
using Tiveria.Common.Extensions;
using Tiveria.Knx.Exceptions;
using Tiveria.Knx.IP.Structures;
using Tiveria.Knx.IP.Utils;

namespace Tiveria.Knx.Tests.IP
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
            var buffer = "0801c0a80278e1f6".ToByteArray(); // Type: IPV4 UDP, IP: 192.168.2.120, Port: 57846
            var hpai = Hpai.FromBuffer(buffer, 0);
            Assert.AreEqual(hpai.Ip.ToString(), "192.168.2.120");
            Assert.IsTrue(hpai.Port == 57846);
            Assert.IsTrue(hpai.EndpointType == HPAIEndpointType.IPV4_UDP);
        }

        [Test]
        public void HPAI_FromBytes_WrongEndpointTypeByte()
        {
            var buffer = "0822c0a80278e1f6".ToByteArray(); // EnpointType byte set to 0x22
            Assert.Catch<BufferFieldException>(() => Hpai.FromBuffer(buffer, 0));
        }

        [Test]
        public void HPAI_FromBytes_WrongSizeByte()
        {
            var buffer = "0701c0a80278e1f6".ToByteArray(); // Size set to 0x07 (first byte)
            Assert.Catch<BufferFieldException>(() => Hpai.FromBuffer(buffer, 0));
        }

        [Test]
        public void HPAI_FromBytes_WrongBufferSize()
        {
            var buffer = "0801c0a80278e1".ToByteArray(); // Buffer one byte too short
            Assert.Catch<BufferException>(() => Hpai.FromBuffer(buffer, 0));
        }

        [Test]
        public void CRI_Constructor1_Ok()
        {
            var cri = new CRI(ConnectionType.TUNNEL_CONNECTION);
            Assert.IsTrue(cri.ConnectionType == ConnectionType.TUNNEL_CONNECTION);
            Assert.IsTrue(cri.StructureLength == 2);
            var data = cri.ToBytes();
            Assert.IsTrue(data.Length == 2);
            Assert.IsTrue(data[0] == 2);
            Assert.IsTrue(data[1] == (byte) ConnectionType.TUNNEL_CONNECTION);
        }

        [Test]
        public void CRI_Constructor2_Ok()
        {
            var opt = new byte[4] { 1, 2, 3, 4 };
            var cri = new CRI(ConnectionType.TUNNEL_CONNECTION, opt);
            Assert.IsTrue(cri.ConnectionType == ConnectionType.TUNNEL_CONNECTION);
            Assert.IsTrue(cri.StructureLength == 6);
            var data = cri.ToBytes();
            Assert.IsTrue(data.Length == 6);
            Assert.IsTrue(data[0] == 6);
            Assert.IsTrue(data[1] == (byte)ConnectionType.TUNNEL_CONNECTION);
            Assert.IsTrue(data[2] == 1);
            Assert.IsTrue(data[3] == 2);
            Assert.IsTrue(data[4] == 3);
            Assert.IsTrue(data[5] == 4);
        }

        [Test]
        public void CRI_FromBytes_Ok()
        {
            var buffer = "04060200".ToByteArray(); // Size: 4, Type: Remlog Connection, Layer: Link
            var cri = CRI.FromBuffer(buffer, 0);
            Assert.IsTrue(cri.StructureLength == 4);
            Assert.IsTrue(cri.ConnectionType == ConnectionType.REMLOG_CONNECTION);
            var data = cri.ToBytes();
            Assert.IsTrue(data.Length == 4);
            Assert.IsTrue(data[0] == 4);
            Assert.IsTrue(data[1] == (byte)ConnectionType.REMLOG_CONNECTION);
            Assert.IsTrue(data[2] == 2);
            Assert.IsTrue(data[3] == 0);
        }

        [Test]
        public void CRI_FromBytes_WrongConnectionTypeByte()
        {
            var buffer = "04ff0200".ToByteArray(); // Size: 4, Type byte set to 0xff
            Assert.Catch<BufferFieldException>(() => CRI.FromBuffer(buffer, 0));
        }
    }
}
