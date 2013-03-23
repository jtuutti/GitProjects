// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Net;

namespace RestFoundation.Client
{
    /// <summary>
    /// Represents a REST HTTP resource without a body.
    /// This type of resource is commonly used for GET and DELETE operations.
    /// </summary>
    public class RestResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestResource"/> class.
        /// </summary>
        /// <param name="type">The resource type.</param>
        public RestResource(RestResourceType type) : this(type, new WebHeaderCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestResource"/> class.
        /// </summary>
        /// <param name="type">The resource type.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        public RestResource(RestResourceType type, WebHeaderCollection headers)
        {
            if (!Enum.IsDefined(typeof(RestResourceType), type))
            {
                throw new ArgumentOutOfRangeException("type");
            }

            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            Type = type;
            Headers = headers;
        }

        internal RestResource()
        {
            Headers = new WebHeaderCollection();
        }

        /// <summary>
        /// Gets or sets an XML namespace for the resource.
        /// </summary>
        public string XmlNamespace { get; set; }

        /// <summary>
        /// Gets the collection of associated HTTP headers.
        /// </summary>
        public WebHeaderCollection Headers { get; protected set; }

        /// <summary>
        /// Gets the resource type.
        /// </summary>
        public RestResourceType Type { get; protected internal set; }

        /// <summary>
        /// Gets the last HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; protected internal set; }

        /// <summary>
        /// Gets the last HTTP status description.
        /// </summary>
        public string StatusDescription { get; protected internal set; }

        internal virtual object ResourceBody
        {
            get
            {
                return null;
            }
        }
    }
}
