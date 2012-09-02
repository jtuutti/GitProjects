using System;

namespace RestFoundation
{
    /// <summary>
    /// Respresents a REST service implementation that defines its own contract.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ServiceContractAttribute : Attribute
    {
    }
}
