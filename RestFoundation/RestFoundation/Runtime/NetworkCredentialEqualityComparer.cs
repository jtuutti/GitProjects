using System;
using System.Collections.Generic;
using System.Net;

namespace RestFoundation.Runtime
{
    internal sealed class NetworkCredentialEqualityComparer : EqualityComparer<NetworkCredential>
    {
        public override bool Equals(NetworkCredential x, NetworkCredential y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return String.Equals(x.UserName, y.UserName, StringComparison.OrdinalIgnoreCase) && String.Equals(x.Password, y.Password);
        }

        public override int GetHashCode(NetworkCredential obj)
        {
            if (obj == null)
            {
                return 0;
            }

            int result = obj.UserName != null ? obj.UserName.ToUpperInvariant().GetHashCode() : 0;
            result = (result * 397) ^ (obj.Password != null ? obj.Password.GetHashCode() : 0);

            return result;
        }
    }
}
