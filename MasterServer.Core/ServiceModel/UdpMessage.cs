using System.Net;

namespace MasterServer.ServiceModel
{
    public class UdpMessage
    {
        public IPEndPoint RemoteEndPoint;
        public byte[] Message;
        public ushort PacketType { get; set; }
        public ushort Flags { get; set; }
        public ushort Session { get; set; }
        public ushort Key { get; set; }

        public UdpMessage(IPEndPoint remoteEndPoint)
        {
            RemoteEndPoint = remoteEndPoint;
        }
    }
}
