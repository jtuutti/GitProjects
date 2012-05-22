using System;
using System.IO;
using System.Messaging;
using System.Text;
using Newtonsoft.Json;

namespace MessageBus.Msmq.Formatters
{
    /// <summary>
    /// Formatter class that serializes and deserializes objects into and from MessageQueue messages using Json. 
    /// </summary>
    public class JsonFormatter : IMessageFormatter
    {
        /// <summary>
        /// When implemented in a class, determines whether the formatter can deserialize the contents of the message.
        /// </summary>
        /// <returns>
        /// true if the formatter can deserialize the message; otherwise, false.
        /// </returns>
        /// <param name="message">The <see cref="T:System.Messaging.Message"/> to inspect.</param>
        public bool CanRead(Message message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.BodyStream == null || String.IsNullOrWhiteSpace(message.Label)) return false;

            return Type.GetType(message.Label, false) != null;
        }

        /// <summary>
        /// When implemented in a class, reads the contents from the given message and creates an object that contains data from the message.
        /// </summary>
        /// <returns>
        /// The deserialized message.
        /// </returns>
        /// <param name="message">The <see cref="T:System.Messaging.Message"/> to deserialize.</param>
        public object Read(Message message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.BodyStream == null) return null;

            var serializer = new JsonSerializer();

            using (var streamReader = new StreamReader(message.BodyStream, Encoding.UTF8))
            {
                var jsonReader = new JsonTextReader(streamReader);

                return serializer.Deserialize(jsonReader, Type.GetType(message.Label));
            }
        }

        /// <summary>
        /// When implemented in a class, serializes an object into the body of the message.
        /// </summary>
        /// <param name="message">The <see cref="T:System.Messaging.Message"/> that will contain the serialized object.</param>
        /// <param name="obj">The object to be serialized into the message.</param>
        public void Write(Message message, object obj)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (obj == null) throw new ArgumentNullException("obj");

            message.Label = obj.GetType().AssemblyQualifiedName ?? String.Empty;

            var bodyStream = new MemoryStream();
            var serializer = new JsonSerializer();

            var textWriter = new StreamWriter(bodyStream, Encoding.UTF8);
            serializer.Serialize(textWriter, obj);
            textWriter.Flush();

            message.BodyStream = bodyStream;
            message.BodyType = 0;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            return new JsonFormatter();
        }
    }
}
