using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using RestFoundation.Results;

namespace RestFoundation.Runtime
{
    public class RestHandler : IRestHandler
    {
        private readonly IServiceContext m_serviceContext;
        private readonly IServiceFactory m_serviceFactory;
        private readonly IResultFactory m_resultFactory;
        private readonly IServiceMethodInvoker m_methodInvoker;

        public RestHandler(IServiceContext serviceContext, IServiceFactory serviceFactory, IServiceMethodInvoker methodInvoker, IResultFactory resultFactory)
        {
            if (serviceContext == null) throw new ArgumentNullException("serviceContext");
            if (serviceFactory == null) throw new ArgumentNullException("serviceFactory");
            if (methodInvoker == null) throw new ArgumentNullException("methodInvoker");
            if (resultFactory == null) throw new ArgumentNullException("resultFactory");

            m_serviceContext = serviceContext;
            m_serviceFactory = serviceFactory;
            m_methodInvoker = methodInvoker;
            m_resultFactory = resultFactory;
        }

        public IServiceContext Context
        {
            get
            {
                return m_serviceContext;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        protected string ServiceUrl { get; set; }
        protected string ServiceContractTypeName { get; set; }
        protected string UrlTemplate { get; set; }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (requestContext == null) throw new ArgumentNullException("requestContext");

            if (UnvalidatedHandlerRegistry.IsUnvalidated(this))
            {
                requestContext.HttpContext.Items[ServiceRequestValidator.UnvalidatedHandlerKey] = Boolean.TrueString;
            }

            if (requestContext.RouteData == null || requestContext.RouteData.Values == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No route data found");
            }

            ServiceUrl = (string) requestContext.RouteData.Values[RouteConstants.ServiceUrl];
            ServiceContractTypeName = (string) requestContext.RouteData.Values[RouteConstants.ServiceContractType];
            UrlTemplate = (string) requestContext.RouteData.Values[RouteConstants.UrlTemplate];

            if (String.IsNullOrEmpty(ServiceUrl) || String.IsNullOrEmpty(ServiceContractTypeName) || UrlTemplate == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "Not Found");
            }

            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            Type serviceContractType = ServiceContractTypeRegistry.GetType(ServiceContractTypeName);

            if (serviceContractType == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, String.Format("Service contract of type '{0}' could not be determined", ServiceContractTypeName));
            }

            HttpMethod httpMethod = m_serviceContext.Request.Method;

            if (httpMethod == HttpMethod.Options)
            {
                HashSet<HttpMethod> allowedHttpMethods = HttpMethodRegistry.GetHttpMethods(new RouteMetadata(serviceContractType.AssemblyQualifiedName, UrlTemplate));
                m_serviceContext.Response.SetHeader("Allow", String.Join(", ", allowedHttpMethods.Select(m => m.ToString().ToUpperInvariant()).OrderBy(m => m)));

                return;
            }

            if (httpMethod == HttpMethod.Head)
            {
                m_serviceContext.GetHttpContext().Response.SuppressContent = true;
            }

            object service = m_serviceFactory.Create(m_serviceContext, serviceContractType);

            if (service == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, String.Format("Service with contract of type '{0}' could not be created", ServiceContractTypeName));
            }

            MethodInfo method = ServiceMethodRegistry.GetMethod(new ServiceMetadata(serviceContractType, ServiceUrl), UrlTemplate, httpMethod);

            object result = m_methodInvoker.Invoke(this, service, method);

            if (!(result is EmptyResult))
            {
                ProcessResult(result, method.ReturnType);
            }
        }

        private void ProcessResult(object result, Type methodReturnType)
        {
            IResult httpResult = m_resultFactory.Create(m_serviceContext, result);

            if (httpResult != null)
            {
                httpResult.Execute(m_serviceContext);
            }
            else if (m_serviceContext.Response.GetStatusCode() == HttpStatusCode.OK && methodReturnType == typeof(void))
            {
                m_serviceContext.Response.SetStatus(HttpStatusCode.NoContent, "No Content");
            }
        }
    }
}
