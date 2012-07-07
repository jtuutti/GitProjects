using System;
using System.Runtime.Serialization;
using System.Security;
using System.Web;

namespace RestFoundation.Runtime
{
    [Serializable]
    public class ServiceUri : Uri
    {
        private readonly string m_serviceRelativeUrl;

        public ServiceUri(Uri currentUri, string serviceUrl) : base(currentUri != null ? currentUri.ToString() : null, UriKind.Absolute)
        {
            if (currentUri == null) throw new ArgumentNullException("currentUri");
            if (String.IsNullOrEmpty(serviceUrl)) throw new ArgumentNullException("serviceUrl");

            m_serviceRelativeUrl = serviceUrl.TrimStart('~', ' ').TrimEnd('/', ' ');

            ServiceUrl = new Uri(ToAbsoluteUrl(serviceUrl), UriKind.Absolute);
        }

        protected ServiceUri(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null) throw new ArgumentNullException("info");

            string serviceUrl = info.GetString("ServiceUrl");

            if (!String.IsNullOrEmpty(serviceUrl))
            {
                ServiceUrl = new Uri(serviceUrl, UriKind.Absolute);
            }
        }

        public Uri ServiceUrl { get; protected set; }

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

            string absoluteUrl = VirtualPathUtility.ToAbsolute(baseUrl, m_serviceRelativeUrl);

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

        [SecurityCritical]
        protected new virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext) 
        {
            if (serializationInfo == null) throw new ArgumentNullException("serializationInfo");

            base.GetObjectData(serializationInfo, streamingContext);

            serializationInfo.AddValue("ServiceUrl", ServiceUrl);
        }
    }
}
