using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace RestFoundation.Behaviors
{
    public class ContentTypeBehavior : ServiceBehavior
    {
        private readonly HashSet<string> m_contentTypes;

        public ContentTypeBehavior(params string[] contentTypes)
        {
            if (contentTypes == null) throw new ArgumentNullException("contentTypes");

            m_contentTypes = new HashSet<string>(contentTypes, StringComparer.OrdinalIgnoreCase);
        }

        public override void OnActionBinding(object service, MethodInfo actionMethod)
        {
            if (Request.Method != HttpMethod.Post && Request.Method != HttpMethod.Put && Request.Method != HttpMethod.Patch)
            {
                return;
            }

            string contentType = Request.Headers.ContentType ?? String.Empty;

            if (!m_contentTypes.Contains(contentType))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType, "Content type is not specified or not allowed");
            }
        }
    }
}
