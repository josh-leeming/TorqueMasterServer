using System;
using System.Net.Sockets;
using MasterServer.ServiceModel;

namespace MasterServer.ServiceInterface
{
    public interface IMessageHandler
    {
        UdpMessage ParseUdpReceiveResult(UdpReceiveResult result);
        void ProcessUdpMessage(UdpMessage message);
        void OnUdpMessageReceivedError(UdpReceiveResult result, UdpMessage message, Exception exception);
    }
}