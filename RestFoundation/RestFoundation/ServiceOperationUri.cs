using System;
using System.Runtime.Serialization;
using System.Security;
using System.Web;

namespace RestFoundation
{
    /// <summary>
    /// Represents a service operation URL.
    /// </summary>
    [Serializable]
    public class ServiceOperationUri : Uri
    {
        private readonly string m_relativeUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceOperationUri"/> class.
        /// </summary>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="operationUrl">The operation URL</param>
        public ServiceOperationUri(Uri serviceUrl, string operationUrl) : base(serviceUrl != null ? serviceUrl.ToString() : null, UriKind.Absolute)
        {
            if (serviceUrl == null) throw new ArgumentNullException("serviceUrl");
            if (String.IsNullOrEmpty(operationUrl)) throw new ArgumentNullException("operationUrl");

            m_relativeUrl = operationUrl.TrimStart('~', ' ').TrimEnd('/', ' ');

            OperationUrl = new Uri(ToAbsoluteUrl(operationUrl), UriKind.Absolute);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceOperationUri"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected ServiceOperationUri(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null) throw new ArgumentNullException("info");

            string serviceUrl = info.GetString("ServiceUrl");

            if (!String.IsNullOrEmpty(serviceUrl))
            {
                OperationUrl = new Uri(serviceUrl, UriKind.Absolute);
            }
        }

        /// <summary>
        /// Gets the full service operation URL.
        /// </summary>
        public Uri OperationUrl { get; protected set; }

        /// <summary>
        /// Returns a converted relative URL as an absolute URL.
        /// </summary>
        /// <param name="url">The relative URL.</param>
        /// <returns>A corresponding absolute URL.</returns>
        public string ToAbsoluteUrl(string url)
        {
            if (url == null) throw new ArgumentNullException("url");

            if (IsWellFormedUriString(url, UriKind.Absolute))
            {
                return url;
            }

            if (String.IsNullOrWhiteSpace(url))
            {
                url = "~/";
            }

            string[] urlParts = url.Split(new[] { '?' }, 2);
            string baseUrl = urlParts[0];

            if (!VirtualPathUtility.IsAbsolute(baseUrl))
            {
                baseUrl = VirtualPathUtility.Combine("~", baseUrl);
            }

            string absoluteUrl = !String.IsNullOrEmpty(m_relativeUrl) ? VirtualPathUtility.ToAbsolute(baseUrl, m_relativeUrl) : VirtualPathUtility.ToAbsolute(baseUrl);

            if (absoluteUrl.StartsWith("/", StringComparison.Ordinal))
            {
                absoluteUrl = String.Concat(GetLeftPart(UriPartial.Authority), absoluteUrl);
            }

            if (urlParts.Length > 1)
            {
                absoluteUrl += ("?" + urlParts[1]);
            }

            return absoluteUrl;
        }

        /// <summary>
        /// Sets the <see cref="ServiceOperationUri"/> with information about the exception.
        /// </summary>
        /// <param name="serializationInfo">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="streamingContext">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <filterpriority>2</filterpriority>
        [SecurityCritical]
        protected new virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext) 
        {
            if (serializationInfo == null) throw new ArgumentNullException("serializationInfo");

            base.GetObjectData(serializationInfo, streamingContext);

            serializationInfo.AddValue("ServiceUrl", OperationUrl);
        }
    }
}
