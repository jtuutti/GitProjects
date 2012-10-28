// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Globalization;
using System.Net;
using System.Reflection;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a service method locator.
    /// </summary>
    public class ServiceMethodLocator : IServiceMethodLocator
    {
        private readonly IServiceContext m_serviceContext;
        private readonly IServiceFactory m_serviceFactory;
        private readonly AllowHeaderGenerator m_allowHeaderGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMethodLocator"/> class.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="serviceFactory">The service factory.</param>
        public ServiceMethodLocator(IServiceContext serviceContext, IServiceFactory serviceFactory)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException("serviceContext");
            }

            if (serviceFactory == null)
            {
                throw new ArgumentNullException("serviceFactory");
            }

            m_serviceContext = serviceContext;
            m_serviceFactory = serviceFactory;
            m_allowHeaderGenerator = new AllowHeaderGenerator(serviceContext.Response);
        }

        /// <summary>
        /// Locates the service method associated with the provided REST handler and returns the associated data.
        /// </summary>
        /// <param name="handler">A REST handler associated with HTTP request.</param>
        /// <returns>The service method data.</returns>
        public virtual ServiceMethodLocatorData Locate(IRestHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            var serviceContractType = GetServiceContractType(handler.ServiceContractTypeName);
            object service = InitializeService(serviceContractType);

            HttpMethod httpMethod = m_serviceContext.Request.Method;

            if (httpMethod == HttpMethod.Options)
            {
                var optionsDescriptor = service as IOptionsDescriptor;

                if (optionsDescriptor == null || !m_allowHeaderGenerator.TrySetAllowHeaderFromDescriptor(m_serviceContext.Request.Url.LocalPath, optionsDescriptor))
                {
                    m_allowHeaderGenerator.SetAllowHeader(handler.UrlTemplate, serviceContractType);
                }

                return ServiceMethodLocatorData.Options;
            }

            if (httpMethod == HttpMethod.Head)
            {
                m_serviceContext.GetHttpContext().Response.SuppressContent = true;
            }

            MethodInfo method = GetServiceMethod(handler.ServiceUrl, serviceContractType, handler.UrlTemplate, httpMethod);

            return new ServiceMethodLocatorData(service, method);
        }

        private static Type GetServiceContractType(string serviceTypeContractName)
        {
            Type serviceContractType = ServiceContractTypeRegistry.GetType(serviceTypeContractName);

            if (serviceContractType == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, String.Format(CultureInfo.InvariantCulture,
                                                                                                  "Service contract of type '{0}' could not be determined",
                                                                                                  serviceTypeContractName));
            }

            return serviceContractType;
        }

        private static MethodInfo GetServiceMethod(string serviceUrl, Type serviceContractType, string urlTemplate, HttpMethod httpMethod)
        {
            MethodInfo method = ServiceMethodRegistry.GetMethod(new ServiceMetadata(serviceContractType, serviceUrl), urlTemplate, httpMethod);

            if (method == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, String.Format(CultureInfo.InvariantCulture,
                                                                                                  "Service with contract of type '{0}' could not match the URL with a method",
                                                                                                  serviceContractType.Name));
            }

            return method;
        }

        private object InitializeService(Type serviceContractType)
        {
            object service = m_serviceFactory.Create(serviceContractType);

            if (service == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, String.Format(CultureInfo.InvariantCulture,
                                                                                                  "Service with contract of type '{0}' could not be created",
                                                                                                  serviceContractType.Name));
            }

            return service;
        }
    }
}
