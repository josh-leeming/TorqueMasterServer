using System.IO;
using MasterServer.ServiceModel;
using MasterServer.Torque.ServiceModel;

namespace MasterServer.Torque.Extensions
{
    public static class UdpMessageExtensions
    {
        public static void AddSession(this UdpMessage message, Session session)
        {
            message.Session = session.SessionID;
            message.Key = session.Key;
        }

        public static byte[] WriteUdpMessage(this UdpMessage message)
        {
            using (var stream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(stream))
                {
                    binaryWriter.Write((byte)message.PacketType);
                    binaryWriter.Write((byte)message.Flags);
                    binaryWriter.Write(message.Session);
                    binaryWriter.Write(message.Key);

                    var multiPart = message as MultiPartUdpMessage;
                    if (multiPart != null)
                    {
                        binaryWriter.Write(multiPart.PacketIndex);
                        binaryWriter.Write(multiPart.TotalPackets);
                    }
                }
                return stream.ToArray();
            }
        }

        public static byte[] WriteUdpMessage(this ServerInfo serverInfo)
        {
            using (var stream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(stream))
                {
                    var addressStrings = serverInfo.RemoteAddress.ToString().Split('.');
                    binaryWriter.Write(byte.Parse(addressStrings[0]));
                    binaryWriter.Write(byte.Parse(addressStrings[1]));
                    binaryWriter.Write(byte.Parse(addressStrings[2]));
                    binaryWriter.Write(byte.Parse(addressStrings[3]));
                }
                return stream.ToArray();
            }
        }
    }
}
