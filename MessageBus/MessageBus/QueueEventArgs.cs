using System;

namespace MessageBus
{
    /// <summary>
    /// Contains an underlying queue reference used by the bus operation.
    /// </summary>
    [Serializable]
    public class QueueEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueEventArgs"/> class.
        /// </summary>
        /// <param name="queueName">A queue name.</param>
        public QueueEventArgs(string queueName)
        {
            if (String.IsNullOrEmpty(queueName)) throw new ArgumentNullException("queueName");

            QueueName = queueName;
        }

        /// <summary>
        /// Gets a queue name. 
        /// </summary>
        public string QueueName { get; protected set; }
    }
}
