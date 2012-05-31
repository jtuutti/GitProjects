using System;

namespace MvcAlt
{
    public interface IBinder
    {
        string[] Bind(IHttpRequest request, string parameterName, Type resourceType, ref object resource);
    }
}
