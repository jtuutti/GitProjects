using System.Collections.Generic;

namespace MessageBus
{
    /// <summary>
    /// Defines a message type for a message bus.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets a dictionary of message headers.
        /// </summary>
        IDictionary<string, string> Headers { get; }
    }
}
