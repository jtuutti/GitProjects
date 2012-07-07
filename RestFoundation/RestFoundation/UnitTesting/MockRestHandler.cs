using System;
using RestFoundation.Runtime;

namespace RestFoundation.UnitTesting
{
    public class MockRestHandler : RestHandler, IEquatable<MockRestHandler>
    {
        public MockRestHandler() :
            base(Rest.Active.CreateObject<IServiceContext>(), Rest.Active.CreateObject<ServiceMethodLocator>(),
                 Rest.Active.CreateObject<IServiceMethodInvoker>(), Rest.Active.CreateObject<IResultFactory>())
        {
        }

        public bool Equals(MockRestHandler other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(other.ServiceUrl, ServiceUrl) && Equals(other.ServiceContractTypeName, ServiceContractTypeName) &&
                   Equals(other.UrlTemplate, UrlTemplate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj is MockRestHandler && Equals((MockRestHandler) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (ServiceUrl != null ? ServiceUrl.GetHashCode() : 0);
                result = (result * 397) ^ (ServiceContractTypeName != null ? ServiceContractTypeName.GetHashCode() : 0);
                result = (result * 397) ^ (UrlTemplate != null ? UrlTemplate.GetHashCode() : 0);
                return result;
            }
        }
    }
}
