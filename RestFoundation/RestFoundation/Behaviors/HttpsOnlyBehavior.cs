// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents an HTTPS secure behavior for a service or a service method. Any unsecure
    /// HTTP connection will set a 403 (Forbidden) HTTP status code if the connection is not secure.
    /// </summary>
    public class HttpsOnlyBehavior : SecureServiceBehavior
    {
        private readonly bool m_enableLoadBalancerSupport;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpsOnlyBehavior"/> class.
        /// Load balancer support is disabled by default.
        /// </summary>
        public HttpsOnlyBehavior() : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpsOnlyBehavior"/> class.
        /// </summary>
        /// <param name="enableLoadBalancerSupport">
        /// A value indicating whether the load balancer support for forwarding HTTPS traffic over an HTTP
        /// channel is allowed.
        /// </param>
        public HttpsOnlyBehavior(bool enableLoadBalancerSupport)
        {
            m_enableLoadBalancerSupport = enableLoadBalancerSupport;
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

            if (m_enableLoadBalancerSupport && !serviceContext.Request.IsSecure)
            {
                SetStatusDescription(RestResources.HttpsRequiredStatusDescription);
                return BehaviorMethodAction.Stop;               
            }

            if (!m_enableLoadBalancerSupport && !String.Equals("https", serviceContext.Request.Url.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                SetStatusDescription(RestResources.HttpsRequiredStatusDescription);
                return BehaviorMethodAction.Stop;               
            }

            return BehaviorMethodAction.Execute;
        }
    }
}
