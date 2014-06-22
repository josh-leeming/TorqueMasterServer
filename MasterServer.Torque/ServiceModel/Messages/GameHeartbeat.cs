using System.Net;
using MasterServer.ServiceModel;

namespace MasterServer.Torque.ServiceModel.Messages
{
    public class GameHeartbeat : UdpMessage
    {
        public GameHeartbeat(IPEndPoint remotEndPoint)
            : base(remotEndPoint)
        {
            PacketType = (int)TorqueMessageTypes.GameHeartbeat;
        }
    }
}
