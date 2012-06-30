using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using RestFoundation.Acl;
using RestFoundation.Results;

namespace RestFoundation.Runtime
{
    public sealed class RestHandler : IRouteHandler, IHttpHandler
    {
        private RouteValueDictionary m_routeValues;
        private IServiceContext m_serviceContext;
        private IServiceFactory m_serviceFactory;
        private IResultFactory m_resultFactory;
        private IServiceMethodInvoker m_methodInvoker;

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

            m_routeValues = requestContext.RouteData.Values;
            m_serviceContext = Rest.Active.CreateObject<IServiceContext>();
            m_serviceFactory = Rest.Active.CreateObject<IServiceFactory>();
            m_resultFactory = Rest.Active.CreateObject<IResultFactory>();
            m_methodInvoker = Rest.Active.CreateObject<IServiceMethodInvoker>();

            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var serviceUrl = (string) m_routeValues[RouteConstants.ServiceUrl];
            var serviceContractTypeName = (string) m_routeValues[RouteConstants.ServiceContractType];
            var urlTemplate = (string) m_routeValues[RouteConstants.UrlTemplate];

            Type serviceContractType = ServiceContractTypeRegistry.GetType(serviceContractTypeName);
            HttpMethod httpMethod = context.GetOverriddenHttpMethod();

            if (httpMethod == HttpMethod.Options)
            {
                HashSet<HttpMethod> allowedHttpMethods = HttpMethodRegistry.GetHttpMethods(new RouteMetadata(serviceContractType.AssemblyQualifiedName, urlTemplate));
                context.AppendAllowHeader(allowedHttpMethods);

                return;
            }

            if (httpMethod == HttpMethod.Head)
            {
                context.Response.SuppressContent = true;
            }

            object service = m_serviceFactory.Create(m_serviceContext, serviceContractType);

            ValidateAclAttribute acl;
            OutputCacheAttribute cache;
            MethodInfo method = ServiceMethodRegistry.GetMethod(new ServiceMetadata(serviceContractType, serviceUrl), urlTemplate, httpMethod, out acl, out cache);

            if (acl != null)
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
                if ((httpMethod == HttpMethod.Get || httpMethod == HttpMethod.Head) && cache != null)
                {
                    using (var page = new OutputCachedPage(cache.CacheSettings))
                    {
                        page.ProcessRequest(context);
                    }
                }

                httpResult.Execute(m_serviceContext);
            }
            else
            {
                context.SetServiceMethodResponseStatus(methodReturnType);
            }
        }
    }
}
