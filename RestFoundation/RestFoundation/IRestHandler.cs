using System.Web;
using System.Web.Routing;

namespace RestFoundation
{
    public interface IRestHandler : IRouteHandler, IHttpHandler
    {
        IServiceContext Context { get; }
    }
}
