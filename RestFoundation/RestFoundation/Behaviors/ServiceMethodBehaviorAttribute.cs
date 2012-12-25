// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Net;
using System.Web;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a service method behavior defined on a service contract.
    /// This class cannot be instantiated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public abstract class ServiceMethodBehaviorAttribute : Attribute, ISecureServiceBehavior
    {
        /// <summary>
        /// Gets the HTTP status code in case of a security exception.
        /// </summary>
        public virtual HttpStatusCode StatusCode
        {
            get
            {
                return HttpStatusCode.Forbidden;
            }
        }

        /// <summary>
        /// Gets the HTTP status description in case of a security exception.
        /// </summary>
        public virtual string StatusDescription
        {
            get
            {
                return RestResources.Forbidden;
            }
        }

        /// <summary>
        /// Returns a value indicating whether to apply the behavior to the provided method of the specified
        /// service type.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="methodContext">The "method applies" context.</param>
        /// <returns>true to apply the behavior; false to bypass.</returns>
        public bool AppliesTo(IServiceContext serviceContext, MethodAppliesContext methodContext)
        {
            return true;
        }

        /// <summary>
        /// Called during the authorization process before a service method or behavior is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method authorizing" behavior context.</param>
        /// <returns>A service method action.</returns>
        public virtual BehaviorMethodAction OnMethodAuthorizing(IServiceContext serviceContext, MethodAuthorizingContext behaviorContext)
        {
            return BehaviorMethodAction.Execute;
        }

        /// <summary>
        /// Called before a service method is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method executing" behavior context.</param>
        /// <returns>A service method action.</returns>
        public virtual BehaviorMethodAction OnMethodExecuting(IServiceContext serviceContext, MethodExecutingContext behaviorContext)
        {
            return BehaviorMethodAction.Execute;
        }

        /// <summary>
        /// Called after a service method is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method executed" behavior context.</param>
        public virtual void OnMethodExecuted(IServiceContext serviceContext, MethodExecutedContext behaviorContext)
        {
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

                if (statusCode != HttpStatusCode.Unauthorized && statusCode != StatusCode)
                {
                    throw new HttpResponseException(StatusCode, StatusDescription);
                }

                throw new HttpResponseException(statusCode, serviceContext.Response.GetStatusDescription());
            }

            HttpCachePolicyBase cache = serviceContext.GetHttpContext().Response.Cache;
            cache.SetProxyMaxAge(new TimeSpan(0L));
            cache.AddValidationCallback(CacheValidationHandler, new CacheValidationHandlerData(serviceContext, behaviorContext));
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
