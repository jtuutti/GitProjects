// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Specifies that the entire service contract should be hidden from the service proxy.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ProxyHiddenAttribute : Attribute
    {
    }
}
