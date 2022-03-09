using Tiveria.Common.IO;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.IP.Services.Serializers;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.ObjectServer
{
    public class ObjectServerProtocolServiceSerializer : ServiceSerializerBase<ObjectServerProtocolService>
    {
        public override ushort ServiceTypeIdentifier => IP.Enums.ServiceTypeIdentifier.ObjectServer;

        public override ObjectServerProtocolService Deserialize(BigEndianBinaryReader reader)
        {
            var conHeader = ConnectionHeader.Parse(reader);
            var mainSvc = reader.ReadByte();
            var subSvc = reader.ReadByte();
            reader.Seek(reader.Position - 2);
            var serializer = ObjectServerServiceSerializerFactory.Instance.Create(mainSvc, subSvc);
            var objectService = serializer.Deserialize(reader);
            return new ObjectServerProtocolService(conHeader, objectService);
        }

        public override void Serialize(ObjectServerProtocolService service, BigEndianBinaryWriter writer)
        {
            service.ConnectionHeader.Write(writer);

            var serializer = ObjectServerServiceSerializerFactory.Instance.Create(service.ObjectServerService.MainService, service.ObjectServerService.SubService);
            serializer.Serialize(service.ObjectServerService, writer);
        }

        static ObjectServerProtocolServiceSerializer()
        {
            KnxNetIPServiceSerializerFactory.Instance.Register<ObjectServerProtocolServiceSerializer>();
        }
    }
}
