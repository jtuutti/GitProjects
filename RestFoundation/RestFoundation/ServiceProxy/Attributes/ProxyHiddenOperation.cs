using System;

namespace RestFoundation.ServiceProxy.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ProxyHiddenOperation : Attribute
    {
    }
}
