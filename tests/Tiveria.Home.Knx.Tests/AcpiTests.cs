using System;
using System.Net;
using NUnit.Framework;
using Tiveria.Home.Knx.EMI;
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
            var result = new Apci(ApciTypes.GroupValue_Read, data);

            Assert.AreEqual(ApciTypes.GroupValue_Read, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.Zero(result.Data.Length);
        }
        /*
        [Test]
        public void ParseGroupValue_Write1()
        {
            var data = "00800c56".ToByteArray(); // Type:GroupValue_Write, Data:0c56
            var result = new Apci(data, 0);

            Assert.AreEqual(APCIType.GroupValue_Write, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, result.Data.Length);
            Assert.AreEqual(0x0c, result.Data[0]);
            Assert.AreEqual(0x56, result.Data[1]);
        }

        [Test]
        public void ParseGroupValue_Write2()
        {
            var data = "00800d".ToByteArray(); // Type:GroupValue_Write, Data:0d
            var result = new Apci(data, 0);

            Assert.AreEqual(APCIType.GroupValue_Write, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, result.Data.Length);
            Assert.AreEqual(0x00, result.Data[0]);
            Assert.AreEqual(0x0d, result.Data[1]);
        }

        [Test]
        public void ParseGroupValue_Response1()
        {
            var data = "00400abc".ToByteArray(); // Type:GroupValue_Response, Data:0abc
            var result = new Apci(data, 0);

            Assert.AreEqual(APCIType.GroupValue_Response, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, result.Data.Length);
            Assert.AreEqual(0x0a, result.Data[0]);
            Assert.AreEqual(0xbc, result.Data[1]);
        }

        [Test]
        public void ParseGroupValue_Response2()
        {
            var data = "00404d".ToByteArray(); // Type:GroupValue_Response, Data:4d
            var result = new Apci(data, 0);

            Assert.AreEqual(APCIType.GroupValue_Response, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, result.Data.Length);
            Assert.AreEqual(0x00, result.Data[0]);
            Assert.AreEqual(0x4d, result.Data[1]);
        }

        [Test]
        public void ParseGroupValue_Response3()
        {
            var data = "0041".ToByteArray(); // Type:GroupValue_Response, Data: low 6 bits = 0b000001 = 0x01
            var result = new Apci(data, 0);

            Assert.AreEqual(result.Type, APCIType.GroupValue_Response);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(APCIType.GroupValue_Response, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual(0x01, result.Data[0]);
        }

        [Test]
        public void ParseGroupValue_Response4()
        {
            var data = "006A".ToByteArray(); // Type:GroupValue_Response, Data: low 6 bits = 0b101010 = 0x2a
            var result = new Apci(data, 0);

            Assert.AreEqual(APCIType.GroupValue_Response, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual(0x2a, result.Data[0]);
        }
        */
    }
}
