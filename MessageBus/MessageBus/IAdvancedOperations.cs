using System;

namespace MessageBus
{
    /// <summary>
    /// Defines advanced message bus operations.
    /// </summary>
    public interface IAdvancedOperations
    {
        /// <summary>
        /// Forwards a message to a queue with the provided name. This method is usually used for auditing purposes.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="queueName">The queue name to forward the message to.</param>
        /// <param name="isRecoverable">A <see cref="bool"/> indicating whether the message should be recoverable in the queue.</param>
        /// <returns>The message id in the forwarded queue.</returns>
        string ForwardTo(IMessage message, string queueName, bool isRecoverable);

        /// <summary>
        /// Marks a message as faulted and moves it the error queue on the bus.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="messageId">The message id.</param>
        /// <returns>true if the message has been invalidated; false otherwise</returns>
        bool Invalidate<T>(string messageId);

        /// <summary>
        /// Marks a message as faulted and moves it the error queue on the bus.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="messageId">The message id.</param>
        /// <returns>true if the message has been invalidated; false otherwise</returns>
        bool Invalidate(Type messageType, string messageId);

        /// <summary>
        /// Purges all the messages for a message type.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="targets">Message targets to purge.</param>
        void Purge<T>(PurgeTargets targets) where T : IMessage;

        /// <summary>
        /// Purges all the messages for a message type.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="targets">Message targets to purge.</param>
        void Purge(Type messageType, PurgeTargets targets);

        /// <summary>
        /// Removes a message from the bus.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="messageId">The message id.</param>
        /// <returns>true if the message has been removed; false otherwise</returns>
        bool Remove<T>(string messageId);

        /// <summary>
        /// Removes the message from the bus.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="messageId">The message id.</param>
        /// <returns>true if the message has been removed; false otherwise</returns>
        bool Remove(Type messageType, string messageId);

        /// <summary>
        /// Resends a message in a corresponding input queue on the bus.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The message id in the input queue.</returns>
        string Resend(IMessage message);

        /// <summary>
        /// Resends faulted messages of the provided type.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        void ResendFaulted(Type messageType);
    }
}
