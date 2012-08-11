using System;
using RestFoundation.Security;

namespace RestFoundation.Tests.Implementation.Authorization
{
    public class TestAuthorizationManager : IAuthorizationManager
    {
        public Credentials GetCredentials(string userName)
        {
            if (String.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }

            return new Credentials(userName, "test123", new[] { "Tester" });
        }
    }
}
