// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy decorator attribute that indicates that the service contract
    /// or a service method is IP restricted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ProxyIPFilteredAttribute : Attribute
    {
    }
}
