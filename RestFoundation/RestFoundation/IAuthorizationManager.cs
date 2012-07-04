using System.Collections.Generic;

namespace RestFoundation
{
    public interface IAuthorizationManager
    {
        bool ValidateUser(string userName, string password);
        IEnumerable<string> GetRoles(string userName);
    }
}
