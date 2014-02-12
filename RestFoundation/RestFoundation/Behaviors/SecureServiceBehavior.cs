// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Net;
using System.Web;
using RestFoundation.Runtime;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// The base secure service behavior class. It is highly recommended to inherit this class
    /// instead of implementing the <see cref="ISecureServiceBehavior"/> interface to avoid
    /// output caching security problems.
    /// This class cannot be instantiated.
    /// </summary>
    public abstract class SecureServiceBehavior : ServiceBehavior, ISecureServiceBehavior
    {
        private HttpStatusCode m_statusCode;
        private string m_statusDescription;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureServiceBehavior"/> class.
        /// </summary>
        protected SecureServiceBehavior()
        {
            m_statusCode = HttpStatusCode.Forbidden;
            m_statusDescription = Resources.Global.Forbidden;
        }

        /// <summary>
        /// Called during the authorization process before a service method or behavior is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method authorizing" behavior context.</param>
        /// <returns>A service method action.</returns>
        public virtual BehaviorMethodAction OnMethodAuthorizing(IServiceContext serviceContext, MethodAuthorizingContext behaviorContext)
        {
            return BehaviorMethodAction.Stop;
        }

        void ISecureServiceBehavior.OnMethodAuthorizing(IServiceContext serviceContext, MethodAuthorizingContext behaviorContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException("serviceContext");
            }

            if (OnMethodAuthorizing(serviceContext, behaviorContext) == BehaviorMethodAction.Stop)
            {
                HttpStatusCode statusCode = serviceContext.Response.GetStatusCode();

                if (statusCode != HttpStatusCode.Unauthorized && statusCode != m_statusCode)
                {
                    throw new HttpResponseException(m_statusCode, m_statusDescription);
                }

                throw new HttpResponseException(statusCode, serviceContext.Response.GetStatusDescription());
            }

            HttpCachePolicyBase cache = serviceContext.GetHttpContext().Response.Cache;
            cache.SetProxyMaxAge(new TimeSpan(0L));
            cache.AddValidationCallback(CacheValidationHandler, new CacheValidationHandlerData(serviceContext, behaviorContext));
        }

        /// <summary>
        /// Sets an HTTP status code and description in case of a security exception.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="description">The status description.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the status code is less than 400.</exception>
        protected void SetStatus(HttpStatusCode statusCode, string description)
        {
            SetStatusCode(statusCode);
            SetStatusDescription(description);
        }

        /// <summary>
        /// Sets an HTTP status code in case of a security exception.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the status code is less than 400.</exception>
        protected void SetStatusCode(HttpStatusCode statusCode)
        {
            if ((int) statusCode < 400)
            {
                throw new ArgumentOutOfRangeException("statusCode");
            }

            m_statusCode = statusCode;
        }

        /// <summary>
        /// Sets an HTTP status description in case of a security exception.
        /// </summary>
        /// <param name="description">The status description.</param>
        protected void SetStatusDescription(string description)
        {
            m_statusDescription = description ?? Resources.Global.Forbidden;
        }

        private void CacheValidationHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            var handlerData = data as CacheValidationHandlerData;

            if (handlerData == null)
            {
                validationStatus = HttpValidationStatus.Invalid;
                return;
            }

            if (OnMethodAuthorizing(handlerData.ServiceContext, handlerData.BehaviorContext) == BehaviorMethodAction.Stop)
            {
                validationStatus = HttpValidationStatus.IgnoreThisRequest;
                return;
            }

            validationStatus = HttpValidationStatus.Valid;
        }
    }
}
