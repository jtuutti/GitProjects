using System.Reflection;

namespace RestFoundation
{
    public interface IServiceMethodInvoker
    {
        object Invoke(IRestHandler handler, object service, MethodInfo method);
    }
}
