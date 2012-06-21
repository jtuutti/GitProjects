using System;

namespace RestFoundation.ServiceProxy.Helpers
{
    public sealed class EndPoint : IComparable<EndPoint>
    {
        public string ServiceUrl { get; set; }
        public string UrlTempate { get; set; }
        public string RelativeUrl { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public string Description { get; set; }

        public int CompareTo(EndPoint other)
        {
            if (other == null)
            {
                return 1;
            }

            int serviceTypeComparision = String.CompareOrdinal(ServiceUrl, ServiceUrl);

            if (serviceTypeComparision != 0)
            {
                return serviceTypeComparision;
            }

            int urlComparision = String.CompareOrdinal(UrlTempate, other.UrlTempate);

            return urlComparision == 0 ? HttpMethod.CompareTo(other.HttpMethod) : urlComparision;
        }
    }
}
