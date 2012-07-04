using System;
using System.Collections.Generic;
using RestFoundation;

namespace RestTest.Security
{
    public class ServiceAuthorizationManager : IAuthorizationManager
    {
        public bool ValidateUser(string userName, string password)
        {
            return "admin".Equals(userName, StringComparison.OrdinalIgnoreCase) && "Rest".Equals(password);
        }

        public IEnumerable<string> GetRoles(string userName)
        {
            return new[] { "Administrator" };
        }
    }
}
