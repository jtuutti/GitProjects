using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation
{
    public interface IHttpMethodResolver
    {
        IEnumerable<HttpMethod> Resolve(MethodInfo method);
    }
}
