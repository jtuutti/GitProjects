// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Net;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy decorator attribute that specifies additional HTTP status codes
    /// that could be returned by the service method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class ProxyStatusCodeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyStatusCodeAttribute"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="condition">The condition when the status code may occur.</param>
        public ProxyStatusCodeAttribute(HttpStatusCode statusCode, string condition)
        {
            if (String.IsNullOrEmpty(condition))
            {
                throw new ArgumentNullException("condition");
            }

            StatusCode = statusCode;
            Condition = condition;
        }

        /// <summary>
        /// Gets the condition under which the HTTP status code may occur.
        /// </summary>
        public string Condition { get; private set; }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }
    }
}
