﻿// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Net;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a service method that can only be called over HTTPS/SSL.
    /// HTTP connection will set a 403 (Forbidden) HTTP status code if the connection is not secure.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class HttpsOnlyAttribute : ServiceMethodBehaviorAttribute
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

            if (!String.Equals("https", serviceContext.Request.Url.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                serviceContext.Response.SetStatus(HttpStatusCode.Forbidden, RestResources.HttpsRequiredStatusDescription);
                return BehaviorMethodAction.Stop;
            }

            return BehaviorMethodAction.Execute;
        }
    }
}
