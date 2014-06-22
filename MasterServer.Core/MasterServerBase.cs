using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using MasterServer.ServiceInterface;
using MasterServer.ServiceModel;
using TinyIoC;

namespace MasterServer
{
    /// <summary>
    /// Base class for Master Server implementation.
    /// </summary>
    public abstract class MasterServerBase : IMasterServer
    {
        #region Dependencies
        public IMessageHandler MessageHandler { get; set; }        
        #endregion

        #region Props

        protected IPEndPoint ServerEndPoint { get; set; }

        protected UdpClient UdpClient;
        protected List<IApplicationLifecycleCallbackHandler> ApplicationLifecycleCallbacks = new List<IApplicationLifecycleCallbackHandler>();
        protected List<IMessageLifecycleCallbackHandler> MessageLifecycleCallbacks = new List<IMessageLifecycleCallbackHandler>();

        private bool isInitialised;
        #endregion

        #region Ctor
        protected MasterServerBase(TinyIoCContainer container, IMessageHandler messageHandler, IPEndPoint endpoint)
            : this(container, endpoint)
        {
            MessageHandler = messageHandler;
        }

        protected MasterServerBase(TinyIoCContainer container, IPEndPoint endpoint)
            : this(endpoint)
        {
            // Resolve lifecycle callbacks
            ApplicationLifecycleCallbacks.AddRange(
                container.ResolveAll<IApplicationLifecycleCallbackHandler>().OrderByDescending(s => s.Priority).ToList());

            MessageLifecycleCallbacks.AddRange(
                container.ResolveAll<IMessageLifecycleCallbackHandler>().OrderByDescending(s => s.Priority).ToList());
        }

        private MasterServerBase(IPEndPoint endpoint)
        {
            ServerEndPoint = endpoint;
            UdpClient = new UdpClient(ServerEndPoint);  
        }
        #endregion

        #region IMasterServer
        /// <summary>
        /// True if the MasterServer is currently listening for/accepting 
        /// incoming messages.
        /// </summary>
        public bool IsListening { get; protected set; }

        /// <summary>
        /// Start listening for/accepting incoming messages
        /// </summary>
        public virtual void StartMasterServer()
        {
            if (isInitialised == false)
            {
                InitMasterServer();    
            }

            OnStartup();

            IsListening = true;

            //start listening for messages
            Task.Factory.StartNew(async () =>
            {
                while (IsListening)
                {
                    await Receive();
                }
            });
        }

        /// <summary>
        /// Stop listening
        /// </summary>
        public virtual void StopMasterServer()
        {
            OnShutdown();

            IsListening = false;
        } 

        #endregion

        #region Lifecycle
        protected virtual void OnInitialise()
        {
            ApplicationLifecycleCallbacks.ForEach(handler => handler.OnInitialise());
        }

        protected virtual void OnStartup()
        {
            ApplicationLifecycleCallbacks.ForEach(handler => handler.OnStartup());
        }

        protected virtual void OnShutdown()
        {
            ApplicationLifecycleCallbacks.ForEach(handler => handler.OnShutdown());
        } 
        #endregion

        #region Messaging
        protected virtual Task<int> Send(UdpMessage message)
        {
            return UdpClient.SendAsync(message.Message, message.Message.Length, message.RemoteEndPoint);
        }

        protected virtual async Task Receive()
        {
            var udpReceiveResult = await UdpClient.ReceiveAsync();

            //process the result asynchronously
            Task.Factory.StartNew(() => ProcessReceiveResult(udpReceiveResult));
        }

        protected virtual void ProcessReceiveResult(UdpReceiveResult udpReceiveResult)
        {
            UdpMessage message = null;
            try
            {
                //If all "before" callbacks return true we can proceed; logging, security etc
                if (MessageLifecycleCallbacks.All(handler => handler.BeforeUdpMessageReceived(udpReceiveResult)))
                {
                    message = MessageHandler.ParseUdpReceiveResult(udpReceiveResult);

                    MessageHandler.ProcessUdpMessage(message);

                    MessageLifecycleCallbacks.ForEach(
                        handler => handler.AfterUdpMessageReceived(udpReceiveResult));
                }
            }
            catch (Exception e)
            {
                MessageHandler.OnUdpMessageReceivedError(udpReceiveResult, message, e);
            }
        } 
        #endregion

        #region Bootstrap
        protected void InitMasterServer()
        {
            OnInitialise();

            isInitialised = true;
        } 
        #endregion
    }
}
