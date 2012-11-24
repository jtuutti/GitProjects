using System;

namespace RestFoundation.ServiceProxy
{
    public sealed class ParameterMetadata : IEquatable<ParameterMetadata>
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool IsRouteParameter { get; set; }
        public object ExampleValue { get; set; }
        public string AllowedValues { get; set; }
        public string RegexConstraint { get; set; }

        public string GetTypeDescription()
        {
            return TypeDescriptor.GetTypeName(Type);
        }

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
            return Name != null ? Name.GetHashCode() : 0;
        }
    }
}