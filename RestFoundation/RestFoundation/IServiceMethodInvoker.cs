using System.Reflection;
using System.Web.Routing;

namespace RestFoundation
{
    public interface IServiceMethodInvoker
    {
        object Invoke(IRouteHandler handler, object service, MethodInfo method);
    }
}
