using System;
using System.Web;

namespace RestFoundation.Runtime
{
    public sealed class ServiceUri : Uri
    {
        public ServiceUri(Uri uri, string serviceUrl) : base(uri.ToString(), UriKind.Absolute)
        {
            if (uri == null) throw new ArgumentNullException("uri");
            if (String.IsNullOrEmpty(serviceUrl)) throw new ArgumentNullException("serviceUrl");

            ServiceUrl = new Uri(ToAbsoluteUrl(serviceUrl), UriKind.Absolute);
        }

        public Uri ServiceUrl { get; private set; }

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

            string absoluteUrl = VirtualPathUtility.ToAbsolute(baseUrl);

            if (absoluteUrl.StartsWith("/"))
            {
                absoluteUrl = String.Concat(GetLeftPart(UriPartial.Authority), absoluteUrl);
            }

            if (urlParts.Length > 1)
            {
                absoluteUrl += ("?" + urlParts[1]);
            }

            return absoluteUrl;
        }
    }
}
