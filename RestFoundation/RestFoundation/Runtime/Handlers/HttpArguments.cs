// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System.Web;

namespace RestFoundation.Runtime.Handlers
{
    internal sealed class HttpArguments
    {
        public HttpArguments(HttpContext context, ServiceMethodLocatorData serviceMethodData)
        {
            Context = context;
            ServiceMethodData = serviceMethodData;
        }

        public HttpContext Context { get; private set; }
        public ServiceMethodLocatorData ServiceMethodData { get; private set; }
    }
}
