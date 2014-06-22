using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using MasterServer.ServiceInterface;
using MasterServer.ServiceModel;

namespace MasterServer.Torque
{
    public class InMemorySessionManager : ISessionHandler
    {
        #region Dependencies
        public ILogHandler Logger { get; set; }
        #endregion

        private readonly Dictionary<IPEndPoint, Session> sessions = new Dictionary<IPEndPoint, Session>(); 

        public InMemorySessionManager(ILogHandler logger)
        {
            Logger = logger;
        }

        public bool HasSession(IPEndPoint remoteAddress)
        {
            return sessions.ContainsKey(remoteAddress);
        }

        public bool Heartbeat(IPEndPoint remoteAddress)
        {
            if (HasSession(remoteAddress))
            {
                sessions[remoteAddress].Heartbeat = DateTimeOffset.Now;

                return true;
            }
            return false;
        }

        public Session GetSession(IPEndPoint remoteAddress)
        {
            Session session;

            sessions.TryGetValue(remoteAddress, out session);

            return session;
        }

        public Session CreateSession(IPEndPoint remoteAddress)
        {
            if (sessions.ContainsKey(remoteAddress))
            {
                throw new Exception("session already exists");    
            }

            var random = new Random();

            var session = new Session
            {
                CreatedDate = DateTimeOffset.Now,
                Heartbeat = DateTimeOffset.Now,
                SessionID = (ushort)random.Next(ushort.MaxValue),
                Key = (ushort)random.Next(ushort.MaxValue)
            };

            Logger.Info(string.Format("{0} SESSION : {1}, Key {2}", remoteAddress, session.SessionID, session.Key));

            sessions.Add(remoteAddress, session);

            return session;
        }
    }

    public class SessionMessageLifecycleCallback : IMessageLifecycleCallbackHandler
    {
        #region Dependencies
        public ISessionHandler SessionHandler { get; set; }
        #endregion

        public SessionMessageLifecycleCallback(ISessionHandler sessionHandler)
        {
            SessionHandler = sessionHandler;
        }

        #region IMessageLifecycleCallbackHandler
        /// <summary>
        /// Normal. Only want to create a session if the endpoint passes any security checks/spam checking.
        /// </summary>
        public CallbackPriority Priority
        {
            get { return CallbackPriority.Normal; }
        }

        public bool BeforeUdpMessageReceived(UdpReceiveResult result)
        {
            if (SessionHandler.HasSession(result.RemoteEndPoint) == false)
            {
                SessionHandler.CreateSession(result.RemoteEndPoint);
            }
            return true;
        }

        public void AfterUdpMessageReceived(UdpReceiveResult result)
        {

        }
        #endregion
    }
}