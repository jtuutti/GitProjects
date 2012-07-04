using System;
using System.Net;
using System.Reflection;
using System.Web;

namespace RestFoundation.Behaviors
{
    public abstract class ServiceSecurityBehavior : ServiceBehavior, ISecureServiceBehavior
    {
        private const string DefaultForbiddenMessage = "Forbidden";

        private string m_forbiddenMessage;

        protected ServiceSecurityBehavior()
        {
            m_forbiddenMessage = DefaultForbiddenMessage;
        }

        public virtual bool OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            return false;
        }

        protected void SetForbiddenErrorMessage(string message)
        {
            m_forbiddenMessage = message ?? DefaultForbiddenMessage;
        }

        void ISecureServiceBehavior.OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            if (HttpContext.Current == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No HTTP context found");
            }

            if (!OnMethodAuthorizing(context, service, method))
            {
                HttpStatusCode statusCode = context.Response.GetStatusCode();

                if (statusCode != HttpStatusCode.Unauthorized && statusCode != HttpStatusCode.Forbidden)
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden, m_forbiddenMessage);
                }

                throw new HttpResponseException(statusCode, context.Response.GetStatusDescription());
            }

            HttpCachePolicy cache = HttpContext.Current.Response.Cache;
            cache.SetProxyMaxAge(new TimeSpan(0L));
            cache.AddValidationCallback(CacheValidationHandler, new CacheValidationHandlerData(context, service, method));
        }

        private void CacheValidationHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            var handlerData = data as CacheValidationHandlerData;

            if (handlerData == null || handlerData.Service == null || handlerData.Method == null)
            {
                validationStatus = HttpValidationStatus.Invalid;
                return;
            }

            if (!OnMethodAuthorizing(handlerData.Context, handlerData.Service, handlerData.Method))
            {
                validationStatus = HttpValidationStatus.IgnoreThisRequest;
                return;
            }

            validationStatus = HttpValidationStatus.Valid;
        }

        #region Cache Validation Handler Data

        private sealed class CacheValidationHandlerData
        {
            public CacheValidationHandlerData(IServiceContext context, object service, MethodInfo method)
            {
                Context = context;
                Service = service;
                Method = method;
            }

            public IServiceContext Context { get; private set; }
            public object Service { get; private set; }
            public MethodInfo Method { get; private set; }
        }

        #endregion
    }
}
