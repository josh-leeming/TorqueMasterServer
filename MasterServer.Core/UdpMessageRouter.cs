using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using MasterServer.Extensions;
using MasterServer.ServiceInterface;
using MasterServer.ServiceModel;

namespace MasterServer
{
    /// <summary>
    /// Process and routes parsed UdpReceiveResult objects as UdpMessage dto to anyone who cares.
    /// </summary>
    public class UdpMessageRouter : IMessageHandler, IMessageRouter<UdpMessage>
    {
        #region Dependencies
        public ILogHandler Logger { get; set; }
        #endregion

        #region Props
        private static readonly object _lock = new object();
        private static readonly List<EventSubscription<UdpMessage>> _subscriptions = new List<EventSubscription<UdpMessage>>(); 
        #endregion

        #region Ctor
        public UdpMessageRouter(ILogHandler logger)
        {
            Logger = logger;
        } 
        #endregion

        #region IMessageHandler

        public virtual UdpMessage ParseUdpReceiveResult(UdpReceiveResult result)
        {
            var message = result.ToUdpMessage();

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug(string.Format("RECEIVE {0}, PacketType {1}, Size {2}", message.RemoteEndPoint, message.PacketType, message.Message.Count()));
            }

            return message;
        }

        public virtual void ProcessUdpMessage(UdpMessage message)
        {
            if (Publish(message) == false && Logger.IsWarnEnabled)
            {
                Logger.Warn(string.Format("Unhandled message from {0}, PacketType {1}, Size {2}", message.RemoteEndPoint, message.PacketType, message.Message.Count()));
            }
        }

        public virtual void OnUdpMessageReceivedError(UdpReceiveResult result, UdpMessage message, Exception exception)
        {
            Logger.Error(string.Format("Exception processing message from {0}, {1}", result.RemoteEndPoint, message != null ? message.GetType().Name : "(null)"), exception);
        }  
        #endregion

        /// <summary>
        /// Publish a message
        /// </summary>
        /// <param name="message">Message to publish</param>
        /// <returns>Flag indicating if the messaged was handled by at least 1 subscriber</returns>
        public bool Publish(UdpMessage message)
        {
            var handled = false;

            lock (_lock)
            {
                _subscriptions.OrderByDescending(s => s.Priority).ToList().ForEach(es =>
                {
                    if (es != null && es.Handler != null && es.Predicate != null)
                    {
                        if (es.Predicate.Invoke(message))
                        {
                            if (InvokeActionWrapper != null)
                            {
                                InvokeActionWrapper.Invoke(() => es.Handler.Invoke(message));
                            }
                            else
                            {
                                es.Handler.Invoke(message);
                            }
                            handled = true;
                        }
                    }
                });
            }
            return handled;
        }

        /// <summary>
        /// Subscribe to a message
        /// </summary>
        /// <param name="handler">Action to call when a message is present</param>
        /// <returns>A subscription token that can be used to modify this subscription</returns>
        public MessageSubscriptionToken Subscribe(Action<UdpMessage> handler)
        {
            return Subscribe(handler, T => true);
        }

        /// <summary>
        /// Subscribe to a message
        /// </summary>
        /// <param name="handler">Action to call when a message is present</param>
        /// <param name="predicate">Predicate to determine whether the message is appropriate for this subscriber</param>
        /// <param name="priority"></param>
        /// <returns>A subscription token that can be used to modify this subscription</returns>
        public MessageSubscriptionToken Subscribe(Action<UdpMessage> handler, Predicate<UdpMessage> predicate, SubscriptionPriority priority = SubscriptionPriority.Normal)
        {
            EventSubscription<UdpMessage> es;

            lock (_lock)
            {
                es = new EventSubscription<UdpMessage> { Handler = handler, Predicate = predicate };
                _subscriptions.Add(es);
            }
            return es.Token;
        }

        /// <summary>
        /// Action wrapper used to contain the invoked action. 
        /// </summary>
        public Action<Action> InvokeActionWrapper { get; set; }

        /// <summary>
        /// Unsubscribe from a message
        /// </summary>
        /// <param name="token">Subscription to unsubscribe</param>
        public void Unsubscribe(MessageSubscriptionToken token)
        {
            lock (_lock)
            {
                var sub = _subscriptions.SingleOrDefault(s => s.Token == token);
                if (sub != null)
                {
                    _subscriptions.Remove(sub);
                    sub.Dispose();
                }
            }
        }
    }

    public class MessageSubscriptionToken
    {
        public Guid Token { get; set; }
    }

    public enum SubscriptionPriority
    {
        Low, Normal, High
    }

    public interface IMessageRouter<T>
    {
        MessageSubscriptionToken Subscribe(Action<T> handler, Predicate<T> predicate, SubscriptionPriority priority);
        bool Publish(T message);
    }

    internal class EventSubscription<T>
    {
        public EventSubscription()
        {
            Token = new MessageSubscriptionToken() { Token = Guid.NewGuid() };
        }
        public MessageSubscriptionToken Token { get; set; }
        public Action<T> Handler { get; set; }
        public Predicate<T> Predicate { get; set; }
        public SubscriptionPriority Priority { get; set; }

        public void Dispose()
        {
            Handler = null;
            Predicate = null;
        }
    }
}
