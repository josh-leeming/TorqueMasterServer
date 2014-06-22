using System.Net.Sockets;
using MasterServer.ServiceInterface;
using MasterServer.ServiceModel;

namespace MasterServer.Torque
{
    public class TorqueMessageRouter : UdpMessageRouter
    {
        public TorqueMessageRouter(ILogHandler logger) : base(logger)
        {
        }

        public override UdpMessage ParseUdpReceiveResult(UdpReceiveResult result)
        {
            return TorqueMessageFactory.ParseUdpReceiveResult(result);
        }
    }
}
