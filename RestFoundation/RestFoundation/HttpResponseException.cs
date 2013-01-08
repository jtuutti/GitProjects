// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;

namespace RestFoundation
{
    /// <summary>
    /// Represents an HTTP response exception. This is a special type of exception that is designed to stop
    /// the HTTP request and set a status. It does not get caught by the service behaviors.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
                     Justification = "Constructors with a nested exception do not make sense since this is a special type of exception for generating HTTP response")]
    [ExcludeFromCodeCoverage]
    public sealed class HttpResponseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class.
        /// </summary>
        public HttpResponseException() : this(HttpStatusCode.InternalServerError, String.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class with the HTTP status description.
        /// The status code is assumed to be 500 (internal server error).
        /// </summary>
        /// <param name="statusDescription">The status description.</param>
        public HttpResponseException(string statusDescription) : this(HttpStatusCode.InternalServerError, statusDescription)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class with the HTTP status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public HttpResponseException(HttpStatusCode statusCode) : this(statusCode, String.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class with the HTTP status code and description.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="statusDescription">The status description.</param>
        public HttpResponseException(HttpStatusCode statusCode, string statusDescription) :
            base(String.Format(CultureInfo.InvariantCulture, "Http Status Exception: ({0}) {1}", (int) statusCode, statusDescription ?? String.Empty))
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription ?? String.Empty;
        }

        private HttpResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            StatusCode = (HttpStatusCode) info.GetInt32("statusCode");
            StatusDescription = info.GetString("statusDescription");
        }

        /// <summary>
        /// Gets the associated HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Gets the associated HTTP status description.
        /// </summary>
        public string StatusDescription { get; private set; }

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("statusCode", (int) StatusCode);
            info.AddValue("statusDescription", StatusDescription);
        }
    }
}
