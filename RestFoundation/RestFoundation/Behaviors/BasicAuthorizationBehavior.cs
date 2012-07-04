using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using RestFoundation.Runtime;

namespace RestFoundation.Behaviors
{
    public sealed class BasicAuthorizationBehavior : ServiceSecurityBehavior
    {
        private const string AuthenticationType = "Basic";

        private readonly IAuthorizationManager m_authorizationManager;
        private readonly AuthorizationHeaderParser m_headerParser;

        public BasicAuthorizationBehavior()
        {
            m_authorizationManager = Rest.Active.CreateObject<IAuthorizationManager>();

            if (m_authorizationManager == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No authorization manager could be found");
            }

            m_headerParser = new AuthorizationHeaderParser();
        }

        public override bool OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            AuthorizationHeader header;

            if (!m_headerParser.TryParse(context.Request.Headers.Authorization, context.Request.Headers.ContentCharsetEncoding, out header) ||
                !AuthenticationType.Equals(header.AuthenticationType, StringComparison.OrdinalIgnoreCase) ||
                !m_authorizationManager.ValidateUser(header.UserName, header.Password))
            {
                context.Response.Output.Clear();
                context.Response.SetHeader("WWW-Authenticate", String.Format(CultureInfo.InvariantCulture, "{0} realm=\"{1}\"", AuthenticationType, context.Request.Url.ServiceUrl));
                context.Response.SetStatus(HttpStatusCode.Unauthorized, "Unauthorized");

                return false;
            }

            context.User = new GenericPrincipal(new GenericIdentity(header.UserName, AuthenticationType), m_authorizationManager.GetRoles(header.UserName).ToArray());

            return true;
        }
    }
}
