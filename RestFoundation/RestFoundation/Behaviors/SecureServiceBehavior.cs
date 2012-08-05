// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Net;
using System.Reflection;
using System.Web;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// The base secure service behavior class. It is highly recommended to inherit this class
    /// instead of implementing the <see cref="ISecureServiceBehavior"/> interface to avoid
    /// output caching security problems.
    /// </summary>
    public abstract class SecureServiceBehavior : ServiceBehavior, ISecureServiceBehavior
    {
        private const string DefaultForbiddenMessage = "Forbidden";

        private string m_forbiddenMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureServiceBehavior"/> class.
        /// </summary>
        protected SecureServiceBehavior()
        {
            m_forbiddenMessage = DefaultForbiddenMessage;
        }

        /// <summary>
        /// Called during the authorization process before a service method or behavior is executed.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        /// <returns>A service method action.</returns>
        public virtual BehaviorMethodAction OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            return BehaviorMethodAction.Stop;
        }

        /// <summary>
        /// Called during the authorization process before a service method or behavior is executed.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        void ISecureServiceBehavior.OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (OnMethodAuthorizing(context, service, method) == BehaviorMethodAction.Stop)
            {
                HttpStatusCode statusCode = context.Response.GetStatusCode();

                if (statusCode != HttpStatusCode.Unauthorized && statusCode != HttpStatusCode.Forbidden)
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden, m_forbiddenMessage);
                }

                throw new HttpResponseException(statusCode, context.Response.GetStatusDescription());
            }

            HttpCachePolicyBase cache = context.GetHttpContext().Response.Cache;
            cache.SetProxyMaxAge(new TimeSpan(0L));
            cache.AddValidationCallback(CacheValidationHandler, new CacheValidationHandlerData(context, service, method));
        }

        /// <summary>
        /// Sets an error message in the case of a security exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        protected void SetForbiddenErrorMessage(string message)
        {
            m_forbiddenMessage = message ?? DefaultForbiddenMessage;
        }

        private void CacheValidationHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            var handlerData = data as CacheValidationHandlerData;

            if (handlerData == null || handlerData.Service == null || handlerData.Method == null)
            {
                validationStatus = HttpValidationStatus.Invalid;
                return;
            }

            if (OnMethodAuthorizing(handlerData.Context, handlerData.Service, handlerData.Method) == BehaviorMethodAction.Stop)
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
