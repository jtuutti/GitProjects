using System;
using System.Collections.Generic;

namespace RestFoundation.ServiceProxy
{
    public sealed class ParameterMetadata : IEquatable<ParameterMetadata>
    {
        public string Name { get; set; }
        public ParameterType Type { get; set; }
        public object ExampleValue { get; set; }
        public IList<object> AllowedValues { get; set; }
        public string RegexConstraint { get; set; }

        public bool Equals(ParameterMetadata other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return ReferenceEquals(this, other) || string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is ParameterMetadata && Equals((ParameterMetadata) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}