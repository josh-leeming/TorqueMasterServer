using System.IO;
using System.Net;
using System.Net.Sockets;
using MasterServer.ServiceModel;

namespace MasterServer.Extensions
{
    public static class UdpMessageExtensions
    {
        public static UdpMessage ToUdpMessage(this UdpReceiveResult result)
        {
            return result.Buffer.ToUdpMessage(result.RemoteEndPoint);
        }

        public static UdpMessage ToUdpMessage(this byte[] message, IPEndPoint endPoint)
        {
            using (var stream = new MemoryStream(message))
            {
                using (var binaryReader = new BinaryReader(stream))
                {
                    var udpMsg = new UdpMessage(endPoint)
                    {
                        Message = message,
                        PacketType = binaryReader.ReadByte(),
                        Flags = binaryReader.ReadByte(),
                        Session = binaryReader.ReadUInt16(),
                        Key = binaryReader.ReadUInt16()
                    };
                    return udpMsg;
                }
            }
        }
    }
}
