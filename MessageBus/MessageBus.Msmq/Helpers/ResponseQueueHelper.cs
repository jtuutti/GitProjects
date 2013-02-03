using System;
using System.Globalization;
using System.Messaging;

namespace MessageBus.Msmq.Helpers
{
    internal sealed class ResponseQueueHelper : QueueHelperBase
    {
        public ResponseQueueHelper(MsmqBus bus) : base(bus)
        {
        }

        public override MessageQueue GetQueue(Type messageType)
        {
            if (messageType == null) throw new ArgumentNullException("messageType");

            string queueName = String.Format(CultureInfo.InvariantCulture, "{0}.response", QueueNameHelper.GetName(messageType));

            return GetOrCreateQueue(messageType, queueName);
        }
    }
}
