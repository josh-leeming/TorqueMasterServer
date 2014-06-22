using System.Net;
using MasterServer.ServiceModel;

namespace MasterServer.Torque.ServiceModel.Messages
{
    public class GameInfoResponse : UdpMessage
    {
        public ServerInfo ServerInfo { get; set; }

        public GameInfoResponse(IPEndPoint remotEndPoint)
            : base(remotEndPoint)
        {
            PacketType = (int)TorqueMessageTypes.GameMasterInfoResponse;
        }
    }
}
