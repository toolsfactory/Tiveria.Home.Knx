using System;
using System.Net;
using NUnit.Framework;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Common.Extensions;
using Tiveria.Common.IO;

namespace Tiveria.Home.Knx.Tests
{
    [TestFixture]
    class AcpiTests
    {
        [Test]
        public void ParseGroupValue_Read1()
        {
            var data = "0000".ToByteArray(); // Type:GroupValue_Read, Data:None
            Assert.Throws<ArgumentException>(() =>new Apci(ApciTypes.GroupValue_Read, data));
        }

        [Test]
        public void ParseGroupValue_Write1()
        {
            var data = "00800c56".ToByteArray(); // Type:GroupValue_Write, Data:0c56
            var result = Apci.Parse(data);

            Assert.AreEqual(ApciTypes.GroupValue_Write, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, result.Data.Length);
            Assert.AreEqual(0x0c, result.Data[0]);
            Assert.AreEqual(0x56, result.Data[1]);
        }

        [Test]
        public void ParseGroupValue_Write2()
        {
            var data = "00800d".ToByteArray(); // Type:GroupValue_Write, Data:0d
            var result = Apci.Parse(data);

            Assert.AreEqual(ApciTypes.GroupValue_Write, result.Type);
            Assert.AreEqual(3, result.Size);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual(0x0d, result.Data[0]);
        }

        [Test]
        public void ParseGroupValue_Response1()
        {
            var data = "00400abc".ToByteArray(); // Type:GroupValue_Response, Data:0abc
            var result = Apci.Parse(data);

            Assert.AreEqual(ApciTypes.GroupValue_Response, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, result.Data.Length);
            Assert.AreEqual(0x0a, result.Data[0]);
            Assert.AreEqual(0xbc, result.Data[1]);
        }

        [Test]
        public void ParseGroupValue_Response2()
        {
            var data = "00404d".ToByteArray(); // Type:GroupValue_Response, Data:4d
            var result = Apci.Parse(data);

            Assert.AreEqual(ApciTypes.GroupValue_Response, result.Type);
            Assert.AreEqual(3, result.Size);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual(0x4d, result.Data[0]);
        }

        [Test]
        public void ParseGroupValue_Response3()
        {
            var data = "0041".ToByteArray(); // Type:GroupValue_Response, Data: low 6 bits = 0b000001 = 0x01
            var result = Apci.Parse(data);

            Assert.AreEqual(ApciTypes.GroupValue_Response, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual(0x01, result.Data[0]);
        }


        [Test]
        public void ParseGroupValue_Response4()
        {
            var data = "006A".ToByteArray(); // Type:GroupValue_Response, Data: low 6 bits = 0b101010 = 0x2a
            var result = Apci.Parse(data);

            Assert.AreEqual(ApciTypes.GroupValue_Response, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual(0x2a, result.Data[0]);
        }

        [Test]
        public void ParseGroupValue_Response5()
        {
            var data = "00-40-00".Replace("-", "").ToByteArray();
            var result = Apci.Parse(data);
            var buffer = result.ToBytes();

            Assert.AreEqual(3, buffer.Length);
            Assert.AreEqual(ApciTypes.GroupValue_Response, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual(0x00, result.Data[0]);
        }

        [Test]
        public void ParseGroupValue_Write3()
        {
            var data = "00-80-3D-30-20-C5".Replace("-", null).ToByteArray(); // Type:GroupValue_Response, Data: low 6 bits = 0b101010 = 0x2a
            var result = Apci.Parse(data);

            Assert.AreEqual(ApciTypes.GroupValue_Write, result.Type);
            Assert.AreEqual(6, result.Size);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(4, result.Data.Length);
            Assert.AreEqual(0x3d, result.Data[0]);
        }

        [Test]
        public void BuildGroupValueRead()
        {
            var tpdu = new Apci(ApciTypes.GroupValue_Write, new byte[] { 0x01 });
            Assert.AreEqual(new byte [] { 0, 129 }, tpdu.ToBytes());
        }
    }
}
