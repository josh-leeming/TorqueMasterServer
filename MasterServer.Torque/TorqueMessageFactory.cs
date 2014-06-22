using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using MasterServer.ServiceModel;
using MasterServer.Torque.ServiceModel;
using MasterServer.Torque.ServiceModel.Messages;

namespace MasterServer.Torque
{
    public static class TorqueMessageFactory
    {
        /// <summary>
        /// Return concrete "Torque" message type
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static UdpMessage ParseUdpReceiveResult(UdpReceiveResult result)
        {
            using (var stream = new MemoryStream(result.Buffer))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var packetType = reader.ReadByte();

                    switch ((TorqueMessageTypes)packetType)
                    {
                        case TorqueMessageTypes.GameHeartbeat:
                            return result.ToGameHeartbeat(reader, result.RemoteEndPoint);
                        case TorqueMessageTypes.GameMasterInfoResponse:
                            return result.ToGameInfoResponse(reader, result.RemoteEndPoint);
                        default:
                            return new UdpMessage(result.RemoteEndPoint)
                            {
                                PacketType = packetType,
                                Message = result.Buffer
                            };
                    }

                }
            }
        }

        private static GameHeartbeat ToGameHeartbeat(this UdpReceiveResult result, BinaryReader reader, IPEndPoint remoteEndPoint)
        {
            var message = new GameHeartbeat(remoteEndPoint)
            {
                Message = result.Buffer,
                RemoteEndPoint = result.RemoteEndPoint,
                Flags = reader.ReadByte(),
                Session = reader.ReadUInt16(),
                Key = reader.ReadUInt16()
            };
            return message;
        }

        private static GameInfoResponse ToGameInfoResponse(this UdpReceiveResult result, BinaryReader reader, IPEndPoint remoteEndPoint)
        {
            var message = new GameInfoResponse(remoteEndPoint)
            {
                Message = result.Buffer,
                Flags = reader.ReadByte(),
                Session = reader.ReadUInt16(),
                Key = reader.ReadUInt16(),
                ServerInfo = new ServerInfo()
                {
                    RemoteAddress = result.RemoteEndPoint,
                    GameType = reader.ReadString(),
                    MissionType = reader.ReadString(),
                    MaxPlayers = reader.ReadByte(),
                    Regions = reader.ReadUInt32(),
                    Version = reader.ReadUInt32(),
                    InfoFlags = reader.ReadByte(),
                    NumBots = reader.ReadByte(),
                    CPUSpeed = reader.ReadUInt32(),
                    PlayerCount = reader.ReadByte(),
                    PlayerList = new List<uint>()
                }
            };

            for (ushort i = 0; i < message.ServerInfo.PlayerCount; i++)
            {
                var player = reader.ReadUInt32();
                message.ServerInfo.PlayerList.Add(player);
            }

            return message;
        }
    }
}
