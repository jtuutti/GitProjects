using System;

namespace RestFoundation.ServiceProxy
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ProxyHttpsOnlyAttribute : Attribute
    {
        public ProxyHttpsOnlyAttribute()
        {
            Port = 443;
        }

        public int Port { get; set; }
    }
}
