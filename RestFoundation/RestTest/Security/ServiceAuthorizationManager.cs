using System;
using System.Collections.Generic;
using RestFoundation;

namespace RestTest.Security
{
    public class ServiceAuthorizationManager : IAuthorizationManager
    {
        public string GetPassword(string userName)
        {
            if (String.Equals("admin", userName, StringComparison.OrdinalIgnoreCase))
            {
                return "Rest";
            }

            return null;
        }

        public IEnumerable<string> GetRoles(string userName)
        {
            return new[] { "Administrator" };
        }
    }
}
