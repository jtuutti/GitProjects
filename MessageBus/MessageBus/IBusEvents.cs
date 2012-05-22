using System;

namespace MessageBus
{
    /// <summary>
    /// Defines message bus events.
    /// </summary>
    public interface IBusEvents
    {
        /// <summary>
        /// Occurs when a message has been delivered due to an operation fault. This event will only
        /// be called while the client is subscribed for messages.
        /// </summary>
        event EventHandler<FaultEventArgs> FaultOccurred;

        /// <summary>
        /// Occurs when an underlying queue has been created.
        /// </summary>
        event EventHandler<QueueEventArgs> QueueCreated;

        /// <summary>
        /// Occurs when the bus starts listening for messages of a specific type.
        /// </summary>
        event EventHandler<MessageTypeEventArgs> Listening;
    }
}
