using System;
using System.Net.Sockets;
using MasterServer.ServiceModel;

namespace MasterServer.ServiceInterface
{
    public interface IMessageLifecycleCallbackHandler
    {
        CallbackPriority Priority { get; }

        bool BeforeUdpMessageReceived(UdpReceiveResult result);

        void AfterUdpMessageReceived(UdpReceiveResult result);
    }
}