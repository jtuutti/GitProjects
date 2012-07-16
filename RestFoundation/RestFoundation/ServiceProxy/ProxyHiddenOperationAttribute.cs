using System;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy decorator attribute that hides service methods from the
    /// service proxy UI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ProxyHiddenOperationAttribute : Attribute
    {
    }
}
