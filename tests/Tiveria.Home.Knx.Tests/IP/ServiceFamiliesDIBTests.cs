using NUnit.Framework;
using System.Net;
using Tiveria.Common.Extensions;
using Tiveria.Home.Knx.IP.Structures;
using Tiveria.Home.Knx.IP.Enums;
using System.Collections.ObjectModel;
using Tiveria.Common.IO;

namespace Tiveria.Home.Knx.Tests.IP
{
    [TestFixture]
    public class ServiceFamiliesDIBTests
    {
        [Test]
        public void Construct_Structure_01()
        {
            var servicefamilies = new Collection<(byte Family, byte Version)>();
            servicefamilies.Add((0x02, 0x01));
            servicefamilies.Add((0x03, 0x01));
            var expected = "060202010301".ToByteArray();
            var dib = new ServiceFamiliesDIB(servicefamilies);

            Assert.AreEqual(dib.Size, 6);
            Assert.AreEqual(dib.ServiceFamilies.Count, 2);
            Assert.AreEqual(dib.ServiceFamilies[0], servicefamilies[0]);
            Assert.AreEqual(dib.ServiceFamilies[1], servicefamilies[1]);
            Assert.AreEqual(dib.ToBytes(), expected);
        }

        [Test]
        public void Construct_Structure_02()
        {
            var servicefamilies = new Collection<(byte Family, byte Version)>();
            servicefamilies.Add((0x02, 0x01));
            servicefamilies.Add((0x03, 0x01));
            servicefamilies.Add((0x04, 0x01));
            var expected = "0602020103010401".ToByteArray();
            var dib = new ServiceFamiliesDIB(servicefamilies);

            Assert.AreEqual(dib.Size, 8);
            Assert.AreEqual(dib.ServiceFamilies.Count, 3);
            Assert.AreEqual(dib.ServiceFamilies[0], servicefamilies[0]);
            Assert.AreEqual(dib.ServiceFamilies[1], servicefamilies[1]);
            Assert.AreEqual(dib.ServiceFamilies[2], servicefamilies[2]);
            Assert.AreEqual(dib.ToBytes(), expected);
        }

        [Test]
        public void Parse_Structure_01()
        {
            var data = "0602020103010401".ToByteArray();
            var reader = new BigEndianBinaryReader(new MemoryStream(data));
            var dib = ServiceFamiliesDIB.Parse(reader);

            Assert.AreEqual(dib.Size, 8);
            Assert.AreEqual(dib.ServiceFamilies.Count, 3);
            Assert.AreEqual(dib.ServiceFamilies[0], (0x02, 0x01));
            Assert.AreEqual(dib.ServiceFamilies[1], (0x03, 0x01));
            Assert.AreEqual(dib.ServiceFamilies[2], (0x04, 0x01));
            Assert.AreEqual(dib.ToBytes(), data);
        }
    }
}
