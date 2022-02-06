using System;
using NUnit.Framework;
using Tiveria.Home.Knx;
using Tiveria.Home.Knx.Primitives;

namespace Tiveria.Home.Knx.Tests
{
    [TestFixture]
    public class AddressTests
    {
        [Test]
        public void IndividualAddress_Constructor1_Ok()
        {
            ushort sampleAddress = 0xff_f1; // 15.15.241
            var individualAddress = new IndividualAddress(sampleAddress);

            Assert.IsTrue(individualAddress.Area == 15);
            Assert.IsTrue(individualAddress.Line == 15);
            Assert.IsTrue(individualAddress.Device == 241);
            Assert.IsTrue(individualAddress.AddressType == AddressType.IndividualAddress);
        }

        [Test]
        public void IndividualAddress_Constructor2_Ok()
        {
            ushort sampleAddress = 0xff_f1; // 15.15.241
            var individualAddress = new IndividualAddress(15, 15, 241);

            Assert.IsTrue(individualAddress.Area == 15);
            Assert.IsTrue(individualAddress.Line == 15);
            Assert.IsTrue(individualAddress.Device == 241);
            Assert.IsTrue(individualAddress.RawAddress == sampleAddress);
            Assert.IsTrue(individualAddress.AddressType == AddressType.IndividualAddress);
        }

        [Test]
        public void IndividualAddress_ToString_Ok()
        {
            var individualAddress = new IndividualAddress(15, 15, 241);
            Assert.AreEqual(individualAddress.ToString(), "15.15.241");
        }

        [Test]
        public void IndividualAddress_ToBytes_Ok()
        {
            var individualAddress = new IndividualAddress(15, 15, 241);
            Assert.AreEqual(individualAddress.ToBytes(), new byte[2] { 0xff, 0xf1 });
        }

        [Test]
        public void IndividualAddress_Constructor_InvalidAddressParts()
        {
            // Area out of range
            Assert.Throws<ArgumentOutOfRangeException>(() => new IndividualAddress(-1, 15, 241));
            Assert.Throws<ArgumentOutOfRangeException>(() => new IndividualAddress(16, 15, 241));
            // Line out of range
            Assert.Throws<ArgumentOutOfRangeException>(() => new IndividualAddress(1, -1, 241));
            Assert.Throws<ArgumentOutOfRangeException>(() => new IndividualAddress(1, 16, 241));
            // Device out of range
            Assert.Throws<ArgumentOutOfRangeException>(() => new IndividualAddress(1, 1, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new IndividualAddress(1, 1, 256));
        }

        [Test]
        public void IndividualAddress_Parse_Ok()
        {
            var sample = "15.15.241";
            var individualAddress = IndividualAddress.Parse(sample);
            Assert.IsTrue(individualAddress.Area == 15);
            Assert.IsTrue(individualAddress.Line == 15);
            Assert.IsTrue(individualAddress.Device == 241);
            Assert.IsTrue(individualAddress.RawAddress == 0xff_f1);
            Assert.IsTrue(individualAddress.AddressType == AddressType.IndividualAddress);
        }

        [Test]
        public void GroupAddress_Constructor1_Ok()
        {
            ushort sampleAddress = 0x31_2f; // 6/303 = 6/1/47 = 12591 = 0x31_2f
            var group = new GroupAddress(sampleAddress);

            Assert.IsTrue(group.RawAddress == 0x31_2f);
            Assert.IsTrue(group.TwoLevelAddress.MainGroup == 6);
            Assert.IsTrue(group.TwoLevelAddress.SubGroup == 303);
            Assert.IsTrue(group.AddressType == AddressType.GroupAddress);
        }

        [Test]
        public void GroupAddress_TwoLevel_Parse_Ok()
        {
            var sample = "6/303"; // 0x31_2f
            var group = GroupAddress.Parse(sample);
            var (main, sub) = group.TwoLevelAddress;
            Assert.IsTrue(main == 6);
            Assert.IsTrue(sub == 303);
        }

        [Test]
        public void GroupAddress_ThreeLevel_Parse_Ok()
        {
            var sample = "6/1/47"; // 0x31_2f
            var group = GroupAddress.Parse(sample);
            var (main, middle, sub) = group.ThreeLevelAddress;
            Assert.IsTrue(main == 6);
            Assert.IsTrue(middle == 1);
            Assert.IsTrue(sub == 47);
        }

        [Test]
        public void GroupAddress_ToString_Ok()
        {
            // 6/303 = 6/1/47 = 12591 = 0x31_2f
            var group = new GroupAddress(0x312f);
            Assert.AreEqual(group.ToString(GroupAddressStyle.ThreeLevel), "6/1/47");
            Assert.AreEqual(group.ToString(GroupAddressStyle.TwoLevel), "6/303");
            Assert.AreEqual(group.ToString(GroupAddressStyle.Free), "12591");
        }

        [Test]
        public void GroupAddress_ToBytes_Ok()
        {
            var group = new GroupAddress(6, 1, 47);
            Assert.AreEqual(group.ToBytes(), new byte[2] { 0x31, 0x2f });
        }
    }
    }
