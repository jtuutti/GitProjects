// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using RestFoundation.Collections.Specialized;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the default content negotiator that determines an accepted media type from
    /// the current HTTP request headers or query string data.
    /// </summary>
    public class ContentNegotiator : IContentNegotiator
    {
        private const string AcceptOverrideQuery = "X-Accept-Override";
        private const string ApplicationXmlMediaType = "application/xml";
        private const string JsonMediaType = "application/json";
        private const string TextXmlMediaType = "text/xml";
        private const string UserAgentVariableName = "HTTP_USER_AGENT";

        private ICollection<string> m_supportedMediaTypes;

        /// <summary>
        /// Gets the preferred accepted media type from the provided HTTP request.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>A <see cref="string"/> containing the preferred media type.</returns>
        public virtual string GetPreferredMediaType(IHttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            PopulateSupportedMediaTypes();

            if (Rest.Configuration.Options.ForceDefaultMediaType && m_supportedMediaTypes.Contains(Rest.Configuration.Options.DefaultMediaType))
            {
                return Rest.Configuration.Options.DefaultMediaType;
            }

            string acceptValue = GetAcceptedMediaType(request, request.QueryString.TryGet(AcceptOverrideQuery));

            if (!String.IsNullOrWhiteSpace(acceptValue))
            {
                return acceptValue;
            }

            foreach (string acceptedType in request.Headers.AcceptTypes)
            {
                acceptValue = GetAcceptedMediaType(request, acceptedType);

                if (!String.IsNullOrWhiteSpace(acceptValue))
                {
                    return acceptValue;
                }
            }

            acceptValue = GetAcceptedMediaType(request, Rest.Configuration.Options.DefaultMediaType);

            if (!String.IsNullOrWhiteSpace(acceptValue))
            {
                return acceptValue;
            }

            if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put || request.Method == HttpMethod.Patch)
            {
                acceptValue = GetAcceptedMediaType(request, request.Headers.ContentType);

                if (!String.IsNullOrWhiteSpace(acceptValue))
                {
                    return acceptValue;
                }
            }

            return request.IsAjax && m_supportedMediaTypes.Contains(JsonMediaType) ? JsonMediaType : null;
        }

        /// <summary>
        /// Returns a <see cref="bool"/> value indicating whether the HTTP request
        /// came from a web browser directly.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>
        /// true if the HTTP request came from a web browser; otherwise, false.
        /// </returns>
        public virtual bool IsBrowserRequest(IHttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            string userAgent = request.ServerVariables.Get(UserAgentVariableName);

            if (userAgent == null)
            {
                return false;
            }

            return userAgent.IndexOf("Mozilla", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   userAgent.IndexOf("MSIE", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   userAgent.IndexOf("Opera", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   userAgent.IndexOf("WebKit", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void PopulateSupportedMediaTypes()
        {
            if (m_supportedMediaTypes == null)
            {
                m_supportedMediaTypes = new HashSet<string>(MediaTypeFormatterRegistry.GetMediaTypes());
            }
        }

        private string GetAcceptedMediaType(IHttpRequest request, string acceptValue)
        {
            if (String.IsNullOrWhiteSpace(acceptValue))
            {
                return null;
            }

            string acceptedMediaType = new AcceptValueCollection(acceptValue).GetPreferredName();

            return IsValidAcceptedMediaType(request, ref acceptedMediaType) ? acceptedMediaType : null;
        }

        private bool IsValidAcceptedMediaType(IHttpRequest request, ref string acceptValue)
        {
            if (String.IsNullOrWhiteSpace(acceptValue))
            {
                return false;
            }

            acceptValue = acceptValue.Trim();

            if (acceptValue.Equals("*/*"))
            {
                var contentTypeValue = GetAcceptedMediaType(request, request.Headers.ContentType);

                if (String.IsNullOrWhiteSpace(contentTypeValue))
                {
                    acceptValue = IsBrowserRequest(request) ? ApplicationXmlMediaType : JsonMediaType;
                }
                else
                {
                    acceptValue = contentTypeValue;
                }
            }
            else if (acceptValue.Equals("application/*", StringComparison.OrdinalIgnoreCase))
            {
                acceptValue = JsonMediaType;
            }
            else if (acceptValue.Equals("text/*", StringComparison.OrdinalIgnoreCase))
            {
                acceptValue = TextXmlMediaType;
            }

            return m_supportedMediaTypes.Contains(acceptValue);
        }
    }
}
