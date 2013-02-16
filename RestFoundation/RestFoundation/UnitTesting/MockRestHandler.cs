// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents a mock REST handler.
    /// </summary>
    public class MockRestHandler : RestServiceHandler, IEquatable<MockRestHandler>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockRestHandler"/> class.
        /// </summary>
        public MockRestHandler() :
            base(Rest.Configuration.ServiceLocator.GetService<IServiceContext>(),
                 Rest.Configuration.ServiceLocator.GetService<IServiceMethodLocator>(),
                 Rest.Configuration.ServiceLocator.GetService<IServiceMethodInvoker>())
        {
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(MockRestHandler other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.ServiceUrl, ServiceUrl) && Equals(other.ServiceContractTypeName, ServiceContractTypeName) &&
                   Equals(other.UrlTemplate, UrlTemplate);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <filterpriority>2</filterpriority>
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

            return obj is MockRestHandler && Equals((MockRestHandler) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = ServiceUrl != null ? ServiceUrl.GetHashCode() : 0;
                result = (result * 397) ^ (ServiceContractTypeName != null ? ServiceContractTypeName.GetHashCode() : 0);
                result = (result * 397) ^ (UrlTemplate != null ? UrlTemplate.GetHashCode() : 0);
                return result;
            }
        }
    }
}
