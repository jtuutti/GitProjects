// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Net;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a secure behavior for a service or a service method that rejects responses
    /// not generated using AJAX. A non-AJAX HTTP request will set a 404 (Not Found) HTTP
    /// status code.
    /// </summary>
    public class AjaxOnlyBehavior : SecureServiceBehavior
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
                SetStatus(HttpStatusCode.NotFound, Resources.Global.NotFound);
                return BehaviorMethodAction.Stop;
            }

            return BehaviorMethodAction.Execute;
        }
    }
}