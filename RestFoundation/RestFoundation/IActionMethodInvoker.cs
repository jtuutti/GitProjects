using System.Reflection;
using System.Web.Routing;

namespace RestFoundation
{
    public interface IActionMethodInvoker
    {
        object Invoke(IRouteHandler handler, object service, MethodInfo actionMethod);
    }
}
