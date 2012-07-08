using System;
using System.Collections.Specialized;

namespace RestFoundation.Collections.Concrete
{
    public class ServerVariableCollection : StringValueCollection, IServerVariableCollection
    {
        internal ServerVariableCollection(NameValueCollection values) : base(values)
        {
            ApplicationPoolId = TryGet("APP_POOL_ID");
            HttpVersion = TryGet("HTTP_VERSION");
            LocalAddress = TryGet("LOCAL_ADDR");
            Referrer = TryGet("HTTP_REFERER");
            RemoteAddress = TryGet("REMOTE_ADDR");
            RemoteUser = TryGet("REMOTE_USER");
            ServerName = TryGet("SERVER_NAME");
            UserAgent = TryGet("HTTP_USER_AGENT");

            int remotePort, serverPort;

            if (Int32.TryParse(TryGet("REMOTE_PORT"), out remotePort))
            {
                RemotePort = remotePort;
            }

            if (Int32.TryParse(TryGet("SERVER_PORT"), out serverPort))
            {
                ServerPort = serverPort;
            }
        }

        public string ApplicationPoolId { get; protected set; }
        public string HttpVersion { get; protected set; }
        public string LocalAddress { get; protected set; }
        public string Referrer { get; protected set; }
        public string RemoteAddress { get; protected set; }
        public int RemotePort { get; protected set; }
        public string RemoteUser { get; protected set; }
        public string ServerName { get; protected set; }
        public int ServerPort { get; protected set; }
        public string UserAgent { get; protected set; }
    }
}
