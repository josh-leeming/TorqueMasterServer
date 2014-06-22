using System.Net;
using MasterServer.ServiceModel;

namespace MasterServer.Torque.ServiceModel.Messages
{
    public class ServerListRequest : UdpMessage
    {
        public ServerListRequest(IPEndPoint remotEndPoint)
            : base(remotEndPoint)
        {
            
        }
    }
}
