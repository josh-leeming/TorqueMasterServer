using System.Net;

namespace MasterServer.Torque.ServiceModel.Messages
{
    public class ServerListResponse : MultiPartUdpMessage
    {
        public ServerListResponse(IPEndPoint remoteEndPoint)
            : base(remoteEndPoint)
        {
            
        }
    }
}
