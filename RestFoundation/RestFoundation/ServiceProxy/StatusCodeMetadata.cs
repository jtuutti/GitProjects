using System;
using System.Net;

namespace RestFoundation.ServiceProxy
{
    public sealed class StatusCodeMetadata : IComparable<StatusCodeMetadata>, IEquatable<StatusCodeMetadata>
    {
        public HttpStatusCode Code { get; set; }
        public string Condition { get; set; }

        public int GetNumericStatusCode()
        {
            return (int) Code;
        }

        public bool Equals(StatusCodeMetadata other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Code == other.Code && string.Equals(Condition, other.Condition);
        }

        public int CompareTo(StatusCodeMetadata other)
        {
            return other != null ? Code.CompareTo(other.Code) : 1;
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

            return obj is StatusCodeMetadata && Equals((StatusCodeMetadata) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Code * 397) ^ (Condition != null ? Condition.GetHashCode() : 0);
            }
        }
    }
}