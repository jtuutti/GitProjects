using System;
using System.Messaging;

namespace MessageBus.Msmq.Helpers
{
    internal sealed class PendingQueueHelper : QueueHelperBase
    {
        public const string SubQueueName = "pending";

        public PendingQueueHelper(MsmqBus bus) : base(bus)
        {
        }

        public override MessageQueue GetQueue(Type messageType)
        {
            if (messageType == null) throw new ArgumentNullException("messageType");

            string queueName = String.Concat(QueueNameHelper.GetName(messageType), ";", SubQueueName);

            return GetOrCreateQueue(messageType, queueName);
        }

        public void MoveToSubQueue(Type messageType, Message message)
        {
            var helper = new InputQueueHelper(bus);

            using (var queue = helper.GetQueue(messageType))
            {
                queue.MoveToSubQueue(SubQueueName, message);
            }
        }

        public void MoveFromSubQueue(Type messageType, Message message)
        {
            using (var pendingQueue = GetQueue(messageType))
            {
                pendingQueue.MoveFromSubQueue(message);
            }
        }
    }
}
