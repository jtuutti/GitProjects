﻿using System;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents a mock REST async handler.
    /// </summary>
    public class MockRestAsyncHandler : RestAsyncHandler, IEquatable<MockRestAsyncHandler>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockRestAsyncHandler"/> class.
        /// </summary>
        public MockRestAsyncHandler() :
            base(Rest.Active.ServiceLocator.GetService<IServiceContext>(),
                 Rest.Active.ServiceLocator.GetService<IServiceMethodLocator>(),
                 Rest.Active.ServiceLocator.GetService<IServiceMethodInvoker>(),
                 Rest.Active.ServiceLocator.GetService<IResultFactory>(),
                 Rest.Active.ServiceLocator.GetService<IResultExecutor>())
        {
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(MockRestAsyncHandler other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

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
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj is MockRestAsyncHandler && Equals((MockRestAsyncHandler) obj);
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
