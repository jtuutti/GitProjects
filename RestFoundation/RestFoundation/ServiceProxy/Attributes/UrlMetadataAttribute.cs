using System;

namespace RestFoundation.ServiceProxy.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class UrlMetadataAttribute : Attribute
    {
        public UrlMetadataAttribute(string description)
        {
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

            Description = description;
        }

        public string Description { get; private set; }
        public string RelativeUrl { get; set; }
    }
}
