using MasterServer.ServiceModel;

namespace MasterServer.ServiceInterface
{
    public interface IApplicationLifecycleCallbackHandler
    {
        CallbackPriority Priority { get; }

        void OnInitialise();

        void OnStartup();

        void OnShutdown();
    }
}