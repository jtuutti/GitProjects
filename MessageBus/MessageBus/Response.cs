using System;
using System.Xml.Serialization;

namespace MessageBus
{
    /// <summary>
    /// This class is used internally by the message bus to respond to 2-way communications.
    /// </summary>
    [Serializable, XmlRoot("Response")]
    public class Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        public Response()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class with the provided message id
        /// and response.
        /// </summary>
        public Response(string messageID, object body)
        {
            if (String.IsNullOrWhiteSpace(messageID)) throw new ArgumentNullException("messageID");

            MessageID = messageID;
            Body = body;
        }

        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        public string MessageID { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        public object Body { get; set; }
    }
}
