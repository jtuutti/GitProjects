using System;
using System.Collections.Generic;
using System.Reflection;
using RestFoundation.Acl;

namespace RestFoundation.Behaviors
{
    public class AclBehavior : ServiceSecurityBehavior
    {
        private readonly string m_sectionName;

        public AclBehavior(string nameValueSectionName)
        {
            if (String.IsNullOrWhiteSpace(nameValueSectionName))
            {
                throw new ArgumentNullException("nameValueSectionName");
            }

            m_sectionName = nameValueSectionName;
        }

        public override bool OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            IEnumerable<IPAddressRange> ranges = IPAddressRange.GetConfiguredRanges(m_sectionName);

            if (ranges == null)
            {
                return true;
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
