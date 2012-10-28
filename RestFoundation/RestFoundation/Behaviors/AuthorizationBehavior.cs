// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a role based authorization behavior for a service or a service method.
    /// </summary>
    public class AuthorizationBehavior : SecureServiceBehavior
    {
        private readonly string[] m_roles;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationBehavior"/> class.
        /// </summary>
        /// <param name="roles">An array of authorized roles.</param>
        public AuthorizationBehavior(params string[] roles)
        {
            if (roles == null)
            {
                throw new ArgumentNullException("roles");
            }

            m_roles = roles;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationBehavior"/> class.
        /// </summary>
        /// <param name="roles">A sequence of authorized roles.</param>
        public AuthorizationBehavior(IEnumerable<string> roles)
        {
            if (roles == null)
            {
                throw new ArgumentNullException("roles");
            }

            m_roles = roles.ToArray();
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

            if (!serviceContext.IsAuthenticated)
            {
                SetStatusDescription(RestResources.Forbidden);
                return BehaviorMethodAction.Stop;
            }

            bool isInRole = IsUserInRole(serviceContext.User);

            if (!isInRole)
            {
                SetStatusDescription(RestResources.Forbidden);
                return BehaviorMethodAction.Stop;
            }

            return BehaviorMethodAction.Execute;
        }

        private bool IsUserInRole(IPrincipal user)
        {
            for (int i = 0; i < m_roles.Length; i++)
            {
                if (user.IsInRole(m_roles[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
