// <copyright>
// Dmitry Starosta, 2012
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
                serviceContext.Response.SetStatus(HttpStatusCode.NotFound, RestResources.NotFound);
                return BehaviorMethodAction.Stop;
            }

            return BehaviorMethodAction.Execute;
        }
    }
}
