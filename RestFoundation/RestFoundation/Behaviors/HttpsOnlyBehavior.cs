// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Reflection;

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

            if (!String.Equals("https", context.Request.Url.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                SetStatusDescription(RestResources.HttpsRequiredStatusDescription);
                return BehaviorMethodAction.Stop;
            }

            return BehaviorMethodAction.Execute;
        }
    }
}
