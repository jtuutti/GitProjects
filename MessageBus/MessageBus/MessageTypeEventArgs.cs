using System;

namespace MessageBus
{
    /// <summary>
    /// Contains the message type being listened by the bus.
    /// </summary>
    [Serializable]
    public class MessageTypeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTypeEventArgs"/> class.
        /// </summary>
        /// <param name="messageType">A message type.</param>
        public MessageTypeEventArgs(Type messageType)
        {
            if (messageType == null) throw new ArgumentNullException("messageType");

            MessageType = messageType;
        }

        /// <summary>
        /// Gets the message type. 
        /// </summary>
        public Type MessageType { get; protected set; }
    }
}
