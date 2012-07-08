using System;

namespace RestFoundation.ServiceProxy
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ProxyHiddenOperationAttribute : Attribute
    {
    }
}
