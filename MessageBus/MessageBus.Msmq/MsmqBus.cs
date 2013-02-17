using System;
using System.Collections.Generic;
using System.Globalization;
using System.Messaging;
using System.Threading.Tasks;
using MessageBus.Msmq.Helpers;
using Timer = System.Timers.Timer;

namespace MessageBus.Msmq
{
    public class MsmqBus : IBus, IAdvancedOperations
    {
        #region Bus Events

        internal sealed class BusEvents : IBusEvents
        {
            public event EventHandler<FaultEventArgs> FaultOccurred;
            public event EventHandler<QueueEventArgs> QueueCreated;
            public event EventHandler<MessageTypeEventArgs> Listening;

            public void OnFaultOccurred(String messageId, IMessage message, Exception ex)
            {
                EventHandler<FaultEventArgs> handler = FaultOccurred;

                if (handler != null)
                {
                    handler(this, new FaultEventArgs(messageId, message, ex));
                }
            }

            public void OnQueueCreated(string queueName)
            {
                EventHandler<QueueEventArgs> handler = QueueCreated;

                if (handler != null)
                {
                    handler(this, new QueueEventArgs(queueName));
                }
            }

            public void OnListening(Type messageType)
            {
                EventHandler<MessageTypeEventArgs> handler = Listening;

                if (handler != null)
                {
                    handler(this, new MessageTypeEventArgs(messageType));
                }
            }
        }

        #endregion

        private readonly object syncRoot = new Object();
        private readonly MessageListener listener;
        private readonly IBusLogger logger;

        public MsmqBus()
        {
            listener = new MessageListener(this);

            Advanced = this;
            Events = new BusEvents();
            Settings = new BusSettings
                       {
                           MinRetryTimeout = TimeSpan.FromSeconds(10),
                           ResponseTimeout = TimeSpan.FromSeconds(10),
                           SendAttemptTimeout = TimeSpan.FromMilliseconds(100),
                           SendAttempts = 3
                       };

            logger = BusConfiguration.Configure.Logger;
        }

        public IAdvancedOperations Advanced { get; protected set; }
        public IBusEvents Events { get; protected set; }
        public BusSettings Settings { get; protected set; }

        public void Subscribe<T>()
            where T : IMessage
        {
            Subscribe(typeof(T));
        }

        public void Subscribe(params Type[] messageTypes)
        {
            if (messageTypes == null) throw new ArgumentNullException("messageTypes");
            if (messageTypes.Length == 0) throw new ArgumentException("No subscribed message types provided", "messageTypes");

            for (int i = 0; i < messageTypes.Length; i++)
            {
                Type messageType = messageTypes[i];
                if (messageType == null) continue;

                listener.Start(messageType).ConfigureAwait(false);

                logger.Info("Listening for messages of type '{0}'...", messageType.FullName);
            }
        }

        public void SubscribeAll()
        {
            Subscribe(BusConfiguration.GetHandledTypes());
        }

        public void Unsubscribe<T>()
            where T : IMessage
        {
            Unsubscribe(typeof(T));
        }

        public void Unsubscribe(Type messageType)
        {
            if (messageType == null) throw new ArgumentNullException("messageType");

            listener.Stop(messageType);

            logger.Info("Stopped listening for messages of type '{0}'", messageType.FullName);
        }

        public void UnsubscribeAll()
        {
            listener.StopAll();

            logger.Info("Stopped listening for messages of all types");
        }

        public string Send(IMessage message)
        {
            Validate(message);
            message.Headers.Remove(SystemHeaders.RequestResponse);

            return SendOneWay(message);
        }

        public string Send<T>(Action<T> messageInitializer)
            where T : IMessage
        {
            if (messageInitializer == null) throw new ArgumentNullException("messageInitializer");

            var message = (T) BusConfiguration.Configure.MessageCreator(typeof(T));
            messageInitializer(message);

            return Send(message);
        }

        public string SendBatch<T>(T[] messages)
            where T : IMessage
        {
            if (messages == null) throw new ArgumentNullException("messages");
            if (messages.Length == 0) return null;

            Type messageType = typeof(T);

            if (messageType.IsInterface || messageType.IsAbstract)
            {
                throw new ArgumentException("Message type cannot be an interface or an abstract class");
            }

            foreach (T message in messages)
            {
                Validate(message);
                message.Headers.Remove(SystemHeaders.RequestResponse);
            }

            return SendOneWay(messages);
        }

        public async Task<TResponse> SendAndReceive<TResponse>(IMessage message)
        {
            Validate(message);

            var task = Task<TResponse>.Factory.StartNew(m => CreateSendTwoWayTask<TResponse>((IMessage) m), message);
            return await task;
        }

        public async Task<TResponse> SendAndReceive<T, TResponse>(Action<T> messageInitializer)
            where T : IMessage
        {
            if (messageInitializer == null) throw new ArgumentNullException("messageInitializer");

            var message = (T) BusConfiguration.Configure.MessageCreator(typeof(T));
            messageInitializer(message);

            return await SendAndReceive<TResponse>(message);
        }

