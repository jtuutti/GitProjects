using System;

namespace RestFoundation.ServiceProxy
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ProxyIPFilteredAttribute : Attribute
    {
    }
}
