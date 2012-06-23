using System;
using System.Runtime.Serialization;

namespace RestFoundation
{
    [Serializable]
    public class ObjectActivationException : Exception
    {
        public ObjectActivationException()
        {
        }

        public ObjectActivationException(string message) : base(message)
        {
        }

        public ObjectActivationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ObjectActivationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
