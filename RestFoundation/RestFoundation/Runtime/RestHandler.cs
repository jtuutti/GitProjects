using System;
using System.Net;
using System.Web;
using System.Web.Routing;
using RestFoundation.Results;

namespace RestFoundation.Runtime
{
    public class RestHandler : IRestHandler
    {
        private readonly IServiceContext m_serviceContext;
        private readonly ServiceMethodLocator m_serviceMethodLocator;
        private readonly IServiceMethodInvoker m_methodInvoker;
        private readonly IResultFactory m_resultFactory;
        private readonly ResultExecutor m_resultExecutor;

        public RestHandler(IServiceContext serviceContext, ServiceMethodLocator serviceMethodLocator, IServiceMethodInvoker methodInvoker, IResultFactory resultFactory)
        {
            if (serviceContext == null) throw new ArgumentNullException("serviceContext");
            if (serviceMethodLocator == null) throw new ArgumentNullException("serviceMethodLocator");
            if (methodInvoker == null) throw new ArgumentNullException("methodInvoker");
            if (resultFactory == null) throw new ArgumentNullException("resultFactory");

            m_serviceContext = serviceContext;
            m_serviceMethodLocator = serviceMethodLocator;
            m_methodInvoker = methodInvoker;
            m_resultFactory = resultFactory;

            m_resultExecutor = new ResultExecutor();
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

        public string ServiceUrl { get; protected set; }
        public string ServiceContractTypeName { get; protected set; }
        public string UrlTemplate { get; protected set; }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (requestContext == null) throw new ArgumentNullException("requestContext");

            if (requestContext.RouteData == null || requestContext.RouteData.Values == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No route data found");
            }

            if (UnvalidatedHandlerRegistry.IsUnvalidated(this))
            {
                requestContext.HttpContext.Items[ServiceRequestValidator.UnvalidatedHandlerKey] = Boolean.TrueString;
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
            ServiceMethodLocatorData serviceMethodData = m_serviceMethodLocator.Execute(this);

            if (serviceMethodData == ServiceMethodLocatorData.Options)
            {
                return;
            }

            object returnedObj = m_methodInvoker.Invoke(this, serviceMethodData.Service, serviceMethodData.Method);
            IResult result = m_resultFactory.Create(m_serviceContext, returnedObj);

            if (!(result is EmptyResult))
            {
                m_resultExecutor.Execute(m_serviceContext, result, serviceMethodData.Method.ReturnType);
            }
        }
    }
}
