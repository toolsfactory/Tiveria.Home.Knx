using NUnit.Framework;
using Tiveria.Home.Knx.IP;

namespace Tiveria.Home.Knx.Tests
{
    [TestFixture]
    class KnxHeaderVersionTests
    {

        static object[] TestDataCheckDefaults =
        {
            new object[] { KnxNetIPVersion.DefaultVersion, 0x10, 6 },
            new object[] { KnxNetIPVersion.Version10, 0x10, 6 },
            new object[] { KnxNetIPVersion.Version20, 0x20, 6 },
        };

        [TestCaseSource(nameof(TestDataCheckDefaults))]
        [Test]
        public void CheckDefaults(KnxNetIPVersion ver, int identifier, int len)
        {
            Assert.AreEqual(identifier, ver.Identifier);
            Assert.AreEqual(len, ver.HeaderLength);
        }

        [Test]
        public void BuildCustom()
        {
            var header = new KnxNetIPVersion("demo", 0x03, 8);
            Assert.AreEqual(0x03, header.Identifier);
            Assert.AreEqual(8, header.HeaderLength);
            Assert.AreEqual("demo", header.Name);
        }

        [Test]
        public void CheckSupportedVersions()
        {
            Assert.IsTrue(KnxNetIPVersion.SupportedVersions.Contains(KnxNetIPVersion.Version10));
            Assert.IsTrue(KnxNetIPVersion.SupportedVersions.Contains(KnxNetIPVersion.Version20));
            Assert.AreEqual(2, KnxNetIPVersion.SupportedVersions.Length);
        }

        static object[] TestDataIsSupportedVersion =
        {
            new object[] { KnxNetIPVersion.DefaultVersion, true },
            new object[] { KnxNetIPVersion.Version10, true },
            new object[] { KnxNetIPVersion.Version20, true },
            new object[] { new KnxNetIPVersion("x", 0x10, 7), false },
            new object[] { new KnxNetIPVersion("x", 0x20, 7), false },
            new object[] { new KnxNetIPVersion("x", 0x03, 6), false },
            new object[] { new KnxNetIPVersion("x", 0x03, 7), false },
            new object[] { new KnxNetIPVersion("x", 0x00, 6), false },
            new object[] { new KnxNetIPVersion("x", 0x04, 6), false },
            new object[] { new KnxNetIPVersion("x", 0x07, 6), false },
        };

        [TestCaseSource(nameof(TestDataIsSupportedVersion))]
        [Test]
        public void CheckIsSupportedVersion(KnxNetIPVersion ver, bool supported)
        {
            Assert.AreEqual(supported, KnxNetIPVersion.IsSupportedVersion(ver));
        }

    }
}
