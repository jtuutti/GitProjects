// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using RestFoundation.Results;

namespace RestFoundation.Runtime.Handlers
{
    internal sealed class HtmlRouteHandler : HttpTaskAsyncHandler, IRouteHandler
    {
        private readonly string m_virtualUrl;

        public HtmlRouteHandler(string virtualUrl)
        {
            if (virtualUrl == null)
            {
                throw new ArgumentNullException("virtualUrl");
            }

            if (!virtualUrl.TrimStart().StartsWith("~", StringComparison.Ordinal))
            {
                throw new ArgumentException(Resources.Global.InvalidVirtualUrl, "virtualUrl");
            }

            m_virtualUrl = virtualUrl.Trim();
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        public override Task ProcessRequestAsync(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var serviceContext = Rest.Configuration.ServiceLocator.GetService<IServiceContext>();

            var result = new HtmlFileResult
            {
                FilePath = serviceContext.MapPath(m_virtualUrl)
            };

            serviceContext.Response.CancellationTokenSource = new CancellationTokenSource();

            return result.ExecuteAsync(serviceContext, serviceContext.Response.CancellationTokenSource.Token);
        }
    }
}
