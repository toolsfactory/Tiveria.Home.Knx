using Tiveria.Common.IO;
using Tiveria.Home.Knx.Exceptions;
using Tiveria.Home.Knx.IP;
using Tiveria.Home.Knx.IP.Services.Serializers;
using Tiveria.Home.Knx.IP.Structures;

namespace Tiveria.Home.Knx.ObjectServer
{
    public class ObjectServerProtocolService : IKnxNetIPService
    {
        public int Size { get; init; }

        public ushort ServiceTypeIdentifier => IP.Enums.ServiceTypeIdentifier.ObjectServer;
        public ConnectionHeader ConnectionHeader { get; init; }

        public IObjectServerService ObjectServerService { get; set; }

        public ObjectServerProtocolService(ConnectionHeader connectionHeader, IObjectServerService objectServerService)
        {
            ConnectionHeader = connectionHeader;
            ObjectServerService = objectServerService;
            Size = ConnectionHeader.Size + ObjectServerService.Size;
        }
    }
}
