using System;
using System.Net;

namespace MasterServer.ServiceModel
{
    public class Session
    {
        public ushort SessionID { get; set; }
        public ushort Key { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset Heartbeat { get; set; }
    }
}