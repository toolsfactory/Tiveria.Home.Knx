using NUnit.Framework;
using Tiveria.Home.Knx.BaseTypes;
using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.IP.Enums;
using Tiveria.Common.Extensions;
using System.Net;

namespace Tiveria.Home.Knx.Tests.IP
{
    [TestFixture]
    public class DeviceInfoDIBTests
    {
        [Test]
        public void Construct_Structure_01()
        {
            var expected = "36010201110000111234567890abe000170c4549426e65744d59484f4d45000000000000000000000000000000000000000000000000".ToByteArray();

            var dib = new DeviceInfoDIB(KnxMediumType.TP1, DeviceStatus.Programming,
                new IndividualAddress(1,1,0), 0x0011, 0x1234567890AB, IPAddress.Parse("224.0.23.12"), 
                new byte[] { 0x45, 0x49, 0x42, 0x6e, 0x65, 0x74 }, "MYHOME");


            Console.WriteLine(BitConverter.ToString(expected));
            Console.WriteLine(BitConverter.ToString(dib.ToBytes()));

            Assert.AreEqual(DeviceInfoDIB.DEVICEINFODIB_SIZE, dib.Size);
            Assert.AreEqual(DeviceStatus.Programming, dib.Status);
            Assert.AreEqual(new IndividualAddress(1, 1, 0), dib.IndividualAddress);
            Assert.AreEqual(0x0011, dib.ProjectInstallerId);
            Assert.AreEqual(0x1234567890AB, dib.SerialNumber);
            Assert.AreEqual(IPAddress.Parse("224.0.23.12"), dib.RoutingMulticastIP);
            Assert.AreEqual(new byte[] { 0x45, 0x49, 0x42, 0x6e, 0x65, 0x74 }, dib.MAC);
            Assert.AreEqual("MYHOME", dib.FriendlyName);
            Assert.AreEqual(expected, dib.ToBytes());
        }

        [Test]
        public void Parse_Structure_01()
        {
            var expected = "36010201110000111234567890abe000170c4549426e65744d59484f4d45000000000000000000000000000000000000000000000000".ToByteArray();

            var dib = DeviceInfoDIB.Parse(expected);

            Console.WriteLine(BitConverter.ToString(expected));
            Console.WriteLine(BitConverter.ToString(dib.ToBytes()));

            Assert.AreEqual(DeviceInfoDIB.DEVICEINFODIB_SIZE, dib.Size);
            Assert.AreEqual(DeviceStatus.Programming, dib.Status);
            Assert.AreEqual(new IndividualAddress(1, 1, 0), dib.IndividualAddress);
            Assert.AreEqual(0x0011, dib.ProjectInstallerId);
            Assert.AreEqual(0x1234567890AB, dib.SerialNumber);
            Assert.AreEqual(IPAddress.Parse("224.0.23.12"), dib.RoutingMulticastIP);
            Assert.AreEqual(new byte[] { 0x45, 0x49, 0x42, 0x6e, 0x65, 0x74 }, dib.MAC);
            Assert.AreEqual("MYHOME", dib.FriendlyName);
            Assert.AreEqual(expected, dib.ToBytes());
        }
    }
}
