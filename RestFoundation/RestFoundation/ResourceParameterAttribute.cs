using System;

namespace RestFoundation
{
    /// <summary>
    /// Represents a service contract method parameter attribute that indicates that the parameter
    /// is used to bind the resource for POST, PATCH and PUT HTTP methods. Only one resource parameter
    /// is allowed per a service method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class ResourceParameterAttribute : Attribute
    {
    }
}
