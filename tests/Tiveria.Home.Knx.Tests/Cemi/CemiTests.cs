﻿using System;
using System.Net;
using NUnit.Framework;
using Tiveria.Home.Knx.Cemi;
using Tiveria.Common.Extensions;
using Tiveria.Common.IO;
using Tiveria.Home.Knx.Datapoint;
using Tiveria.Home.Knx.Primitives;
using Tiveria.Home.Knx.Cemi.Serializers;
using Tiveria.Home.Knx.IP.Services;
using Tiveria.Home.Knx.IP;

namespace Tiveria.Home.Knx.Tests
{
    [TestFixture]
    class CemiTests
    {
        /// <summary>
        /// <code>
        /// messagecode: L_Data.ind(0x29)
        /// add information length: 0 octets
        /// 
        /// Controlfield 1: 0xbc
        /// 1... .... = Frametype: 1 = standard
        /// ..1. .... = Repeat: 1 = do not repeat
        /// ...1 .... = System-Broadcast: 1 = normal broadcast
        /// .... 11.. = Priority: 0x3 Low
        /// .... ..0. = Acknowledge-Request: 0 No Ack
        /// .... ...0 = Confirm-Flag: 0 No Error
        /// 
        /// Controlfield 2: 0xd0
        /// 1... .... = Destination address type: 1
        /// .101 .... = Hop count: 5
        /// .... 0000 = Extended Frame Format: 0x0
        /// 
        /// Source Address 1.1.205
        /// Destination Address 6/1/47 or 6/303
        /// NPDU length: 1 octet
        /// 00.. .... = TPCI: UDT(Unnumbered Data Packet) (0x0)
        /// .... ..00  0000 0000 = APCI: A_GroupValue_Read(0x0000)
        /// </code>
        /// </summary>
        private static string basic = "2900bcd011cd312f010000";

        /// <summary>
        /// Same as above but now with additional info
        /// <code>
        /// add information length: 6 octets
        /// add info field type: 0x08 - RFMULTI
        /// add info field len: 4 (payload)
        /// add info payload: 0x01, 0x02, 0x03, 0x04
        /// 
        private string withAI = "29_06_080401020304_bcd011cd312f010000";

        [OneTimeSetUp]
        public void Prepare()
        { }

        [Test]
        public void ParseCemi01_ok()
        {
            var data = basic.ToByteArray(); //Standard body for Connection_Request
            var result = new CemiLDataSerializer().Deserialize(data);

            Assert.AreEqual(result.MessageCode, MessageCode.LDATA_IND);
            Assert.AreEqual(result.AdditionalInfoLength, 0);
            Assert.AreEqual(result.AdditionalInfoFields.Count, 0);

            Assert.AreEqual(result.ControlField1.ExtendedFrame, false);
            Assert.AreEqual(result.ControlField1.Repeat, false);
            Assert.AreEqual(result.ControlField1.Broadcast, BroadcastType.Normal);
            Assert.AreEqual(result.ControlField1.Priority, Priority.Low);
            Assert.AreEqual(result.ControlField1.AcknowledgeRequest, false);
            Assert.AreEqual(result.ControlField1.Confirm, ConfirmType.NoError);

            Assert.AreEqual(result.ControlField2.HopCount, 5);
            Assert.AreEqual(result.ControlField2.DestinationAddressType, AddressType.GroupAddress);
            Assert.AreEqual(result.ControlField2.ExtendedFrameFormat, 0);

            Assert.AreEqual(result.SourceAddress.ToString(), "1.1.205");
            Assert.AreEqual(((GroupAddress) result.DestinationAddress).ToString(), "6/1/47");
            Assert.IsNotNull(result.Apdu);
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
            Assert.AreEqual(result.Apdu.Size, 2);
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
        }

