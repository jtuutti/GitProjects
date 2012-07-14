using System;
using System.Linq;
using System.Reflection;
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
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        public override bool OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            if (context == null) throw new ArgumentNullException("context");

            var ranges = IPAddressRange.GetConfiguredRanges(m_sectionName).ToList();

            if (ranges.Count == 0)
            {
                return false;
            }

            bool isAllowed = false;

            foreach (var range in ranges)
            {
                if (range.IsInRange(context.GetHttpContext().Request.UserHostAddress))
                {
                    isAllowed = true;
                    break;
                }
            }

            return isAllowed;
        }
    }
}
