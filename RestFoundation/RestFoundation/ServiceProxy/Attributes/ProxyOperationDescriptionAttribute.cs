using System;

namespace RestFoundation.ServiceProxy.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ProxyOperationDescriptionAttribute : Attribute
    {
        public ProxyOperationDescriptionAttribute(string description)
        {
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

            Description = description;
        }

        internal string Description { get; private set; }
    }
}
