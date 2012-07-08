using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using RestFoundation.Runtime;

namespace RestFoundation.Behaviors
{
    public class BasicAuthorizationBehavior : ServiceSecurityBehavior
    {
        private const string AuthenticationType = "Basic";

        private readonly IAuthorizationManager m_authorizationManager;

        public BasicAuthorizationBehavior() : this(Rest.Active.CreateObject<IAuthorizationManager>())
        {
        }

        public BasicAuthorizationBehavior(IAuthorizationManager authorizationManager)
        {
            if (authorizationManager == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No authorization manager could be found");
            }

            m_authorizationManager = authorizationManager;
        }

        public override bool OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            if (context == null) throw new ArgumentNullException("context");

            AuthorizationHeader header;

            if (!AuthorizationHeaderParser.TryParse(context.Request.Headers.Authorization, context.Request.Headers.ContentCharsetEncoding, out header) ||
                !AuthenticationType.Equals(header.AuthenticationType, StringComparison.OrdinalIgnoreCase) ||
                !String.Equals(m_authorizationManager.GetPassword(header.UserName), header.Password, StringComparison.Ordinal))
            {
                GenerateAuthenticationHeader(context);
                return false;
            }

            context.User = new GenericPrincipal(new GenericIdentity(header.UserName, AuthenticationType), m_authorizationManager.GetRoles(header.UserName).ToArray());
            return true;
        }

        private static void GenerateAuthenticationHeader(IServiceContext context)
        {
            context.Response.Output.Clear();
            context.Response.SetHeader("WWW-Authenticate", String.Format(CultureInfo.InvariantCulture, "{0} realm=\"{1}\"", AuthenticationType, context.Request.Url.ServiceUrl));
            context.Response.SetStatus(HttpStatusCode.Unauthorized, "Unauthorized");
        }
    }
}
