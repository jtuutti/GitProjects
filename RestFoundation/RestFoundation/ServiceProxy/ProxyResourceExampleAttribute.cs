using System;

namespace RestFoundation.ServiceProxy
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ProxyResourceExampleAttribute : Attribute
    {
        private Type m_requestExampleType;
        private Type m_responseExampleType;

        public Type RequestExampleType
        {
            get
            {
                return m_requestExampleType;
            }
            set
            {
                ValidateResourceType(value);

                m_requestExampleType = value;
            }
        }

        public Type ResponseExampleType
        {
            get
            {
                return m_responseExampleType;
            }
            set
            {
                ValidateResourceType(value);

                m_responseExampleType = value;
            }
        }

        private static void ValidateResourceType(Type value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (!value.IsClass || value.IsAbstract || !typeof(IResourceExample).IsAssignableFrom(value))
            {
                throw new ArgumentException("A resource example type must be a class implementing the RestFoundation.ServiceProxy.IResourceExample interface");
            }
        }
    }
}
