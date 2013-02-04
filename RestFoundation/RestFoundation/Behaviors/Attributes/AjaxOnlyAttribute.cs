// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Net;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a service method that can only be called using AJAX.
    /// HTTP connection will set a 404 (Not Found) HTTP status code if the service method
    /// was not called using AJAX.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class AjaxOnlyAttribute : ServiceMethodBehaviorAttribute
    {
        /// <summary>
        /// Gets the HTTP status code in case of a security exception.
        /// </summary>
        public override HttpStatusCode StatusCode
        {
            get
            {
                return HttpStatusCode.NotFound;
            }
        }

        /// <summary>
        /// Gets the HTTP status description in case of a security exception.
        /// </summary>
        public override string StatusDescription
        {
            get
            {
                return RestResources.NotFound;
            }
        }

        /// <summary>
        /// Called during the authorization process before a service method or behavior is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method authorizing" behavior context.</param>
        /// <returns>A service method action.</returns>
        public override BehaviorMethodAction OnMethodAuthorizing(IServiceContext serviceContext, MethodAuthorizingContext behaviorContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException("serviceContext");
            }

            if (!serviceContext.Request.IsAjax)
            {
                return BehaviorMethodAction.Stop;
            }

            return BehaviorMethodAction.Execute;
        }
    }
}
