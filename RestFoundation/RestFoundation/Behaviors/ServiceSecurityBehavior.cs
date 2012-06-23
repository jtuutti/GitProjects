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

        public virtual bool OnMethodAuthorizing(object service, MethodInfo method)
        {
            return false;
        }

        protected void SetForbiddenErrorMessage(string message)
        {
            m_forbiddenMessage = message ?? DefaultForbiddenMessage;
        }

        void ISecureServiceBehavior.OnMethodAuthorizing(object service, MethodInfo method)
        {
            if (HttpContext.Current == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No HTTP context found");
            }

            if (!OnMethodAuthorizing(service, method))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, m_forbiddenMessage);
            }

            HttpCachePolicy cache = HttpContext.Current.Response.Cache;
            cache.SetProxyMaxAge(new TimeSpan(0L));
            cache.AddValidationCallback(CacheValidationHandler, new CacheValidationHandlerData(service, method));
        }

        private void CacheValidationHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            var handlerData = data as CacheValidationHandlerData;

            if (handlerData == null || handlerData.Service == null || handlerData.Method == null)
            {
                validationStatus = HttpValidationStatus.Invalid;
                return;
            }

            if (!OnMethodAuthorizing(handlerData.Service, handlerData.Method))
            {
                validationStatus = HttpValidationStatus.IgnoreThisRequest;
                return;
            }

            validationStatus = HttpValidationStatus.Valid;
        }

        #region Cache Validation Handler Data

        private sealed class CacheValidationHandlerData
        {
            public CacheValidationHandlerData(object service, MethodInfo method)
            {
                Service = service;
                Method = method;
            }

            public object Service { get; private set; }
            public MethodInfo Method { get; private set; }
        }

        #endregion
    }
}
