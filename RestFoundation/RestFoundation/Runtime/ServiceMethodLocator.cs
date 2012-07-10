using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;

namespace RestFoundation.Runtime
{
    public class ServiceMethodLocator : IServiceMethodLocator
    {
        private readonly IServiceContext m_serviceContext;
        private readonly IServiceFactory m_serviceFactory;

        public ServiceMethodLocator(IServiceContext serviceContext, IServiceFactory serviceFactory)
        {
            if (serviceContext == null) throw new ArgumentNullException("serviceContext");
            if (serviceFactory == null) throw new ArgumentNullException("serviceFactory");

            m_serviceContext = serviceContext;
            m_serviceFactory = serviceFactory;
        }

        public virtual ServiceMethodLocatorData Execute(IRestHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            var serviceContractType = GetServiceContractType(handler.ServiceContractTypeName);

            HttpMethod httpMethod = m_serviceContext.Request.Method;

            if (httpMethod == HttpMethod.Options)
            {
                SetAllowHeader(handler.UrlTemplate, serviceContractType);
                return ServiceMethodLocatorData.Options;
            }

            if (httpMethod == HttpMethod.Head)
            {
                m_serviceContext.GetHttpContext().Response.SuppressContent = true;
            }

            object service = InitializeService(serviceContractType);
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
            object service = m_serviceFactory.Create(m_serviceContext, serviceContractType);

            if (service == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, String.Format(CultureInfo.InvariantCulture,
                                                                                                  "Service with contract of type '{0}' could not be created",
                                                                                                  serviceContractType.Name));
            }

            return service;
        }

        private void SetAllowHeader(string urlTemplate, Type serviceContractType)
        {
            HashSet<HttpMethod> allowedHttpMethods = HttpMethodRegistry.GetHttpMethods(new RouteMetadata(serviceContractType.AssemblyQualifiedName, urlTemplate));
            m_serviceContext.Response.SetHeader("Allow", String.Join(", ", allowedHttpMethods.Select(m => m.ToString().ToUpperInvariant()).OrderBy(m => m)));
        }
    }
}
