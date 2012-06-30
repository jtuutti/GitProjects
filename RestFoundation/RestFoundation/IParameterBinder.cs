using System.Reflection;

namespace RestFoundation
{
    public interface IParameterBinder
    {
        object BindParameter(IServiceContext context, ParameterInfo parameter, out bool isResource);
    }
}
