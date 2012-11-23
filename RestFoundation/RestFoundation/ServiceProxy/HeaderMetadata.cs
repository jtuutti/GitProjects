using System;
using System.Globalization;

namespace RestFoundation.ServiceProxy
{
    public sealed class HeaderMetadata : IEquatable<HeaderMetadata>, IComparable<HeaderMetadata>
    {
        private static readonly StringComparer Comparer = StringComparer.Create(CultureInfo.InvariantCulture, true);

        public string Name { get; set; }
        public string Value { get; set; }

        public bool Equals(HeaderMetadata other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Name, other.Name) && string.Equals(Value, other.Value);
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

            return obj is HeaderMetadata && Equals((HeaderMetadata) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }
 
        public int CompareTo(HeaderMetadata other)
        {
            if (other == null)
            {
                return 1;
            }

            return Comparer.Compare(Name, other.Name);
        }
    }
}
