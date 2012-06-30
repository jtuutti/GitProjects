using System;

namespace RestFoundation
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class BindResourceAttribute : Attribute
    {
    }
}
