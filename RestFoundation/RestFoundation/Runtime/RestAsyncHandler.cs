﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using RestFoundation.Acl;
using RestFoundation.Results;

namespace RestFoundation.Runtime
{
    public sealed class RestAsyncHandler : IRouteHandler, IHttpAsyncHandler
    {
        private readonly IServiceContext m_serviceContext;
        private readonly IServiceFactory m_serviceFactory;
        private readonly IResultFactory m_resultFactory;
        private readonly IServiceMethodInvoker m_methodInvoker;
        private RouteValueDictionary m_routeValues;

        public RestAsyncHandler(IServiceContext serviceContext, IServiceFactory serviceFactory, IServiceMethodInvoker methodInvoker, IResultFactory resultFactory)
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

            m_routeValues = requestContext.RouteData.Values;
            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new NotSupportedException("Async route handler does not support synchronous requests");
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
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

                return Task<IResult>.Factory.StartNew(() => new EmptyResult()).ContinueWith(action => cb(action));
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

            var httpArguments = new HttpArguments(HttpContext.Current, httpMethod, cache, method.ReturnType);

            return Task<IResult>.Factory.StartNew(state =>
                                                 {
                                                     HttpContext.Current = ((HttpArguments) state).Context;
                                                     return m_resultFactory.Create(m_serviceContext, m_methodInvoker.Invoke(this, m_serviceContext, service, method));
                                                 }, httpArguments)
                                        .ContinueWith(action => cb(action));
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            var task = (Task<IResult>) result;

            if (task.IsFaulted && task.Exception != null)
            {
                throw UnwrapFaultException(task);
            }

            if (!(task.Result is EmptyResult))
            {
                ProcessResult(task.Result, (HttpArguments) result.AsyncState);
            }
        }

        private static Exception UnwrapFaultException(Task<IResult> task)
        {
            AggregateException taskException = task.Exception;

            if (taskException == null)
            {
                return new HttpResponseException(HttpStatusCode.InternalServerError, "HTTP request failed to process");
            }

            if (taskException.InnerException is HttpResponseException || taskException.InnerException is HttpRequestValidationException)
            {
                return taskException.InnerException;
            }

            return taskException;
        }

        private void ProcessResult(IResult result, HttpArguments httpArguments)
        {
            HttpContext.Current = httpArguments.Context;

            if (result != null)
            {
                if ((httpArguments.Method == HttpMethod.Get || httpArguments.Method == HttpMethod.Head) && httpArguments.Cache != null)
                {
                    using (var page = new OutputCachedPage(httpArguments.Cache.CacheSettings))
                    {
                        page.ProcessRequest(httpArguments.Context);
                    }
                }

                result.Execute(m_serviceContext);
            }
            else
            {
               httpArguments.Context.SetServiceMethodResponseStatus(httpArguments.MethodReturnType);
            }
        }

        #region HTTP Task Arguments

        private sealed class HttpArguments
        {
            public HttpArguments(HttpContext context, HttpMethod method, OutputCacheAttribute cache, Type methodReturnType)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                if (methodReturnType == null)
                {
                    throw new ArgumentNullException("methodReturnType");
                }

                Context = context;
                Method = method;
                Cache = cache;
                MethodReturnType = methodReturnType;
            }

            public HttpContext Context { get; private set; }
            public HttpMethod Method { get; private set; }
            public OutputCacheAttribute Cache { get; private set; }
            public Type MethodReturnType { get; private set; }
        }

        #endregion
    }
}
