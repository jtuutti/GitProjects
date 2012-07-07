using System;

namespace RestFoundation.ServiceProxy
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ProxyQueryParameterAttribute : Attribute
    {
        public ProxyQueryParameterAttribute(string name, object exampleValue)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (exampleValue == null) throw new ArgumentNullException("exampleValue");

            Name = name;
            ExampleValue = exampleValue;
        }

        public Type ParameterType { get; set; }
        public string RegexConstraint { get; set; }
        public string AllowedValues { get; set; }

        public string Name { get; private set; }
        public object ExampleValue { get; private set; }
    }
}
