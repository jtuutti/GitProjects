using System;
using System.Reflection;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents the service method data returned by the service method locator.
    /// </summary>
    public class ServiceMethodLocatorData
    {
        /// <summary>
        /// The service method metadata associated with the OPTIONS HTTP method.
        /// </summary>
        public static readonly ServiceMethodLocatorData Options = new ServiceMethodLocatorData();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMethodLocatorData"/> class.
        /// </summary>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The method instance.</param>
        public ServiceMethodLocatorData(object service, MethodInfo method)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            Service = service;
            Method = method;
        }

        private ServiceMethodLocatorData()
        {
        }

        /// <summary>
        /// Gets the service instance.
        /// </summary>
        public object Service { get; private set; }

        /// <summary>
        /// Gets the service method instance.
        /// </summary>
        public MethodInfo Method { get; private set; }
    }
}
