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
        static object[] TestDataCompressed =
        {
            new object[] { ApciTypes.GroupValue_Response, new byte[] { 0x00 }, new byte[] { 0x00, 0x40 } },
            new object[] { ApciTypes.GroupValue_Response, new byte[] { 0x01 }, new byte[] { 0x00, 0x41 } },
            new object[] { ApciTypes.GroupValue_Response, new byte[] { 0x3f }, new byte[] { 0x00, 0x7f } },
        };

        [TestCaseSource(nameof(TestDataCompressed))]
        public void BuildApciCompressed_Many(int type, byte[] data, byte[] raw)
        {
            var result = new Apci(type, data);
            var resultRaw = result.ToBytes();
            Assert.AreEqual(type, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, resultRaw.Length);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual(raw[0], resultRaw[0]);
            Assert.AreEqual(raw[1], resultRaw[1]);
        }

        static object[] TestData =
{
            new object[] { ApciTypes.GroupValue_Read, new byte[] {  }, new byte[] { 0x00, 0x00 } },
            new object[] { ApciTypes.GroupValue_Response, new byte[] { 0x40 }, new byte[] { 0x00, 0x40, 0x40 } },
            new object[] { ApciTypes.GroupValue_Response, new byte[] { 0xff }, new byte[] { 0x00, 0x40, 0xff } },
            new object[] { ApciTypes.GroupValue_Response, new byte[] { 0xaa, 0xbb }, new byte[] { 0x00, 0x40, 0xaa, 0xbb } },
        };

        [TestCaseSource(nameof(TestData))]
        public void BuildApci_Many(int type, byte[] data, byte[] raw)
        {
            var result = new Apci(type, data);
            var resultRaw = result.ToBytes();
            Assert.AreEqual(type, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(raw.Length, resultRaw.Length);
            Assert.AreEqual(data.Length, result.Data.Length);
            Assert.AreEqual(raw[0], resultRaw[0]);
            Assert.AreEqual(raw[1], resultRaw[1]);
            if (raw.Length > 2) Assert.AreEqual(raw[2], resultRaw[2]);
            if (raw.Length > 3) Assert.AreEqual(raw[3], resultRaw[3]);
            if (raw.Length > 4) Assert.AreEqual(raw[4], resultRaw[4]);
            if (raw.Length > 5) Assert.AreEqual(raw[5], resultRaw[5]);
            if (raw.Length > 6) Assert.AreEqual(raw[6], resultRaw[6]);
        }

        /* At the moment no check for data size implemented
        [Test]
        public void ParseGroupValue_Read1()
        {
            var data = "0000".ToByteArray(); // Type:GroupValue_Read, Data:None
            var result = new Apci(ApciTypes.GroupValue_Read, data);

            Assert.AreEqual(ApciTypes.GroupValue_Read, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.Zero(result.Data.Length);
        }
        */

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
    }
}
