using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace RestFoundation.Acl
{
    internal static class AclValidator
    {
        public static void Validate(HttpContext context, string sectionName)
        {
            if (!IPValidated(context, sectionName))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, "Access denied due to ACL restrictions");
            }

            HttpCachePolicy cachePolicy = context.Response.Cache;
            cachePolicy.SetProxyMaxAge(new TimeSpan(0));
            cachePolicy.AddValidationCallback(CacheValidationHandler, sectionName);
        }

        private static bool IPValidated(HttpContext context, string sectionName)
        {
            List<IPAddressRange> ranges = IPAddressRange.GetConfiguredRanges(sectionName).ToList();

            if (ranges.Count == 0)
            {
                return true;
            }

            bool isAllowed = false;

            foreach (var range in ranges)
            {
                if (range.IsInRange(context.Request.UserHostAddress))
                {
                    isAllowed = true;
                    break;
                }
            }

            return isAllowed;
        }

        private static void CacheValidationHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            if (!IPValidated(context, (string) data))
            {
                validationStatus = HttpValidationStatus.IgnoreThisRequest;
                return;
            }

            validationStatus = HttpValidationStatus.Valid;
        }
    }
}
