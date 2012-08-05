// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Net;
using System.Reflection;

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
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        /// <returns>A service method action.</returns>
        public override BehaviorMethodAction OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (!context.Request.IsAjax)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "Not Found");
            }

            return BehaviorMethodAction.Execute;
        }
    }
}
