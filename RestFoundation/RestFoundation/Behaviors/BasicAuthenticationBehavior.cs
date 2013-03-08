// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Globalization;
using System.Net;
using System.Security.Principal;
using RestFoundation.Runtime;
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
        public BasicAuthenticationBehavior() : this(Rest.Configuration.ServiceLocator.GetService<IAuthorizationManager>())
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
                throw new HttpResponseException(HttpStatusCode.InternalServerError, Resources.Global.MissingAuthorizationManager);
            }

            m_authorizationManager = authorizationManager;
        }

        /// <summary>
        /// Called during the authorization process before a service method or behavior is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method authorizing" behavior context.</param>
        /// <returns>A service method action.</returns>
        public override BehaviorMethodAction OnMethodAuthorizing(IServiceContext serviceContext, MethodAuthorizingContext behaviorContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException("serviceContext");
            }

            AuthorizationHeader header;

            if (!AuthorizationHeaderParser.TryParse(serviceContext.Request.Headers.TryGet("Authorization"), serviceContext.Request.Headers.ContentCharsetEncoding, out header) ||
                !AuthenticationType.Equals(header.AuthenticationType, StringComparison.OrdinalIgnoreCase))
            {
                GenerateAuthenticationHeader(serviceContext);
                return BehaviorMethodAction.Stop;
            }

            Credentials credentials = m_authorizationManager.GetCredentials(header.UserName);

            if (credentials == null || !String.Equals(header.Password, credentials.Password, StringComparison.Ordinal))
            {
                GenerateAuthenticationHeader(serviceContext);
                return BehaviorMethodAction.Stop;
            }

            serviceContext.User = new GenericPrincipal(new GenericIdentity(header.UserName, AuthenticationType), credentials.GetRoles());
            return BehaviorMethodAction.Execute;
        }

        private static void GenerateAuthenticationHeader(IServiceContext serviceContext)
        {
            serviceContext.Response.Output.Clear();
            serviceContext.Response.SetHeader("WWW-Authenticate", String.Format(CultureInfo.InvariantCulture, "{0} realm=\"{1}\"", AuthenticationType, serviceContext.Request.Url.OperationUrl));
            serviceContext.Response.SetStatus(HttpStatusCode.Unauthorized, Resources.Global.Unauthorized);
        }
    }
}
