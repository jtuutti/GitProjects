using System;
using System.Net;
using System.Reflection;

namespace RestFoundation.Behaviors
{
    public class HttpsOnlyBehavior : ServiceSecurityBehavior
    {
        public override bool OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            if (!String.Equals("https", context.Request.Url.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, "HTTPS required");
            }

            return true;
        }
    }
}
