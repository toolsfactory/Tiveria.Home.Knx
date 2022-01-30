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
        #region data creation helpers
        private static byte[] GetRandomData(int len)
        {
            byte[] data = new byte[len];
            var rnd = new Random();
            rnd.NextBytes(data);
            return data;
        }
        private static byte[] GetSequenceData(int len)
        {
            byte[] data = new byte[len];
            for(var i= 0; i < len; i++)
                data[i] = (byte)i;
            return data;
        }
        #endregion



        #region testing Acpi types without data
        static object[] TestDataApciNoData =
        {
            new object[] { ApciType.GroupValue_Read,                new byte[] { 0b00000000, 0b00000000 } },
            new object[] { ApciType.IndividualAddress_Read,         new byte[] { 0b00000001, 0b00000000 } },
            new object[] { ApciType.IndividualAddress_Response,     new byte[] { 0b00000001, 0b01000000 } },
            new object[] { ApciType.UserManufacturerInfo_Read,      new byte[] { 0b00000010, 0b11000101 } },
            new object[] { ApciType.Restart_Request,                new byte[] { 0b00000011, 0b10000000 } },
            new object[] { ApciType.Open_Routing_Table_Request,     new byte[] { 0b00000011, 0b11000000 } },
            new object[] { ApciType.Read_Routing_Table_Request,     new byte[] { 0b00000011, 0b11000001 } },
            new object[] { ApciType.Read_Routing_Table_Response,    new byte[] { 0b00000011, 0b11000010 } },
            new object[] { ApciType.Write_Routing_Table_Request,    new byte[] { 0b00000011, 0b11000011 } },
            new object[] { ApciType.Read_Router_Memory_Request,     new byte[] { 0b00000011, 0b11001000 } },
            new object[] { ApciType.Read_Router_Memory_Response,    new byte[] { 0b00000011, 0b11001001 } },
            new object[] { ApciType.Write_Router_Memory_Request,    new byte[] { 0b00000011, 0b11001010 } },
            new object[] { ApciType.Read_Router_Status_Request,     new byte[] { 0b00000011, 0b11001101 } },
            new object[] { ApciType.Read_Router_Status_Response,    new byte[] { 0b00000011, 0b11001110 } },
            new object[] { ApciType.Write_Router_Status_Request,    new byte[] { 0b00000011, 0b11001111 } },
            new object[] { ApciType.DomainAddress_Read,             new byte[] { 0b00000011, 0b11100001 } },
            new object[] { ApciType.GroupPropertyValue_Read,        new byte[] { 0b00000011, 0b11101000 } },
            new object[] { ApciType.GroupPropertyValue_Response,    new byte[] { 0b00000011, 0b11101001 } },
            new object[] { ApciType.GroupPropertyValue_Write,       new byte[] { 0b00000011, 0b11101010 } },
            new object[] { ApciType.GroupPropertyValue_InfoReport,  new byte[] { 0b00000011, 0b11101011 } },
        };
        [TestCaseSource(nameof(TestDataApciNoData))]
        public void Build_NoData_Ok(int type, byte[] raw)
        {
            var result = new Apdu(type);
            var resultRaw = result.ToBytes();
            Assert.AreEqual(type, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(0, result.Data.Length);
            Assert.AreEqual(2, result.Size);
            Assert.AreEqual(raw.Length, resultRaw.Length);
            Assert.AreEqual(raw[0], resultRaw[0]);
            Assert.AreEqual(raw[1], resultRaw[1]);
        }

        [TestCaseSource(nameof(TestDataApciNoData))]
        public void Build_NoData_DataParameterNotEmpty(int type, byte[] raw)
        {
            Assert.Throws<ArgumentException>(() => new Apdu(type, new byte[1]));
        }
        #endregion

        #region testing Apci types with exact data length
        static object[] TestDataExactSize =
        {
            new object[] { ApciType.IndividualAddress_Write,                 2 },
            new object[] { ApciType.UserMemory_Read,                         3 },
            new object[] { ApciType.UserManufacturerInfo_Response,           3 },
            new object[] { ApciType.RestartMasterReset_Request,              2 },
            new object[] { ApciType.RestartMasterReset_Response,             3 },
            new object[] { ApciType.Authorize_Request,                       5 },
            new object[] { ApciType.Authorize_Response,                      1 },
            new object[] { ApciType.Key_Write,                               5 },
            new object[] { ApciType.Key_Response,                            1 },
            new object[] { ApciType.PropertyValue_Read,                      4 },
            new object[] { ApciType.PropertyDescription_Read,                3 },
            new object[] { ApciType.PropertyDescription_Response,            7 },
            new object[] { ApciType.IndividualAddressSerialNumber_Read,      6 },
            new object[] { ApciType.IndividualAddressSerialNumber_Response, 10 },
            new object[] { ApciType.IndividualAddressSerialNumber_Write,    12 },
            new object[] { ApciType.Link_Read,                               2 },
            new object[] { ApciType.Link_Write,                              4 },
            new object[] { ApciType.DomainAddressSerialNumber_Read,          6 },
            new object[] { ApciType.DomainAddressSerialNumber_Response,     10 },
            new object[] { ApciType.DomainAddressSerialNumber_Write,        12 },
        };

        [TestCaseSource(nameof(TestDataExactSize))]
        public void Build_ExactData_Ok(int type, int len)
        {
            var result = new Apdu(type, GetRandomData(len));
            var resultRaw = result.ToBytes();
            Assert.AreEqual(type, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(len, result.Data.Length);
            Assert.AreEqual(len + 2, result.Size);
        }

        [TestCaseSource(nameof(TestDataExactSize))]
        public void Build_ExactData_WrongDataSize(int type, int len)
        {
            Assert.Throws<ArgumentException>(() => new Apdu(type, GetRandomData(len - 1)));
            Assert.Throws<ArgumentException>(() => new Apdu(type, GetRandomData(len + 1)));
        }
        #endregion

        #region testing Apci types with at least a minimal size
        static object[] TestDataMinimalSize =
        {
            new object[] { ApciType.SystemNetworkParameter_Read,  4},
            new object[] { ApciType.SystemNetworkParameter_Response,  6},
            new object[] { ApciType.SystemNetworkParameter_Write,  4},
            new object[] { ApciType.UserMemory_Response,  3},
            new object[] { ApciType.UserMemory_Write,  3},
            new object[] { ApciType.UserMemoryBit_Write,  5},
            new object[] { ApciType.FunctionPropertyCommand,  3},
            new object[] { ApciType.FunctionPropertyState_Read,  3},
            new object[] { ApciType.FunctionPropertyState_Response,  3},
            new object[] { ApciType.MemoryBit_Write,  6},
            new object[] { ApciType.PropertyValue_Response,  4},
            new object[] { ApciType.PropertyValue_Write,  4},
            new object[] { ApciType.NetworkParameter_Read,  4},
            new object[] { ApciType.DomainAddressSelective_Read,  1},
            new object[] { ApciType.NetworkParameter_Write,  4},
            new object[] { ApciType.FileStream_InfoReport,  1},
        };

        [TestCaseSource(nameof(TestDataMinimalSize))]
        public void Build_MinimalData_Ok(int type, int len)
        {
            var result = new Apdu(type, GetRandomData(len));
            var resultRaw = result.ToBytes();
            Assert.AreEqual(type, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(len, result.Data.Length);
            Assert.AreEqual(len + 2, result.Size);
        }

        [TestCaseSource(nameof(TestDataMinimalSize))]
        public void Build_MinimalData_WrongDataSize(int type, int len)
        {
            Assert.Throws<ArgumentException>(() => new Apdu(type, GetRandomData(len - 1)));
        }
        #endregion

        #region testing Apci types with at least a minimal size
        static object[] TestDataMinMaxSize =
        {
            new object[] { ApciType.NetworkParameter_Response,  4, 14},
            new object[] { ApciType.DomainAddress_Write,  2, 6},
            new object[] { ApciType.DomainAddress_Response,  2, 6},
            new object[] { ApciType.Link_Response,  2, 14},
        };

        [TestCaseSource(nameof(TestDataMinMaxSize))]
        public void Build_MinMaxData_Ok(int type, int min, int max)
        {
            for (int i = min; i <= max; i++)
            {
                var result = new Apdu(type, GetRandomData(i));
                var resultRaw = result.ToBytes();
                Assert.AreEqual(type, result.Type);
                Assert.IsNotNull(result.Data);
                Assert.AreEqual(i, result.Data.Length);
                Assert.AreEqual(i + 2, result.Size);
            }
        }

        [TestCaseSource(nameof(TestDataMinMaxSize))]
        public void Build_MinMaxData_WrongDataSize(int type, int min, int max)
        {
            Assert.Throws<ArgumentException>(() => new Apdu(type, GetRandomData(min - 1)));
            Assert.Throws<ArgumentException>(() => new Apdu(type, GetRandomData(max + 1)));
        }
        #endregion

        // ToDo: cleanup below this line

        static object[] TestData =
        {
            // compressed ones
            new object[] { ApciType.DeviceDescriptor_Read,                   1 }, //exact
            new object[] { ApciType.DeviceDescriptor_Response,               2 }, //min
            new object[] { ApciType.ADC_Read,                                2 }, //exact
            new object[] { ApciType.ADC_Response,                            4 }, //exact
            new object[] { ApciType.Memory_Read,                             3 }, //exact
            new object[] { ApciType.Memory_Response, 3 }, //min
            new object[] { ApciType.Memory_Write, 4 }, //min
            new object[] { ApciType.GroupValue_Response, DataMode.MinMax, true, 1, 14}, //minmax
            new object[] { ApciType.GroupValue_Write, DataMode.MinMax, true, 1, 14}, //minmax
        };

        static object[] TestDataCompressed =
        {
            new object[] { ApciType.GroupValue_Response, new byte[] { 0x00 }, new byte[] { 0x00, 0x40 } },
            new object[] { ApciType.GroupValue_Response, new byte[] { 0x01 }, new byte[] { 0x00, 0x41 } },
            new object[] { ApciType.GroupValue_Response, new byte[] { 0x3f }, new byte[] { 0x00, 0x7f } },
        };

        [TestCaseSource(nameof(TestDataCompressed))]
        public void BuildApciCompressed_Many(int type, byte[] data, byte[] raw)
        {
            var result = new Apdu(type, data);
            var resultRaw = result.ToBytes();
            Assert.AreEqual(type, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, resultRaw.Length);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual(raw[0], resultRaw[0]);
            Assert.AreEqual(raw[1], resultRaw[1]);
        }

        static object[] TestDataO =
{
            new object[] { ApciType.GroupValue_Read, new byte[] {  }, new byte[] { 0x00, 0x00 } },
            new object[] { ApciType.GroupValue_Response, new byte[] { 0x40 }, new byte[] { 0x00, 0x40, 0x40 } },
            new object[] { ApciType.GroupValue_Response, new byte[] { 0xff }, new byte[] { 0x00, 0x40, 0xff } },
            new object[] { ApciType.GroupValue_Response, new byte[] { 0xaa, 0xbb }, new byte[] { 0x00, 0x40, 0xaa, 0xbb } },
        };

        [TestCaseSource(nameof(TestDataO))]
        public void BuildApci_Many(int type, byte[] data, byte[] raw)
        {
            var result = new Apdu(type, data);
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





        [Test]
        public void ParseGroupValue_Write1()
        {
            var data = "00800c56".ToByteArray(); // Type:GroupValue_Write, Data:0c56
            var result = Apdu.Parse(data);

            Assert.AreEqual(ApciType.GroupValue_Write, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, result.Data.Length);
            Assert.AreEqual(0x0c, result.Data[0]);
            Assert.AreEqual(0x56, result.Data[1]);
        }

        [Test]
        public void ParseGroupValue_Write2()
        {
            var data = "00800d".ToByteArray(); // Type:GroupValue_Write, Data:0d
            var result = Apdu.Parse(data);

            Assert.AreEqual(ApciType.GroupValue_Write, result.Type);
            Assert.AreEqual(3, result.Size);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual(0x0d, result.Data[0]);
        }

        [Test]
        public void ParseGroupValue_Response1()
        {
            var data = "00400abc".ToByteArray(); // Type:GroupValue_Response, Data:0abc
            var result = Apdu.Parse(data);

            Assert.AreEqual(ApciType.GroupValue_Response, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, result.Data.Length);
            Assert.AreEqual(0x0a, result.Data[0]);
            Assert.AreEqual(0xbc, result.Data[1]);
        }

        [Test]
        public void ParseGroupValue_Response2()
        {
            var data = "00404d".ToByteArray(); // Type:GroupValue_Response, Data:4d
            var result = Apdu.Parse(data);

            Assert.AreEqual(ApciType.GroupValue_Response, result.Type);
            Assert.AreEqual(3, result.Size);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual(0x4d, result.Data[0]);
        }

        [Test]
        public void ParseGroupValue_Response3()
        {
            var data = "0041".ToByteArray(); // Type:GroupValue_Response, Data: low 6 bits = 0b000001 = 0x01
            var result = Apdu.Parse(data);

            Assert.AreEqual(ApciType.GroupValue_Response, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual(0x01, result.Data[0]);
        }


        [Test]
        public void ParseGroupValue_Response4()
        {
            var data = "006A".ToByteArray(); // Type:GroupValue_Response, Data: low 6 bits = 0b101010 = 0x2a
            var result = Apdu.Parse(data);

            Assert.AreEqual(ApciType.GroupValue_Response, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual(0x2a, result.Data[0]);
        }

        [Test]
        public void ParseGroupValue_Response5()
        {
            var data = "00-40-00".Replace("-", "").ToByteArray();
            var result = Apdu.Parse(data);
            var buffer = result.ToBytes();

            Assert.AreEqual(3, buffer.Length);
            Assert.AreEqual(ApciType.GroupValue_Response, result.Type);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual(0x00, result.Data[0]);
        }

        [Test]
        public void ParseGroupValue_Write3()
        {
            var data = "00-80-3D-30-20-C5".Replace("-", null).ToByteArray(); // Type:GroupValue_Response, Data: low 6 bits = 0b101010 = 0x2a
            var result = Apdu.Parse(data);

            Assert.AreEqual(ApciType.GroupValue_Write, result.Type);
            Assert.AreEqual(6, result.Size);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(4, result.Data.Length);
            Assert.AreEqual(0x3d, result.Data[0]);
        }
    }
}
