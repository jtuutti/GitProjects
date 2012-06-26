using System;
using System.Runtime.Serialization;

namespace RestFoundation.Test
{
    [Serializable]
    public class RouteAssertException : Exception
    {
        public RouteAssertException()
        {
        }

        public RouteAssertException(string message) : base(message)
        {
        }

        public RouteAssertException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RouteAssertException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
