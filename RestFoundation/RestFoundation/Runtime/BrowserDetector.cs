using System;
using System.Linq;
using System.Web;
using RestFoundation.Collections.Specialized;

namespace RestFoundation.Runtime
{
    public class BrowserDetector : IBrowserDetector
    {
        private const string HtmlContentType = "text/html";

        public virtual bool IsBrowserRequest(HttpRequestBase request)
        {
            if (request == null) throw new ArgumentNullException("request");

            if (!"GET".Equals(request.HttpMethod, StringComparison.OrdinalIgnoreCase) &&
                !"HEAD".Equals(request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string acceptedValue = request.QueryString["X-Accept-Override"];

            if (String.IsNullOrEmpty(acceptedValue))
            {
                acceptedValue = request.Headers.Get("Accept");
            }

            var acceptTypeCollection = new AcceptValueCollection(acceptedValue);
            var contentTypes = DataFormatterRegistry.GetContentTypes();

            foreach (string acceptedType in acceptTypeCollection.AcceptedNames)
            {
                if (String.Equals(HtmlContentType, acceptedType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (contentTypes.Contains(acceptedType, StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return acceptTypeCollection.CanAccept(HtmlContentType);
        }
    }
}
