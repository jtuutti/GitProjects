// <copyright>
// Dmitry Starosta, 2012-2013
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
        /// Gets the HTTP status code in case of a security exception.
        /// </summary>
        public override HttpStatusCode StatusCode
        {
            get
            {
                return HttpStatusCode.Forbidden;
            }
        }

        /// <summary>
        /// Gets the HTTP status description in case of a security exception.
        /// </summary>
        public override string StatusDescription
        {
            get
            {
                return Resources.Global.HttpsRequiredStatusDescription;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the load balancer support for forwarding HTTPS traffic
        /// over HTTP channel is allowed.
        /// </summary>
        public bool EnableLoadBalancerSupport { get; set; }

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

            if (EnableLoadBalancerSupport && !serviceContext.Request.IsSecure)
            {
                return BehaviorMethodAction.Stop;
            }

            if (!EnableLoadBalancerSupport && !String.Equals("https", serviceContext.Request.Url.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return BehaviorMethodAction.Stop;
            }

            return BehaviorMethodAction.Execute;
        }
    }
}
