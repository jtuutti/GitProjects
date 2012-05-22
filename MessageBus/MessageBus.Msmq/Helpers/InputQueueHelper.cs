using System;
using System.Messaging;

namespace MessageBus.Msmq.Helpers
{
    internal sealed class InputQueueHelper : QueueHelperBase
    {
        public InputQueueHelper(MsmqBus bus) : base(bus)
        {
        }

        public override MessageQueue GetQueue(Type messageType)
        {
            if (messageType == null) throw new ArgumentNullException("messageType");

            string queueName = QueueNameHelper.GetName(messageType);

            return GetOrCreateQueue(messageType, queueName);
        }
    }
}
