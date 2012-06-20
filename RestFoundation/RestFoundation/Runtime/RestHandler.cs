using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public sealed class RestHandler : IRouteHandler, IHttpHandler
    {
        private RouteValueDictionary m_routeValues;
        private IServiceFactory m_serviceFactory;
        private IResultFactory m_resultFactory;
        private IActionMethodInvoker m_methodInvoker;

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
            m_serviceFactory = ObjectActivator.Create<IServiceFactory>();
            m_resultFactory = ObjectActivator.Create<IResultFactory>();
            m_methodInvoker = ObjectActivator.Create<IActionMethodInvoker>();

            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var serviceContractTypeName = (string) m_routeValues[RouteConstants.ServiceContractType];
            var urlTemplate = (string) m_routeValues[RouteConstants.UrlTemplate];

            Type serviceContractType = ServiceContractTypeRegistry.GetType(serviceContractTypeName);

            HashSet<HttpMethod> allowedHttpMethods = HttpMethodRegistry.GetHttpMethods(new RouteMetadata(serviceContractType.AssemblyQualifiedName, urlTemplate));
            HttpMethod httpMethod = context.GetOverriddenHttpMethod();

            if (httpMethod == HttpMethod.Options)
            {
                context.AppendAllowHeader(allowedHttpMethods);
                return;
            }

            if (!allowedHttpMethods.Contains(httpMethod))
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed, "HTTP method is not allowed");
            }

            if (httpMethod == HttpMethod.Head)
            {
                context.Response.SuppressContent = true;
            }

            object service = m_serviceFactory.Create(serviceContractType);

            OutputCacheAttribute cache;
            MethodInfo actionMethod = ActionMethodRegistry.GetActionMethod(serviceContractType, urlTemplate, httpMethod, out cache);

            object result = m_methodInvoker.Invoke(this, service, actionMethod);

            ProcessResult(context, httpMethod, result, cache, actionMethod.ReturnType);
        }

        private void ProcessResult(HttpContext context, HttpMethod httpMethod, object result, OutputCacheAttribute cache, Type actionMethodReturnType)
        {
            IResult httpResult = m_resultFactory.Create(result);

            if (httpResult != null)
            {
                if ((httpMethod == HttpMethod.Get || httpMethod == HttpMethod.Head) && cache != null)
                {
                    using (var page = new OutputCachedPage(cache.CacheSettings))
                    {
                        page.ProcessRequest(context);
                    }
                }

                httpResult.Execute();
            }
            else
            {
                context.SetActionMethodResponseStatus(actionMethodReturnType);
            }
        }
    }
}
