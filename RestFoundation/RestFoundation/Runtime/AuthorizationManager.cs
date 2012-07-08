﻿using System.Collections.Generic;

namespace RestFoundation.Runtime
{
    public class AuthorizationManager : IAuthorizationManager
    {
        public virtual string GetPassword(string userName)
        {
            return null;
        }

        public virtual IEnumerable<string> GetRoles(string userName)
        {
            return new string[0];
        }
    }
}
