using System.Reflection;

namespace RestFoundation
{
    public interface IParameterBinder
    {
        object BindParameter(ParameterInfo parameter, out bool isResource);
    }
}
