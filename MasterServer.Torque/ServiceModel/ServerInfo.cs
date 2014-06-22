using System.Collections.Generic;
using System.Net;

namespace MasterServer.Torque.ServiceModel
{
    public class ServerInfo
    {
        public IPEndPoint RemoteAddress { get; set; }

        public string GameType { get; set; }
        public string MissionType { get; set; }

        public int MaxPlayers { get; set; }
        public uint Regions { get; set; }
        public uint Version { get; set; }
        public ushort InfoFlags { get; set; }
        public ushort NumBots { get; set; }
        public uint CPUSpeed { get; set; }

        public int PlayerCount { get; set; }
        public List<uint> PlayerList { get; set; }
    }
}
