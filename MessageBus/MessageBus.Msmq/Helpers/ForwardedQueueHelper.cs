using System;
using System.Messaging;

namespace MessageBus.Msmq.Helpers
{
    internal class ForwardedQueueHelper : QueueHelperBase
    {
        public ForwardedQueueHelper(MsmqBus bus) : base(bus)
        {
        }

        public override MessageQueue GetQueue(Type messageType)
        {
            throw new NotSupportedException();
        }

        public MessageQueue GetQueue(string queueName)
        {
            if (String.IsNullOrEmpty(queueName)) throw new ArgumentNullException("queueName");

            return GetOrCreateQueue(queueName);
        }
    }
}
