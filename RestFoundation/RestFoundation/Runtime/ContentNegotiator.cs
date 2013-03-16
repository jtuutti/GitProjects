// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        private const string JsonMediaType = "application/json";
        private const string XmlMediaType = "application/xml";
        private const string UserAgentVariableName = "HTTP_USER_AGENT";

        private static readonly Regex WildCardRegex = new Regex(@"^([a-zA-Z\-_\*/]+)\*$", RegexOptions.CultureInvariant | RegexOptions.Compiled);

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

            return (request.IsAjax && m_supportedMediaTypes.Contains(JsonMediaType)) ? JsonMediaType : null;
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

            Match wildCardMatch = WildCardRegex.Match(acceptValue.Trim());

            if (wildCardMatch.Success)
            {
                acceptValue = GetWildCardMediaType(request, wildCardMatch);
            }

            return m_supportedMediaTypes.Contains(acceptValue);
        }

        private string GetWildCardMediaType(IHttpRequest request, Match wildCardMatch)
        {
            string matchedTypePart = wildCardMatch.Groups[1].Value;

            if (matchedTypePart != "*/")
            {
                return MediaTypeFormatterRegistry.GetPrioritizedMediaTypes().FirstOrDefault(x => x.StartsWith(matchedTypePart, StringComparison.OrdinalIgnoreCase));
            }

            var contentTypeValue = GetAcceptedMediaType(request, request.Headers.ContentType);

            if (String.IsNullOrWhiteSpace(contentTypeValue))
            {
                return IsBrowserRequest(request) ? XmlMediaType : MediaTypeFormatterRegistry.GetPrioritizedMediaTypes().FirstOrDefault();
            }

            return contentTypeValue;
        }
    }
}
