// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents an HTTPS secure behavior for a service or a service method. Any unsecure
    /// HTTP connection will set a 403 (Forbidden) HTTP status code.
    /// </summary>
    public class HttpsOnlyBehavior : SecureServiceBehavior
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
                SetStatusDescription(RestResources.HttpsRequiredStatusDescription);
                return BehaviorMethodAction.Stop;
            }

            return BehaviorMethodAction.Execute;
        }
    }
}
