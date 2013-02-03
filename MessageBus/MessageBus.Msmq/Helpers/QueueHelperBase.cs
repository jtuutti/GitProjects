using System;
using System.Globalization;
using System.Messaging;
using System.Text.RegularExpressions;
using System.Threading;
using MessageBus.Msmq.Formatters;

namespace MessageBus.Msmq.Helpers
{
    internal abstract class QueueHelperBase
    {
        private const string PrivateQueuePrefix = @".\private$\";

        protected readonly Regex queueNameRegEx = new Regex(@"^[a-zA-Z0-9\.;]{3,50}$", RegexOptions.Compiled | RegexOptions.Singleline);
        protected readonly MsmqBus bus;

        protected QueueHelperBase(MsmqBus bus)
        {
            if (bus == null) throw new ArgumentNullException("bus");

            this.bus = bus;
        }

        public abstract MessageQueue GetQueue(Type messageType);

        public void Purge(Type messageType)
        {
            using (var queue = GetQueue(messageType))
            {
                queue.Purge();
            }
        }

        public string Send(MessageQueue queue, object body, bool isRecoverable)
        {
            using (var queueMessage = new Message(body))
            {
                queueMessage.Formatter = new JsonFormatter();
                queueMessage.Recoverable = isRecoverable;

                SendOrRetry(queue, queueMessage);

                return queueMessage.Id;
            }
        }

        protected MessageQueue GetOrCreateQueue(Type messageType, string queueName)
        {
            if (String.IsNullOrWhiteSpace(queueName))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                                                                  "Message of type '{0}' does not have a queue name assigned.",
                                                                  messageType.Name));
            }

            if (!queueNameRegEx.IsMatch(queueName))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, 
                                                                  "Message of type '{0}' does not have valid queue name assigned. It must only contain numbers, digits or dots and be from 3 to 50 characters long.",
                                                                  messageType.Name));
            }

            return GetOrCreateQueue(queueName);
        }

        protected MessageQueue GetOrCreateQueue(string queueName)
        {
            string fullQueueName = String.Concat(PrivateQueuePrefix, queueName.Trim());
            MessageQueue queue;

            if (!MessageQueue.Exists(fullQueueName))
            {
                queue = MessageQueue.Create(fullQueueName);
                queue.SetPermissions("Administrators", MessageQueueAccessRights.FullControl);
                queue.SetPermissions("NETWORK SERVICE", MessageQueueAccessRights.GenericRead | MessageQueueAccessRights.GenericWrite);

                ((MsmqBus.BusEvents) bus.Events).OnQueueCreated(queueName);
            }
            else
            {
                queue = new MessageQueue(fullQueueName);
            }

            queue.Formatter = new JsonFormatter();

            return queue;
        }

        private void SendOrRetry(MessageQueue queue, Message queueMessage, int attemptNumber = 1)
        {
            if (attemptNumber > bus.Settings.SendAttempts) return;

            try
            {
                queue.Send(queueMessage);
            }
            catch (MessageQueueException)
            {
                if (attemptNumber == bus.Settings.SendAttempts) throw;

                Thread.Sleep(bus.Settings.SendAttemptTimeout);
                SendOrRetry(queue, queueMessage, attemptNumber + 1);
            }
        }
    }
}
