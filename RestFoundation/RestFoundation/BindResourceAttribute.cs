using System;

namespace RestFoundation
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class BindResourceAttribute : Attribute
    {
    }
}
