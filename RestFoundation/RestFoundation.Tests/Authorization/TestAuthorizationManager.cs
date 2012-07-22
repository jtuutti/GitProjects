using System.Collections.Generic;

namespace RestFoundation.Tests
{
    public class TestAuthorizationManager : IAuthorizationManager
    {
        public string GetPassword(string userName)
        {
            return "test123";
        }

        public IEnumerable<string> GetRoles(string userName)
        {
            return new[] { "Tester" };
        }
    }
}
