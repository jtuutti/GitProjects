using System;
using System.Net;

namespace RestFoundation.ServiceProxy
{
    public sealed class StatusCodeMetadata : IEquatable<StatusCodeMetadata>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }

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

            return StatusCode == other.StatusCode && string.Equals(StatusDescription, other.StatusDescription);
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
                return ((int) StatusCode * 397) ^ (StatusDescription != null ? StatusDescription.GetHashCode() : 0);
            }
        }
    }
}