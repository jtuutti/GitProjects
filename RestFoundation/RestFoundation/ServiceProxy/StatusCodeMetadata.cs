using System;
using System.Net;

namespace RestFoundation.ServiceProxy
{
    public sealed class StatusCodeMetadata : IComparable<StatusCodeMetadata>, IEquatable<StatusCodeMetadata>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string StatusCondition { get; set; }

        public int GetNumericStatusCode()
        {
            return (int) StatusCode;
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

            return StatusCode == other.StatusCode && string.Equals(StatusCondition, other.StatusCondition);
        }

        public int CompareTo(StatusCodeMetadata other)
        {
            return other != null ? StatusCode.CompareTo(other.StatusCode) : 1;
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
                return ((int) StatusCode * 397) ^ (StatusCondition != null ? StatusCondition.GetHashCode() : 0);
            }
        }
    }
}