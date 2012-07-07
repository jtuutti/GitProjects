using System.Web;
using System.Web.Routing;

namespace RestFoundation
{
    public interface IRestHandler : IRouteHandler, IHttpHandler
    {
        IServiceContext Context { get; }
        string ServiceUrl { get; }
        string ServiceContractTypeName { get; }
        string UrlTemplate { get; }
    }
}
