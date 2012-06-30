using System;
using System.Net;
using System.Text;

namespace RestFoundation.Runtime
{
    public class CredentialResolver : ICredentialResolver
    {
        public virtual NetworkCredential GetCredentials(IHttpRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");

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
                credentialToken = Encoding.UTF8.GetString(Convert.FromBase64String(authorizationTokens[1].Trim()));
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

            return credentialTokenItems.Length == 2 ? new NetworkCredential(credentialTokenItems[0], credentialTokenItems[1], request.Url.Host) : null;
        }
    }
}
