using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using MasterServer.ServiceInterface;
using MasterServer.ServiceModel;
using MasterServer.Torque.Extensions;
using MasterServer.Torque.ServiceModel;
using MasterServer.Torque.ServiceModel.Messages;
using TinyIoC;

namespace MasterServer.Torque
{
    /// <summary>
    /// Torque implementation of master server
    /// </summary>
    public class TorqueMasterServer : MasterServerBase
    {
        #region Dependencies
        public ILogHandler Logger { get; set; } 
        public ISessionHandler SessionHandler { get; set; }
        #endregion

        #region Props
        private readonly Dictionary<IPEndPoint, ServerInfo> serverList = new Dictionary<IPEndPoint, ServerInfo>();
        #endregion
 
        public TorqueMasterServer(TinyIoCContainer container, IPEndPoint endpoint)
            : base(container, endpoint)
        {
            MessageHandler = new TorqueMessageRouter(container.Resolve<ILogHandler>());
        }

        protected override void OnInitialise()
        {
            Logger.Info("Initialising");

            UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            base.OnInitialise();

            //wire up some message handling
            var torqueMessageRouter = MessageHandler as TorqueMessageRouter;
            torqueMessageRouter.Subscribe(ProcessHeartbeat, x => x is GameHeartbeat);
            torqueMessageRouter.Subscribe(ProcessGameInfoResponse, x => x is GameInfoResponse);
            torqueMessageRouter.Subscribe(ProcessServerListRequest, x => x is ServerListRequest);
        }

        protected override void OnStartup()
        {
            Logger.Info("Starting");

            base.OnStartup();

            Logger.Info(string.Format("Listening on {0}", ServerEndPoint));
        }

        protected override void OnShutdown()
        {
            Logger.Info("Shutting down");

            base.OnShutdown();

            Logger.Info("Bye");
        }

        protected override Task<int> Send(UdpMessage message)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug(string.Format("SEND {0}, PacketType {1}, Size {2}", message.RemoteEndPoint, message.PacketType, message.Message.Count()));
            }
            return base.Send(message);
        }

        private void ProcessHeartbeat(UdpMessage message)
        {
            var heartbeat = message as GameHeartbeat;

            Logger.Info(string.Format("{0} Heartbeat", heartbeat.RemoteEndPoint));

            if (SessionHandler.Heartbeat(heartbeat.RemoteEndPoint))
            {
                var session = SessionHandler.GetSession(heartbeat.RemoteEndPoint);

                //send game info request
                var gameInfoReq = new GameInfoRequest(heartbeat.RemoteEndPoint );
                gameInfoReq.AddSession(session);

                gameInfoReq.Message = gameInfoReq.WriteUdpMessage();

                Send(gameInfoReq);
            }
        }

        private void ProcessGameInfoResponse(UdpMessage message)
        {
            var gameInfo = message as GameInfoResponse;

            Logger.Info(string.Format("{0} GameInfo", gameInfo.RemoteEndPoint));

            if (serverList.ContainsKey(gameInfo.RemoteEndPoint))
            {
                serverList[gameInfo.RemoteEndPoint] = gameInfo.ServerInfo;
            }
            else
            {
                serverList.Add(gameInfo.RemoteEndPoint, gameInfo.ServerInfo);
            }
        }

        private void ProcessServerListRequest(UdpMessage message)
        {
            Logger.Info(string.Format("{0} ServerList", message.RemoteEndPoint));

            //query server list and send results back

            var session = SessionHandler.GetSession(message.RemoteEndPoint);

            if (serverList.Any())
            {
                var total = (ushort)serverList.Count;
                ushort packetIndex = 0;

                serverList.Values.ToList().ForEach(serverInfo =>
                {
                    var serverListResponse = new ServerListResponse(message.RemoteEndPoint);
                        serverListResponse.AddSession(session);

                    serverListResponse.PacketIndex = ++ packetIndex;
                    serverListResponse.TotalPackets = total;

                    serverListResponse.Message = serverListResponse.WriteUdpMessage().Concat(serverInfo.WriteUdpMessage()).ToArray();

                    Send(serverListResponse);
                });
            }
            else
            {
                var serverListResponse = new ServerListResponse(message.RemoteEndPoint);
                serverListResponse.AddSession(session);

                serverListResponse.PacketIndex = 1;
                serverListResponse.TotalPackets = 1;

                serverListResponse.Message = serverListResponse.WriteUdpMessage();

                Send(serverListResponse);
            }

        }
    }
}
