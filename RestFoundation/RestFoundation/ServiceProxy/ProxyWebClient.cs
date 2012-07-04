using System;
using System.Net;

namespace RestFoundation.ServiceProxy
{
    public sealed class ProxyWebClient : WebClient
    {
        private const string IfModifiedSinceHeader = "If-Modified-Since";

        public bool HeadOnly { get; set; }
        public bool Options { get; set; }
        public ProxyWebResponse WebResponse { get; private set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            DateTime modifiedSince = DateTime.MinValue;

            if (Headers[IfModifiedSinceHeader] != null)
            {
                DateTime.TryParse(Headers[IfModifiedSinceHeader], out modifiedSince);
                Headers.Remove(IfModifiedSinceHeader);
            }

            WebRequest request = base.GetWebRequest(address);
            if (request == null)
                return null;

            if (modifiedSince > DateTime.MinValue)
            {
                ((HttpWebRequest) request).IfModifiedSince = modifiedSince;
            }

            if (Options && String.Equals("GET", request.Method, StringComparison.OrdinalIgnoreCase))
            {
                request.Method = "OPTIONS";
            }
            else if (HeadOnly && String.Equals("GET", request.Method, StringComparison.OrdinalIgnoreCase))
            {
                request.Method = "HEAD";
            }

            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            if (response == null)
                return null;

            WebResponse = new ProxyWebResponse(response);
            return WebResponse;
        }
    }
}
