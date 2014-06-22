using System.Net;
using MasterServer.ServiceModel;

namespace MasterServer.Torque.ServiceModel.Messages
{
    public class GameInfoRequest : UdpMessage
    {
        public GameInfoRequest(IPEndPoint remotEndPoint) : base(remotEndPoint)
        {
            PacketType = (int) TorqueMessageTypes.GameMasterInfoRequest;
        }
    }
}
