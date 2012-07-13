using System;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.UnitTesting
{
    public class MockRestAsyncHandler : RestAsyncHandler, IEquatable<MockRestAsyncHandler>
    {
        public MockRestAsyncHandler() :
            base(Rest.Active.CreateObject<IServiceContext>(), Rest.Active.CreateObject<IServiceMethodLocator>(), Rest.Active.CreateObject<IServiceMethodInvoker>(),
                 Rest.Active.CreateObject<IResultFactory>(), Rest.Active.CreateObject<IResultExecutor>())
        {
        }

        public bool Equals(MockRestAsyncHandler other)
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

            return obj is MockRestAsyncHandler && Equals((MockRestAsyncHandler) obj);
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
