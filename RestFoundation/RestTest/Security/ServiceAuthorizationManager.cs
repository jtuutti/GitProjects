using System;
using RestFoundation;
using RestFoundation.Security;

namespace RestTest.Security
{
    public class ServiceAuthorizationManager : IAuthorizationManager
    {
        public Credentials GetCredentials(string userName)
        {
            if (String.Equals("admin", userName, StringComparison.OrdinalIgnoreCase))
            {
                return new Credentials(userName, "Rest", new[] { "Administrators" });
            }

            return null;
        }
    }
}
