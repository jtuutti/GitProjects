using System;

namespace MessageBus
{
    /// <summary>
    /// Constains fault data that happened while delivering or handling the message.
    /// </summary>
    [Serializable]
    public class FaultEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FaultEventArgs"/> class.
        /// </summary>
        /// <param name="messageId">A message id in the queue.</param>
        /// <param name="message">A message.</param>
        /// <param name="faultException">A fault exception.</param>
        public FaultEventArgs(String messageId, IMessage message, Exception faultException)
        {
            if (messageId == null) throw new ArgumentNullException("messageId");
            if (faultException == null) throw new ArgumentNullException("faultException");

            MessageID = messageId;
            Message = message;
            FaultException = faultException;
        }

        /// <summary>
        /// Gets the message id in the queue.
        /// </summary>
        public string MessageID { get; protected set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public IMessage Message { get; protected set; }

        /// <summary>
        /// Gets an exception that occured during the bus fault.
        /// </summary>
        public Exception FaultException { get; protected set; }
    }
}
