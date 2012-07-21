using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using RestFoundation.Security;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a basic authorization secure behavior for a service or a service method.
    /// </summary>
    public class BasicAuthorizationBehavior : SecureServiceBehavior, IAuthenticationBehavior
    {
        private const string AuthenticationType = "Basic";

        private readonly IAuthorizationManager m_authorizationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthorizationBehavior"/> class.
        /// </summary>
        public BasicAuthorizationBehavior() : this(Rest.Active.DependencyResolver.Resolve<IAuthorizationManager>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthorizationBehavior"/> class.
        /// </summary>
        /// <param name="authorizationManager">The authorization manager.</param>
        public BasicAuthorizationBehavior(IAuthorizationManager authorizationManager)
        {
            if (authorizationManager == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No authorization manager could be found");
            }

            m_authorizationManager = authorizationManager;
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
            context.Response.SetHeader("WWW-Authenticate", String.Format(CultureInfo.InvariantCulture, "{0} realm=\"{1}\"", AuthenticationType, context.Request.Url.OperationUrl));
            context.Response.SetStatus(HttpStatusCode.Unauthorized, "Unauthorized");
        }
    }
}
