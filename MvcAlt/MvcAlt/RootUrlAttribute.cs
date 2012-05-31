using System;

namespace MvcAlt
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class RootUrlAttribute : UrlAttribute
    {
        public RootUrlAttribute(params HttpVerb[] verbs) : base(String.Empty, verbs)
        {
        }
    }
}