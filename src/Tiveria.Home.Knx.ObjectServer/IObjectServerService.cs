using Tiveria.Common.IO;

namespace Tiveria.Home.Knx.ObjectServer
{
    public interface IObjectServerService
    {
        byte MainService { get; }
        byte SubService { get; }
        int Size { get; }
    }

}
