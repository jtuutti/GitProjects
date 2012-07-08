using System.Collections.Generic;

namespace RestFoundation
{
    public interface IAuthorizationManager
    {
        string GetPassword(string userName);
        IEnumerable<string> GetRoles(string userName);
    }
}
