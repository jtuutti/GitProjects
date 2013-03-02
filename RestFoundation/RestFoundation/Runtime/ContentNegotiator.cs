// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string HtmlMediaType = "text/html";
        private const string JsonMediaType = "application/json";
        private const string XmlMediaType = "text/xml";

        private bool supportsJson, supportsXml;

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

            if (Rest.Configuration.Options.ForceDefaultMediaType && !String.IsNullOrWhiteSpace(Rest.Configuration.Options.DefaultMediaType))
            {
                return Rest.Configuration.Options.DefaultMediaType;
            }

            GetMediaTypeSupport();

            string acceptValue = GetAcceptedMediaType(request.QueryString.TryGet(AcceptOverrideQuery));

            if (!String.IsNullOrWhiteSpace(acceptValue))
            {
                return acceptValue;
            }

            acceptValue = GetAcceptedMediaType(request.Headers.AcceptType);

            if (!String.IsNullOrWhiteSpace(acceptValue))
            {
                return acceptValue;
            }

            acceptValue = GetAcceptedMediaType(Rest.Configuration.Options.DefaultMediaType);

            if (!String.IsNullOrWhiteSpace(acceptValue))
            {
                return acceptValue;
            }

            if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put || request.Method == HttpMethod.Patch)
            {
                acceptValue = GetAcceptedMediaType(request.Headers.ContentType);

                if (!String.IsNullOrWhiteSpace(acceptValue))
                {
                    return acceptValue;
                }
            }

            return request.IsAjax ? JsonMediaType : null;
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

            if (request.Method != HttpMethod.Get && request.Method != HttpMethod.Head)
            {
                return false;
            }

            string acceptedValue = request.QueryString.TryGet(AcceptOverrideQuery);

            if (String.IsNullOrEmpty(acceptedValue))
            {
                acceptedValue = request.Headers.Get("Accept");
            }

            var acceptTypeCollection = new AcceptValueCollection(acceptedValue);
            var mediaTypes = MediaTypeFormatterRegistry.GetMediaTypes();

            foreach (string acceptedType in acceptTypeCollection.AcceptedNames)
            {
                if (String.Equals(HtmlMediaType, acceptedType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (mediaTypes.Contains(acceptedType, StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return acceptTypeCollection.CanAccept(HtmlMediaType);
        }

        private void GetMediaTypeSupport()
        {
            var supportedMediaTypes = new HashSet<string>(MediaTypeFormatterRegistry.GetMediaTypes());

            supportsJson = supportedMediaTypes.Contains(JsonMediaType);
            supportsXml = supportedMediaTypes.Contains(XmlMediaType);
        }

        private string GetAcceptedMediaType(string acceptValue)
        {
            if (String.IsNullOrWhiteSpace(acceptValue))
            {
                return null;
            }

            string acceptedMediaType = new AcceptValueCollection(acceptValue).GetPreferredName();

            return IsValidAcceptedMediaType(ref acceptedMediaType) ? acceptedMediaType : null;
        }

        private bool IsValidAcceptedMediaType(ref string acceptValue)
        {
            if (String.IsNullOrWhiteSpace(acceptValue))
            {
                return false;
            }

            acceptValue = acceptValue.Trim();

            if ((acceptValue.Equals("*/*") || acceptValue.Equals("application/*", StringComparison.OrdinalIgnoreCase)) && supportsJson)
            {
                acceptValue = JsonMediaType;
                return true;
            }

            if (acceptValue.Equals("text/*", StringComparison.OrdinalIgnoreCase) && supportsXml)
            {
                acceptValue = XmlMediaType;
                return true;
            }

            return acceptValue.IndexOf('*') < 0;
        }
    }
}
