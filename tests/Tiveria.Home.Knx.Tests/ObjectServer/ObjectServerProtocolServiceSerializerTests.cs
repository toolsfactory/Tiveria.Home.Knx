using NUnit.Framework;
using Tiveria.Common.Extensions;
using Tiveria.Common.IO;
using Tiveria.Home.Knx.ObjectServer.Services.Serializers;
using Tiveria.Home.Knx.ObjectServer;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.Tests.ObjectServer
{
    [TestFixture]
    public class ObjectServerProtocolServiceSerializerTests
    {
        static readonly object[] TestData_Simp = new object[]
        {
            // input, service type
            new object[] { "04000000 F00600060001000603020F12",  typeof(SetDatapointValueReqService) },
            new object[] { "04000000 F006000100010001030101",    typeof(SetDatapointValueReqService) },
            new object[] { "04000000 F006003500010035030101",    typeof(SetDatapointValueReqService) },
            new object[] { "04000000 F00600070001000703021645",  typeof(SetDatapointValueReqService) },
            new object[] { "04000000 F006000700020007030216450035030101",  typeof(SetDatapointValueReqService) },
        };

        [TestCaseSource(nameof(TestData_Simp))]
        [Test]
        public void ParseSvc_Default_Ok(string input, Type serviceType)
        {
            var reader = new BigEndianBinaryReader(input.RemoveAll(' ').ToByteArray());
            var deserializer = new ObjectServerProtocolServiceSerializer();
            var ok = deserializer.TryDeserialize(reader, out var frame);
            Assert.IsTrue(ok);
            Assert.IsNotNull(frame);
            Assert.IsInstanceOf(serviceType, frame!.ObjectServerService);
        }
    }
}
