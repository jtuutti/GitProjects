using System;

namespace RestFoundation.ServiceProxy
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

        public string Description { get; private set; }
    }
}
