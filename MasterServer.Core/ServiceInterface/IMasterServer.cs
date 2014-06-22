namespace MasterServer.ServiceInterface
{
    public interface IMasterServer
    {
        bool IsListening { get; }

        void StartMasterServer();
        void StopMasterServer();
    }
}