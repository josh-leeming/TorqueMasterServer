using System.Net;
using MasterServer.ServiceModel;

namespace MasterServer.Torque.ServiceModel
{
    public class MultiPartUdpMessage : UdpMessage
    {
        public ushort PacketIndex { get; set; }
        public ushort TotalPackets { get; set; }

        public MultiPartUdpMessage(IPEndPoint remotEndPoint)
            : base(remotEndPoint)
        {
            
        }
    }
}
