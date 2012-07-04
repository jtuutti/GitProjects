using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;

namespace RestFoundation.Behaviors
{
    public sealed class BasicAuthorizationBehavior : ServiceSecurityBehavior
    {
        private readonly IAuthorizationManager m_authorizationManager;

        public BasicAuthorizationBehavior()
        {
            m_authorizationManager = Rest.Active.CreateObject<IAuthorizationManager>();

            if (m_authorizationManager == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No authorization manager could be found");
            }
        }

        public override bool OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            Tuple<string, string> requestedCredentials = GetAuthenticationInfo(context.Request);

            if (requestedCredentials == null || !m_authorizationManager.ValidateUser(requestedCredentials.Item1, requestedCredentials.Item2))
            {
                CreateAuthenticationRequest(context);
                return false;
            }

            context.User = new GenericPrincipal(new GenericIdentity(requestedCredentials.Item1, "Basic"),
                                                m_authorizationManager.GetRoles(requestedCredentials.Item1).ToArray());

            return true;
        }

        private static void CreateAuthenticationRequest(IServiceContext context)
        {
            context.Response.SetHeader("WWW-Authenticate", String.Format(CultureInfo.InvariantCulture, "Basic realm=\"{0}\"", context.Request.Url.ServiceUrl));
            context.Response.SetStatus(HttpStatusCode.Unauthorized, "Unauthorized");
        }

        private static Tuple<string, string> GetAuthenticationInfo(IHttpRequest request)
        {
            string authorizationHeader = request.Headers.Authorization;

            if (String.IsNullOrEmpty(authorizationHeader))
            {
                return null;
            }

            string[] authorizationTokens = authorizationHeader.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (authorizationTokens.Length < 2 || !authorizationTokens[0].Trim().Equals("Basic", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            string credentialToken;

            try
            {
                Encoding encoding = request.Headers.ContentCharsetEncoding ?? Encoding.UTF8;

                credentialToken = encoding.GetString(Convert.FromBase64String(authorizationTokens[1].Trim()));
            }
            catch (Exception)
            {
                credentialToken = null;
            }

            if (String.IsNullOrEmpty(credentialToken))
            {
                return null;
            }

            string[] credentialTokenItems = credentialToken.Split(':');

            if (credentialTokenItems.Length != 2 || credentialTokenItems[0].Length == 0)
            {
                return null;
            }

            return Tuple.Create(credentialTokenItems[0], credentialTokenItems[1]);
        }
    }
}
