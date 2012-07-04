using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using RestFoundation.Runtime;

namespace RestFoundation.Behaviors
{
    public sealed class BasicAuthorizationBehavior : ServiceSecurityBehavior
    {
        private static readonly IEqualityComparer<NetworkCredential> comparer = new NetworkCredentialEqualityComparer();

        private readonly IEnumerable<NetworkCredential> m_allowedCredentials;

        public BasicAuthorizationBehavior(IEnumerable<NetworkCredential> allowedCredentials)
        {
            if (allowedCredentials == null) throw new ArgumentNullException("allowedCredentials");

            m_allowedCredentials = allowedCredentials;
        }

        public override bool OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            if (context.Request.Credentials == null || String.IsNullOrWhiteSpace(context.Request.Credentials.UserName) ||
                String.IsNullOrEmpty(context.Request.Credentials.Password))
            {
                CreateAuthenticationRequest(context);
                return false;
            }

            var requestedCredentials = new NetworkCredential(context.Request.Credentials.UserName, context.Request.Credentials.Password);
            var allowedCredentials = new HashSet<NetworkCredential>(m_allowedCredentials, comparer);

            if (!allowedCredentials.Contains(requestedCredentials))
            {
                CreateAuthenticationRequest(context);
                return false;
            }

            context.User = new GenericPrincipal(new GenericIdentity(requestedCredentials.UserName, requestedCredentials.Password), new string[0]);
            return true;
        }

        private static void CreateAuthenticationRequest(IServiceContext context)
        {
            context.Response.SetHeader("WWW-Authenticate", String.Format(CultureInfo.InvariantCulture, "Basic realm=\"{0}\"", context.Request.Url.ServiceUrl));
            context.Response.SetStatus(HttpStatusCode.Unauthorized, "Unauthorized");
        }
    }
}
