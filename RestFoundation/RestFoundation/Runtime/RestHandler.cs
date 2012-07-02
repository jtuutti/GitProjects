using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using RestFoundation.Acl;
using RestFoundation.Results;

namespace RestFoundation.Runtime
{
    public sealed class RestHandler : IRouteHandler, IHttpHandler
    {
        private readonly IServiceContext m_serviceContext;
        private readonly IServiceFactory m_serviceFactory;
        private readonly IResultFactory m_resultFactory;
        private readonly IServiceMethodInvoker m_methodInvoker;

        private HttpContextBase m_context;
        private RouteValueDictionary m_routeValues;

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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (requestContext == null) throw new ArgumentNullException("requestContext");

            if (UnvalidatedHandlerRegistry.IsUnvalidated(this))
            {
                requestContext.HttpContext.Items[ServiceRequestValidator.UnvalidatedHandlerKey] = Boolean.TrueString;
            }

            m_context = requestContext.HttpContext;
            m_routeValues = requestContext.RouteData.Values;

            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            var serviceUrl = (string) m_routeValues[RouteConstants.ServiceUrl];
            var serviceContractTypeName = (string) m_routeValues[RouteConstants.ServiceContractType];
            var urlTemplate = (string) m_routeValues[RouteConstants.UrlTemplate];

            if (String.IsNullOrEmpty(serviceUrl) || String.IsNullOrEmpty(serviceContractTypeName) || String.IsNullOrEmpty(urlTemplate))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "Not Found");
            }

            Type serviceContractType = ServiceContractTypeRegistry.GetType(serviceContractTypeName);

            if (serviceContractType == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, String.Format("Service contract of type '{0}' could not be determined", serviceContractTypeName));
            }

            HttpMethod httpMethod = m_context.GetOverriddenHttpMethod();

            if (httpMethod == HttpMethod.Options)
            {
                HashSet<HttpMethod> allowedHttpMethods = HttpMethodRegistry.GetHttpMethods(new RouteMetadata(serviceContractType.AssemblyQualifiedName, urlTemplate));
                m_serviceContext.Response.SetHeader("Allow", String.Join(", ", allowedHttpMethods.Select(m => m.ToString().ToUpperInvariant()).OrderBy(m => m)));

                return;
            }

            if (httpMethod == HttpMethod.Head)
            {
                m_context.Response.SuppressContent = true;
            }

            object service = m_serviceFactory.Create(m_serviceContext, serviceContractType);

            if (service == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, String.Format("Service with contract of type '{0}' could not be created", serviceContractTypeName));
            }

            ValidateAclAttribute acl;
            OutputCacheAttribute cache;
            MethodInfo method = ServiceMethodRegistry.GetMethod(new ServiceMetadata(serviceContractType, serviceUrl), urlTemplate, httpMethod, out acl, out cache);

            if (acl != null && context != null)
            {
                AclValidator.Validate(context, acl.SectionName);
            }

            object result = m_methodInvoker.Invoke(this, m_serviceContext, service, method);

            if (!(result is EmptyResult))
            {
                ProcessResult(context, httpMethod, result, cache, method.ReturnType);
            }
        }

        private void ProcessResult(HttpContext context, HttpMethod httpMethod, object result, OutputCacheAttribute cache, Type methodReturnType)
        {
            IResult httpResult = m_resultFactory.Create(m_serviceContext, result);

            if (httpResult != null)
            {
                if ((httpMethod == HttpMethod.Get || httpMethod == HttpMethod.Head) && cache != null && context != null)
                {
                    using (var page = new OutputCachedPage(cache.CacheSettings))
                    {
                        page.ProcessRequest(context);
                    }
                }

                httpResult.Execute(m_serviceContext);
            }
            else if (m_serviceContext.Response.GetStatusCode() == HttpStatusCode.OK && methodReturnType == typeof(void))
            {
                m_serviceContext.Response.SetStatus(HttpStatusCode.NoContent, "No Content");
            }
        }
    }
}
