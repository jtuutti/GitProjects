using System;
using System.Web;
using RestFoundation.Collections.Specialized;

namespace RestFoundation.Runtime
{
    public class BrowserDetector : IBrowserDetector
    {
        public virtual bool IsBrowserRequest(HttpRequestBase request)
        {
            if (request == null) throw new ArgumentNullException("request");

            if (!"GET".Equals(request.HttpMethod, StringComparison.OrdinalIgnoreCase) &&
                !"HEAD".Equals(request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string browser = request.Browser != null ? request.Browser.Browser : null;
            string[] acceptTypes = request.AcceptTypes;

            if (String.IsNullOrWhiteSpace(browser) || acceptTypes == null || acceptTypes.Length == 0)
            {
                return false;
            }

            string acceptedValue = request.QueryString["X-Accept-Override"];

            if (String.IsNullOrEmpty(acceptedValue))
            {
                acceptedValue = request.Headers.Get("Accept");
            }

            var acceptTypeCollection = new AcceptValueCollection(acceptedValue);

            if (acceptTypeCollection.AcceptedNames.Count == 0)
            {
                return false;
            }

            string[] contentTypes = DataFormatterRegistry.GetContentTypes();

            for (int i = 0; i < contentTypes.Length; i++)
            {
                if (String.Equals(contentTypes[i], acceptTypeCollection.GetPreferredName(), StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return acceptTypeCollection.CanAccept("text/html");
        }
    }
}
