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
            cache.AddValidationCallback(CacheValidateHandler, new CacheValidateHandlerData(service, method));
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationstatus)
        {
            var handlerData = data as CacheValidateHandlerData;

            if (handlerData == null || handlerData.Service == null || handlerData.Method == null)
            {
                validationstatus = HttpValidationStatus.Invalid;
                return;
            }

            if (!OnMethodAuthorizing(handlerData.Service, handlerData.Method))
            {
                validationstatus = HttpValidationStatus.IgnoreThisRequest;
                return;
            }

            validationstatus = HttpValidationStatus.Valid;
        }

        #region Cache Validate Handler Data

        private sealed class CacheValidateHandlerData
        {
            public CacheValidateHandlerData(object service, MethodInfo method)
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