        [Test]
        public void ParseCemi02_ok()
        {
            var data = withAI.RemoveAll('_').ToByteArray(); //Standard body for Connection_Request
            var result = new CemiLDataSerializer().Deserialize(data);

            Assert.AreEqual(result.MessageCode, MessageCode.LDATA_IND);
            Assert.AreEqual(result.AdditionalInfoLength, 6);
            Assert.AreEqual(result.AdditionalInfoFields.Count, 1);
            Assert.AreEqual(result.AdditionalInfoFields[0].InfoType, AdditionalInfoType.RFMULTI);
            Assert.AreEqual(result.AdditionalInfoFields[0].InfoLength, 4);
            Assert.AreEqual(result.AdditionalInfoFields[0].Information, new byte[4] { 0x01, 0x02, 0x03, 0x04 });

            Assert.AreEqual(result.ControlField1.ExtendedFrame, false);
            Assert.AreEqual(result.ControlField1.Repeat, false);
            Assert.AreEqual(result.ControlField1.Broadcast, BroadcastType.Normal);
            Assert.AreEqual(result.ControlField1.Priority, Priority.Low);
            Assert.AreEqual(result.ControlField1.AcknowledgeRequest, false);
            Assert.AreEqual(result.ControlField1.Confirm, ConfirmType.NoError);

            Assert.AreEqual(result.ControlField2.HopCount, 5);
            Assert.AreEqual(result.ControlField2.DestinationAddressType, AddressType.GroupAddress);
            Assert.AreEqual(result.ControlField2.ExtendedFrameFormat, 0);

            Assert.AreEqual(result.SourceAddress.ToString(), "1.1.205");
            Assert.AreEqual(((GroupAddress)result.DestinationAddress).ToString(), "6/1/47");
            Assert.IsNotNull(result.Apdu);
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
            Assert.AreEqual(result.Apdu.Size, 2);
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
        }

        [Test]
        public void Parse_Cemi04_ok()
        {
            var data = "29-00-BC-E0-11-03-19-0C-05-00-80-3D-30-20-C5-06-10-04-20-00-19-04-6D-00-00-29-00-BC-E0-11-03-19-0C-05-00-80-3D-30-20-C5".Replace("-", "").ToByteArray();
            var result = new CemiLDataSerializer().Deserialize(data);
        }

        [Test]
        public void Parse_Cemi05_ok()
        {
            var data = "29-00-BC-D0-11-78-3A-03-02-00-40-00".Replace("-", "").ToByteArray();
            var result = new CemiLDataSerializer().Deserialize(data);
            Assert.AreEqual(12, result.Size);
        }

        [Test]
        public void Parse_Cemi06_ok()
        {
            var data = "29-00-bc-e0-11-a2-8d-00-03-00-80-0c-7e".Replace("-", "").ToByteArray();
            var result = new CemiLDataSerializer().Deserialize(data);
            Assert.AreEqual(13, result.Size);
        }

        [Test]
        public void Build_Cemi01_ok()
        {
            // cemi LData.Req request from 0.0.0 to 4/0/0 with GroupValueWrite and data = 0x01
            var expected = "11-00-b6-e0-00-00-20-00-01-00-81".Replace("-", "").ToByteArray();

            var apdu = new Cemi.Apdu(Cemi.ApduType.GroupValue_Write, new byte[] { 0x01 });
            var ctrl1 = new ControlField1();
            var ctrl2 = new ControlField2(groupAddress: true);
            var cemi = new Cemi.CemiLData(Cemi.MessageCode.LDATA_REQ, new List<AdditionalInformationField>(), new IndividualAddress(0, 0, 0), GroupAddress.Parse("4/0/0"), ctrl1, ctrl2, new Tpci(), apdu);

            var data = new CemiLDataSerializer().Serialize(cemi);
            Assert.AreEqual(expected, data);
        }

        [Test]
        public void Build_Cemi02_ok()
        {
            var tpci = new Tpci(PacketType.Control, SequenceType.UnNumbered, 0, ControlType.Connect);
            var ctrl1 = new ControlField1(priority: Priority.System, broadcast: BroadcastType.Normal, ack: false);
            var ctrl2 = new ControlField2(groupAddress: false);
            var cemi = new CemiLData(Cemi.MessageCode.LDATA_REQ, new List<AdditionalInformationField>(), new IndividualAddress(0, 0, 0), IndividualAddress.Parse("1.1.2"), ctrl1, ctrl2, tpci, null);

            var data = new CemiLDataSerializer().Serialize(cemi);
        }
    }
}
//var request = "061004200017041300002900bce011a28d000300800c7e".ToByteArray();
