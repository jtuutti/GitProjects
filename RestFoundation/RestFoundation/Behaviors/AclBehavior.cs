// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Linq;
using RestFoundation.Security;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents an IP filtering secure behavior for a service or a service method.
    /// </summary>
    public class AclBehavior : SecureServiceBehavior
    {
        private readonly string m_sectionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="AclBehavior"/> class.
        /// </summary>
        /// <param name="nameValueSectionName">The Web.Config name-value section name containing the ACL list.</param>
        public AclBehavior(string nameValueSectionName)
        {
            if (String.IsNullOrWhiteSpace(nameValueSectionName))
            {
                throw new ArgumentNullException("nameValueSectionName");
            }

            m_sectionName = nameValueSectionName;
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

            var ranges = IPAddressRange.GetConfiguredRanges(m_sectionName).ToList();

            if (ranges.Count == 0)
            {
                return BehaviorMethodAction.Stop;
            }

            bool isAllowed = false;

            foreach (var range in ranges)
            {
                if (range.IsInRange(serviceContext.GetHttpContext().Request.UserHostAddress))
                {
                    isAllowed = true;
                    break;
                }
            }

            return isAllowed ? BehaviorMethodAction.Execute : BehaviorMethodAction.Stop;
        }
    }
}
