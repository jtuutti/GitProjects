using System;
using System.Collections.Generic;

namespace MessageBus
{
    [Serializable]
    public abstract class MessageBase : IMessage
    {
        protected MessageBase()
        {
            Headers = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        protected MessageBase(string queueName)
        {
            if (String.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentNullException("queueName");
            }
        }

        public IDictionary<string, string> Headers
        {
            get;
            private set;
        }
    }
}
