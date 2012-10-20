using System;
using RestFoundation.Security;

namespace RestTest.Security
{
    public class ProxyAuthorizationManager : IAuthorizationManager
    {
        public Credentials GetCredentials(string userName)
        {
            if (String.Equals("proxyuser", userName, StringComparison.OrdinalIgnoreCase))
            {
                return new Credentials(userName, "Rest", new[] { "Proxy Users" });
            }

            return null;
        }
    }
}