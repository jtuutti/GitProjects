using System;
using System.Collections.Specialized;

namespace RestFoundation.Security
{
    internal sealed class AuthorizationHeader
    {
        public AuthorizationHeader(string authenticationType, string userName, NameValueCollection parameters)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentNullException("userName");
            if (parameters == null) throw new ArgumentNullException("parameters");

            AuthenticationType = authenticationType;
            UserName = userName;
            Parameters = parameters;
        }

        public string AuthenticationType { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; set; }
        public NameValueCollection Parameters { get; private set; }
    }
}
