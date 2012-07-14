﻿using System;
using System.Net;
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
        public override bool OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (!String.Equals("https", context.Request.Url.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, "HTTPS required");
            }

            return true;
        }
    }
}
