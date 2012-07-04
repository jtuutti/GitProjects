using System.Collections.Generic;

namespace RestFoundation.Runtime
{
    public class AuthorizationManager : IAuthorizationManager
    {
        public virtual bool ValidateUser(string userName, string password)
        {
            return false;
        }

        public virtual IEnumerable<string> GetRoles(string userName)
        {
            return new string[0];
        }
    }
}
