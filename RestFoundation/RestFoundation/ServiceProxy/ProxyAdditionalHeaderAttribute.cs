using System;

namespace RestFoundation.ServiceProxy
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class ProxyAdditionalHeaderAttribute : Attribute
    {
        public ProxyAdditionalHeaderAttribute(string name, string value)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");

            Name = name;
            Value = value;
        }

        internal string Name { get; private set; }
        internal string Value { get; private set; }
    }
}
