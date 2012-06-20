using System;
using System.Net;
using System.Reflection;
using System.Web;

namespace RestFoundation.Behaviors
{
    public abstract class ServiceSecurityBehavior : IServiceBehavior
    {
        private const string DefaultForbiddenMessage = "Forbidden";

        private string m_forbiddenErrorMessage;

        protected ServiceSecurityBehavior()
        {
            m_forbiddenErrorMessage = DefaultForbiddenMessage;
        }

        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }

        public virtual bool OnActionBinding(object service, MethodInfo actionMethod)
        {
            return false;
        }

        public virtual bool OnActionExecuting(object resource)
        {
            return true;
        }

        public virtual void OnActionExecuted(object result)
        {
        }

        public virtual bool OnException(Exception ex)
        {
            return true;
        }

        protected void SetForbiddenErrorMessage(string message)
        {
            m_forbiddenErrorMessage = message ?? DefaultForbiddenMessage;
        }

        void IServiceBehavior.OnActionBinding(object service, MethodInfo actionMethod)
        {
            if (HttpContext.Current == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No HTTP context found");
            }

            if (!OnActionBinding(service, actionMethod))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, m_forbiddenErrorMessage);
            }

            HttpCachePolicy cache = HttpContext.Current.Response.Cache;
            cache.SetProxyMaxAge(new TimeSpan(0L));
            cache.AddValidationCallback(CacheValidateHandler, new CacheValidateHandlerData(service, actionMethod));
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationstatus)
        {
            var handlerData = data as CacheValidateHandlerData;

            if (handlerData == null || handlerData.Service == null || handlerData.ActionMethod == null)
            {
                validationstatus = HttpValidationStatus.Invalid;
                return;
            }

            if (!OnActionBinding(handlerData.Service, handlerData.ActionMethod))
            {
                validationstatus = HttpValidationStatus.IgnoreThisRequest;
                return;
            }

            validationstatus = HttpValidationStatus.Valid;
        }

        #region Cache Validate Handler Data

        private sealed class CacheValidateHandlerData
        {
            public CacheValidateHandlerData(object service, MethodInfo actionMethod)
            {
                Service = service;
                ActionMethod = actionMethod;
            }

            public object Service { get; private set; }
            public MethodInfo ActionMethod { get; private set; }
        }

        #endregion
    }
}
