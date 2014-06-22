using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using MasterServer.ServiceInterface;
using MasterServer.ServiceModel;

namespace MasterServer.Torque
{
    /// <summary>
    /// Simple spam checking
    /// </summary>
    public class SpamManager : IMessageLifecycleCallbackHandler
    {
        private object padlock = new object();

        #region Dependencies
        public ILogHandler Logger { get; set; }
        #endregion

        public SpamManager(ILogHandler logger)
        {
            Logger = logger;
        }

        public class Spammer
        {
            public IPEndPoint RemoteAddress;
            public float SpamCount;
            public float BanTime;
            public long Ticks;
        }

        public Dictionary<IPEndPoint, Spammer> SpamList = new Dictionary<IPEndPoint,Spammer>();

        public CallbackPriority Priority
        {
            get { return CallbackPriority.High; }                
        }

        public bool BeforeUdpMessageReceived(UdpReceiveResult result)
        {
            Logger.Debug("SPAMCHECK");

            lock (padlock)
            {
                if (SpamList.ContainsKey(result.RemoteEndPoint))
                {
                    if ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - SpamList[result.RemoteEndPoint].Ticks < 100000)
                    {
                        SpamList[result.RemoteEndPoint].SpamCount += 1;

                        if (SpamList[result.RemoteEndPoint].SpamCount > 10)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (SpamList.Count > 5000)
                    {
                        var expired = SpamList.Where(kvp => kvp.Value.BanTime.Equals(0)).ToList();
                        expired.ForEach(kvp => SpamList.Remove(kvp.Key));
                    }

                    SpamList.Add(result.RemoteEndPoint, new Spammer()
                    {
                        SpamCount = 1,
                        Ticks = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond
                    });
                }  
            }
            return true;
        }

        public void AfterUdpMessageReceived(UdpReceiveResult result)
        {

        }
    }
}