using System.Reflection;
using System.Web.Routing;

namespace RestFoundation
{
    public interface IServiceMethodInvoker
    {
        object Invoke(IRouteHandler handler, IServiceContext context, object service, MethodInfo method);
    }
}
