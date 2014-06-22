using System.Net;
using MasterServer.ServiceModel;

namespace MasterServer.ServiceInterface
{
    public interface ISessionHandler
    {
        bool HasSession(IPEndPoint remoteAddress);

        bool Heartbeat(IPEndPoint remoteAddress);

        Session GetSession(IPEndPoint remoteAddress);

        Session CreateSession(IPEndPoint remoteAddress);
    }
}