namespace MasterServer.Torque.ServiceModel
{
    public enum TorqueMessageTypes
    {
        ServerGameTypesRequest = 2,
        ServerGameTypesResponse = 4,
        ServerListRequest = 6,
        ServerListResponse = 8,
        GameMasterInfoRequest = 10,
        GameMasterInfoResponse = 12,
        //GamePingRequest = 14,
        //GamePingResponse = 16,
        //GameInfoRequest = 18,
        //GameInfoResponse = 20,
        GameHeartbeat = 22,
        ServerInfoRequest = 24,
        ServerInfoResponse = 26,
    }
}