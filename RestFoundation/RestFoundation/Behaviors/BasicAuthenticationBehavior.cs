// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using RestFoundation.Security;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a basic authentication secure behavior for a service or a service method.
    /// </summary>
    public class BasicAuthenticationBehavior : SecureServiceBehavior, IAuthenticationBehavior
    {
        private const string AuthenticationType = "Basic";

        private readonly IAuthorizationManager m_authorizationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticationBehavior"/> class.
        /// </summary>
        public BasicAuthenticationBehavior() : this(Rest.Active.ServiceLocator.GetService<IAuthorizationManager>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticationBehavior"/> class.
        /// </summary>
        /// <param name="authorizationManager">The authorization manager.</param>
        public BasicAuthenticationBehavior(IAuthorizationManager authorizationManager)
        {
            if (authorizationManager == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.MissingAuthorizationManager);
            }

            m_authorizationManager = authorizationManager;
        }

        /// <summary>
        /// Called during the authorization process before a service method or behavior is executed.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        /// <returns>A service method action.</returns>
        public override BehaviorMethodAction OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            AuthorizationHeader header;

            if (!AuthorizationHeaderParser.TryParse(context.Request.Headers.TryGet("Authorization"), context.Request.Headers.ContentCharsetEncoding, out header) ||
                !AuthenticationType.Equals(header.AuthenticationType, StringComparison.OrdinalIgnoreCase))
            {
                GenerateAuthenticationHeader(context);
                return BehaviorMethodAction.Stop;
            }

            Credentials credentials = m_authorizationManager.GetCredentials(header.UserName);

            if (credentials == null || !String.Equals(header.Password, credentials.Password, StringComparison.Ordinal))
            {
                GenerateAuthenticationHeader(context);
                return BehaviorMethodAction.Stop;
            }

            context.User = new GenericPrincipal(new GenericIdentity(header.UserName, AuthenticationType), credentials.GetRoles());
            return BehaviorMethodAction.Execute;
        }

        private static void GenerateAuthenticationHeader(IServiceContext context)
        {
            context.Response.Output.Clear();
            context.Response.SetHeader("WWW-Authenticate", String.Format(CultureInfo.InvariantCulture, "{0} realm=\"{1}\"", AuthenticationType, context.Request.Url.OperationUrl));
            context.Response.SetStatus(HttpStatusCode.Unauthorized, RestResources.Unauthorized);
        }
    }
}
