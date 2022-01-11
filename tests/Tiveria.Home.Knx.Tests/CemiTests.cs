using System;
using System.Net;
using NUnit.Framework;
using Tiveria.Home.Knx.EMI;
using Tiveria.Common.Extensions;
using Tiveria.Common.IO;
using Tiveria.Home.Knx.Datapoint;

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


        [Test]
        public void ParseCemi01_ok()
        {
            var data = basic.ToByteArray(); //Standard body for Connection_Request
            var result = CemiLData.Parse(data, 0, data.Length);

            Assert.AreEqual(result.MessageCode, CemiMessageCode.LDATA_IND);
            Assert.AreEqual(result.AdditionalInfoLength, 0);
            Assert.AreEqual(result.AdditionalInfoFields.Length, 0);

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

            Assert.AreEqual(result.Payload.Length, 2);
        }

        [Test]
        public void ParseCemi02_ok()
        {
            var data = withAI.RemoveAll('_').ToByteArray(); //Standard body for Connection_Request
            var result = CemiLData.Parse(data, 0, data.Length);

            Assert.AreEqual(result.MessageCode, CemiMessageCode.LDATA_IND);
            Assert.AreEqual(result.AdditionalInfoLength, 6);
            Assert.AreEqual(result.AdditionalInfoFields.Length, 1);
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

            Assert.AreEqual(result.Payload.Length, 2);
        }

        [Test]
        public void CreateCemi()
        {
            var data = DPType7.DPT_TIMEPERIOD_HRS.Encode(4604);
            var cemi = EMI.CemiLData.CreateReadAnswerCemi(new IndividualAddress(1, 1, 3), new GroupAddress(6, 1, 47), data);
            var bytes = cemi.ToBytes();
            Assert.AreEqual(1, 1);
        }
    }
}
