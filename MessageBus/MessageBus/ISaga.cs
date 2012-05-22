using System;

namespace MessageBus
{
    /// <summary>
    /// Defines a message design for long running processes.
    /// </summary>
    public interface ISaga : IMessage
    {
        /// <summary>
        /// Gets the maximum execution time for the message to be processed.
        /// Sagas use pessimistic locking by the message bus to ensure exclusive
        /// handling.
        /// </summary>
        TimeSpan Timeout { get; }
    }
}
