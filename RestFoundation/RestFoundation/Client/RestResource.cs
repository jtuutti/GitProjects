// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Specialized;

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
        public RestResource(RestResourceType type) : this(type, new NameValueCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestResource"/> class.
        /// </summary>
        /// <param name="type">The resource type.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        public RestResource(RestResourceType type, NameValueCollection headers)
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
            Headers = new NameValueCollection();
        }

        /// <summary>
        /// Gets the resource type.
        /// </summary>
        public RestResourceType Type { get; protected internal set; }

        /// <summary>
        /// Gets the collection of associated HTTP headers.
        /// </summary>
        public NameValueCollection Headers { get; protected set; }

        internal virtual object ResourceBody
        {
            get
            {
                return null;
            }
        }
    }
}
