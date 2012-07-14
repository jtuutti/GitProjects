using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a content type filtering behavior for a service or a service method.
    /// </summary>
    public class ContentTypeBehavior : ServiceBehavior
    {
        private readonly HashSet<string> m_contentTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTypeBehavior"/> class.
        /// </summary>
        /// <param name="contentTypes">The array of allowed content types.</param>
        public ContentTypeBehavior(params string[] contentTypes) : this(contentTypes != null ? contentTypes.AsEnumerable() : null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTypeBehavior"/> class.
        /// </summary>
        /// <param name="contentTypes">The sequence of allowed content types.</param>
        public ContentTypeBehavior(IEnumerable<string> contentTypes)
        {
            if (contentTypes == null) throw new ArgumentNullException("contentTypes");

            m_contentTypes = new HashSet<string>(contentTypes, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Called before a service method is executed.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        /// <param name="resource">The resource parameter value, if applicable, or null.</param>
        /// <returns>true to execute the service method; false to stop the request.</returns>
        public override bool OnMethodExecuting(IServiceContext context, object service, MethodInfo method, object resource)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (context.Request.Method != HttpMethod.Post && context.Request.Method != HttpMethod.Put && context.Request.Method != HttpMethod.Patch)
            {
                return true;
            }

            string contentType = context.Request.Headers.ContentType ?? String.Empty;

            if (!m_contentTypes.Contains(contentType))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType, "Content type is not specified or does not have an associated data formatter");
            }

            return true;
        }
    }
}
