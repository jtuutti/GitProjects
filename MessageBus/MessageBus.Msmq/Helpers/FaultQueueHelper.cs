using System;
using System.Messaging;

namespace MessageBus.Msmq.Helpers
{
    internal sealed class FaultQueueHelper : QueueHelperBase
    {
        public const string SubQueueName = "fault";

        public FaultQueueHelper(MsmqBus bus) : base(bus)
        {
        }

        public override MessageQueue GetQueue(Type messageType)
        {
            string queueName = String.Concat(QueueNameHelper.GetName(messageType), ";", SubQueueName);

            return GetOrCreateQueue(messageType, queueName);
        }

        public void MoveToSubQueue(Type messageType, Message message)
        {
            var helper = new PendingQueueHelper(bus);

            using (var queue = helper.GetQueue(messageType))
            {
                queue.MoveToSubQueue(SubQueueName, message);
            }
        }
    }
}
