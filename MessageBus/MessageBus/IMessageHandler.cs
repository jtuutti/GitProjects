using System;

namespace MessageBus
{
    /// <summary>
    /// Defines a handler to handle a specific message type.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Gets the message type handled.
        /// </summary>
        Type MessageType { get; }

        /// <summary>
        /// Handles the provided message.
        /// </summary>
        /// <param name="message">The message to handle</param>
        /// <returns>
        /// true if the message needs to be removed from the bus; false to try to handle the message again
        /// </returns>
        bool Handle(IMessage message);
    }
}