        public void Reply<TResponse>(IMessage message, TResponse value)
        {
            Validate(message);

            string messageId = message.Headers.Get(SystemHeaders.MessageID);

            if (String.IsNullOrWhiteSpace(messageId) || message.Headers.Get(SystemHeaders.RequestResponse) == null ||
                !message.Headers.Get(SystemHeaders.RequestResponse).Equals(Boolean.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("A message cannot be replied to");
            }

            if (String.Equals(message.Headers.Get(SystemHeaders.Responded), Boolean.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("A message can only be replied to once");
            }

            var response = new Response(messageId, value);
            var helper = new ResponseQueueHelper(this);

            using (var queue = helper.GetQueue(message.GetType()))
            {
                helper.Send(queue, response, false);
            }

            message.Headers[SystemHeaders.Responded] = Boolean.TrueString;

            logger.Info("Replied to a message of type '{0}', ID '{1}' with value '{2}'", message.GetType().FullName, messageId, value);
        }
        
        public string ForwardTo(IMessage message, string queueName, bool isRecoverable)
        {
            Validate(message);

            if (String.IsNullOrEmpty(queueName)) throw new ArgumentNullException("queueName");

            var helper = new ForwardedQueueHelper(this);

            using (var queue = helper.GetQueue(queueName))
            {
                return helper.Send(queue, message, isRecoverable);
            }
        }

        public bool Invalidate<T>(string messageId)
        {
            return Invalidate(typeof(T), messageId);
        }

        public bool Invalidate(Type messageType, string messageId)
        {
            if (messageType == null) throw new ArgumentNullException("messageType");
            if (String.IsNullOrEmpty(messageId)) throw new ArgumentNullException("messageId");

            MessageTypeValidator.Validate(messageType);

            Message queueMessage;
            var helper = new PendingQueueHelper(this);

            using (var queue = helper.GetQueue(messageType))
            {
                try
                {
                    queueMessage = queue.PeekById(messageId);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            if (queueMessage == null || !(queueMessage.Body is IMessage))
            {
                return false;
            }

            var faultHelper = new FaultQueueHelper(this);
            faultHelper.MoveToSubQueue(messageType, queueMessage);

            return true;
        }

        public void Purge<T>(PurgeTargets targets) where T : IMessage
        {
            Purge(typeof(T), targets);
        }

        public void Purge(Type messageType, PurgeTargets targets)
        {
            if (messageType == null) throw new ArgumentNullException("messageType");

            MessageTypeValidator.Validate(messageType);

            if ((targets & PurgeTargets.InputMessages) == PurgeTargets.InputMessages)
            {
                var helper = new InputQueueHelper(this);
                helper.Purge(messageType);

                logger.Debug("Input queue for messages of type '{0}' has been purged", messageType.FullName);
            }

            if ((targets & PurgeTargets.PendingMessages) == PurgeTargets.PendingMessages)
            {
                var helper = new PendingQueueHelper(this);
                helper.Purge(messageType);

                logger.Debug("Pending queue for messages of type '{0}' has been purged", messageType.FullName);
            }

            if ((targets & PurgeTargets.FaultMessages) == PurgeTargets.FaultMessages)
            {
                var helper = new FaultQueueHelper(this);
                helper.Purge(messageType);

                logger.Debug("Fault queue for messages of type '{0}' has been purged", messageType.FullName);
            }

            if ((targets & PurgeTargets.ResponseMessages) == PurgeTargets.ResponseMessages)
            {
                var helper = new ResponseQueueHelper(this);
                helper.Purge(messageType);

                logger.Debug("Response queue for messages of type '{0}' has been purged", messageType.FullName);
            }
        }

        public bool Remove<T>(string messageId)
        {
            return Remove(typeof(T), messageId);
        }

        public bool Remove(Type messageType, string messageId)
        {
            if (messageType == null) throw new ArgumentNullException("messageType");
            if (String.IsNullOrEmpty(messageId)) throw new ArgumentNullException("messageId");

            MessageTypeValidator.Validate(messageType);

            var helper = new PendingQueueHelper(this);

            using (var queue = helper.GetQueue(messageType))
            {
                try
                {
                    Message queueMessage = queue.ReceiveById(messageId);
                    return queueMessage != null && queueMessage.Body is IMessage;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public string Resend(IMessage message)
        {
            Validate(message);

            Type messageType = message.GetType();
            var messageId = message.Headers.Get(SystemHeaders.MessageID);

            if (String.IsNullOrWhiteSpace(messageId))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                                                                  "Message of type '{0}' cannot be resent.",
                                                                  messageType.FullName));
            }

            message.Headers[SystemHeaders.Sent] = DateTime.UtcNow.ToString("o");

            var helper = new InputQueueHelper(this);

            using (var queue = helper.GetQueue(message.GetType()))
            {
                return helper.Send(queue, message, true);
            }
        }

        public void ResendFaulted(Type messageType)
        {
            if (messageType == null) throw new ArgumentNullException("messageType");

            MessageTypeValidator.Validate(messageType);

            var helper = new FaultQueueHelper(this);
            var messageIdsToRemove = new List<string>();

            using (var queue = helper.GetQueue(messageType))
            {
                MessageEnumerator faultEnumerator = queue.GetMessageEnumerator2();

                while (faultEnumerator.MoveNext())
                {
                    Message faultMessage = faultEnumerator.Current;
                    if (faultMessage == null) continue;

                    var message = faultMessage.Body as IMessage;
                    if (message == null) continue;

                    message.Headers[SystemHeaders.MessageID] = faultMessage.Id;

                    try
                    {
                        Resend(message);
                        logger.Info("Re-sent faulted message of type '{0}', ID '{1}'", messageType.FullName, faultMessage.Id);
                    }
                    catch (Exception ex)
                    {
                        ((BusEvents) Events).OnFaultOccurred(faultMessage.Id, message, ex);
                    }

                    if (!messageIdsToRemove.Contains(faultMessage.Id))
                    {
                        messageIdsToRemove.Add(faultMessage.Id);
                    }
                }

                foreach (string messageIdToRemove in messageIdsToRemove)
                {
                    try
                    {
                        queue.ReceiveById(messageIdToRemove);
                    }
                    catch (Exception)
                    {
                        logger.Error("Faulted message of type '{0}', ID '{1}' could not be removed", messageType.FullName, messageIdToRemove);
                    }
                }
            }
        }
        
        private static void Validate(IMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.Headers == null) throw new ArgumentException("Message must have an initialized header collection", "message");
        }

        private string SendOneWay<T>(params T[] messages)
            where T : IMessage
        {
            foreach (T message in messages)
            {
                message.Headers[SystemHeaders.Sent] = DateTime.UtcNow.ToString("o");
                message.Headers[SystemHeaders.Sent] = DateTime.UtcNow.ToString("o");
            }

            var helper = new InputQueueHelper(this);

            string messageId;

            if (messages.Length > 1)
            {
                Type messageType = typeof(T);

                using (var queue = helper.GetQueue(messageType))
                {
                    messageId = helper.Send(queue, messages, true);
                }

                logger.Info("Sent a batch of {0} messages of type '{1}', ID '{2}'", messages.Length, messageType.FullName, messageId);
            }
            else
            {
                Type messageType = messages[0].GetType();

                using (var queue = helper.GetQueue(messageType))
                {
                    messageId = helper.Send(queue, messages[0], true);
                }

                logger.Info("Sent message of type '{0}', ID '{1}'", messageType.FullName, messageId);
            }

            return messageId;
        }

        private TResponse CreateSendTwoWayTask<TResponse>(IMessage message)
        {
            message.Headers[SystemHeaders.RequestResponse] = Boolean.TrueString;

            string messageId = SendOneWay(message);
            message.Headers[SystemHeaders.MessageID] = messageId;

            TimeSpan responseTimeout = Settings.ResponseTimeout;
            var saga = message as ISaga;

            if (saga != null && saga.Timeout > TimeSpan.Zero)
            {
                responseTimeout = saga.Timeout;
            }

            try
            {
                return GetResponse<TResponse>(messageId, message, responseTimeout);
            }
            catch (Exception ex)
            {
                ((BusEvents) Events).OnFaultOccurred(messageId, message, ex);
                return default(TResponse);
            }
        }

        private TResponse GetResponse<TResponse>(string messageId, IMessage message, TimeSpan timeout)
        {
            lock (syncRoot)
            {
                using (var timer = new Timer(timeout.TotalMilliseconds))
                {
                    bool timedOut = false;

                    timer.Elapsed += (sender, args) =>
                                     {
                                         ((Timer) sender).Stop();
                                         timedOut = true;
                                     };

                    timer.Start();

                    var helper = new ResponseQueueHelper(this);

                    while (!timedOut)
                    {
                        using (var queue = helper.GetQueue(message.GetType()))
                        using (MessageEnumerator responseEnumerator = queue.GetMessageEnumerator2())
                        {
                            while (responseEnumerator.MoveNext())
                            {
                                if (timedOut) break;

                                Message responseMessage = responseEnumerator.Current;
                                if (responseMessage == null) continue;

                                var response = responseMessage.Body as Response;
                                if (response == null) continue;
                                if (!String.Equals(messageId, response.MessageID)) continue;

                                responseEnumerator.RemoveCurrent();
                                return (TResponse) response.Body;
                            }
                        }

                        if (timedOut) break;
                    }
                }

                Advanced.Remove(message.GetType(), messageId);

                throw new TimeoutException("Response timeout expired");
            }
        }
    }
}
