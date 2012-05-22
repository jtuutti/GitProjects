using System;
using System.Runtime.Serialization;

namespace MessageBus
{
    /// <summary>
    /// Defines a message bus.
    /// </summary>
    public interface IBus
    {
        /// <summary>
        /// Gets an associated <see cref="IAdvancedOperations"/> instance contains advanced operations.
        /// </summary>
        IAdvancedOperations Advanced { get; }

        /// <summary>
        /// Gets an associated <see cref="IBusEvents"/> instance containing bus events.
        /// </summary>
        IBusEvents Events { get; }

        /// <summary>
        /// Gets an associated <see cref="BusSettings"/> instance containing settings for the bus.
        /// </summary>
        BusSettings Settings { get; }

        /// <summary>
        /// Subscribes to receive messages of the provided type on the bus.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <exception cref="InvalidOperationException">If the message type has already been subscribed.</exception>
        void Subscribe<T>() where T : IMessage;

        /// <summary>
        /// Subscribes to receive messages of provided types on the bus.
        /// </summary>
        /// <param name="messageTypes">The message types to subscribe.</param>
        /// <exception cref="ArgumentException">No message types provided.</exception>
        /// <exception cref="InvalidOperationException">If the message type has already been subscribed.</exception>
        void Subscribe(params Type[] messageTypes);

        /// <summary>
        /// Subscribes to receive messages of all handled types on the bus.
        /// </summary>
        void SubscribeAll();

        /// <summary>
        /// Unsubscribed from receiving messages of the provided type on the bus.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <exception cref="InvalidOperationException">If the message type has not been subscribed.</exception>
        void Unsubscribe<T>() where T : IMessage;

        /// <summary>
        /// Unsubscribed from receiving messages of the provided type on the bus.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <exception cref="InvalidOperationException">If the message type has not been subscribed.</exception>
        void Unsubscribe(Type messageType);

        /// <summary>
        /// Unsubscribed from receiving messages of all handled types on the bus.
        /// </summary>
        void UnsubscribeAll();

        /// <summary>
        /// Sends a message 1-way onto the bus.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The message id on the bus.</returns>
        string Send(IMessage message);

        /// <summary>
        /// Creates a message using the message builder and populates it with data
        /// from the provided delegate and sends the message 1-way onto the bus.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="messageInitializer">A delegate populating the created message with data.</param>
        /// <returns>The message id on the bus.</returns>
        string Send<T>(Action<T> messageInitializer) where T : IMessage;

        /// <summary>
        /// Sends a batch of messages onto the bus.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="messages">An array of messages.</param>
        /// <returns>The batch id on the bus.</returns>
        string SendBatch<T>(params T[] messages) where T : IMessage;

        /// <summary>
        /// Begins an asynchronous 2-way operation that sends a message onto the bus and returns a response.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callback">The asynchronous callback.</param>
        /// <returns>The <see cref="IAsyncResult"/> that identifies the posted asynchronous request.</returns>
        /// <exception cref="TimeoutException">If no active message handler sent a response within the specified timeout.</exception>
        /// <exception cref="SerializationException">If the response type is not serializable.</exception>
        IAsyncResult SendAsync(IMessage message, AsyncCallback callback);

        /// <summary>
        /// Begins an asynchronous 2-way operation that sends the newly initialized message onto the bus and returns a response.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="messageInitializer">A delegate populating the created message with data.</param>
        /// <param name="callback">The asynchronous callback.</param>
        /// <returns>The <see cref="IAsyncResult"/> that identifies the posted asynchronous request.</returns>
        /// <exception cref="TimeoutException">If no active message handler sent a response within the specified timeout.</exception>
        /// <exception cref="SerializationException">If the response type is not serializable.</exception>
        IAsyncResult SendAsync<T>(Action<T> messageInitializer, AsyncCallback callback) where T : IMessage;

        /// <summary>
        /// Completes a pending asynchronous 2-way operation.
        /// </summary>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="result">An <see cref="IAsyncResult"/> that stores state information for this asynchronous operation.</param>
        /// <returns>The message id on the bus.</returns>
        TResponse SendComplete<TResponse>(IAsyncResult result);

        /// <summary>
        /// Replies to a message that was received from the bus. This method should be called from message handlers.
        /// </summary>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="value">The response.</param>
        /// <exception cref="SerializationException">If the response type is not serializable.</exception>
        /// <exception cref="InvalidOperationException">
        /// If the message has not been sent through the message bus or more than 1 response for the same message id was sent.
        /// </exception>
        void Reply<TResponse>(IMessage message, TResponse value);
    }
}
