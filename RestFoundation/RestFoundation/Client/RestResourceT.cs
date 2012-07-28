using System.Collections.Specialized;

namespace RestFoundation.Client
{
    /// <summary>
    /// Represents a REST HTTP resource with a body.
    /// This type of resource is commonly used for POST and PUT operations.
    /// </summary>
    /// <typeparam name="T">The resource body object type.</typeparam>
    public class RestResource<T> : RestResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestResource{T}"/> class.
        /// </summary>
        /// <param name="type">The resource type.</param>
        public RestResource(RestResourceType type) : base(type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestResource{T}"/> class.
        /// </summary>
        /// <param name="type">The resource type.</param>
        /// <param name="headers">A collection of HTTP headers to pass to the request.</param>
        public RestResource(RestResourceType type, NameValueCollection headers) : base(type, headers)
        {
        }

        /// <summary>
        /// Gets an object to use as the resource body.
        /// </summary>
        public T Body { get; set; }

        internal override object ResourceBody
        {
            get
            {
                return Body;
            }
        }
    }
}
