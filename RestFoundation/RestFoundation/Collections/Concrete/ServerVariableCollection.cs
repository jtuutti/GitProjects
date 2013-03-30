// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Specialized;

namespace RestFoundation.Collections.Concrete
{
    /// <summary>
    /// Represents a server variable collection.
    /// </summary>
    public class ServerVariableCollection : StringValueCollection, IServerVariableCollection
    {
        private const char ForwardedAddressSeparator = ',';

        internal ServerVariableCollection(NameValueCollection collection) : base(collection)
        {
            ApplicationPoolId = TryGet("APP_POOL_ID");
            HttpVersion = TryGet("HTTP_VERSION");
            LocalAddress = TryGet("LOCAL_ADDR");
            RemoteAddress = TryGetRemoteAddress();
            RemoteUser = TryGet("REMOTE_USER");
            ServerName = TryGet("SERVER_NAME");

            SetPorts();
        }

        /// <summary>
        /// Gets the IIS application pool identifier.
        /// </summary>
        public string ApplicationPoolId { get; protected set; }

        /// <summary>
        /// Gets the HTTP protocol version.
        /// </summary>
        public string HttpVersion { get; protected set; }

        /// <summary>
        /// Gets the local server IP address.
        /// </summary>
        public string LocalAddress { get; protected set; }

        /// <summary>
        /// Gets the user IP address.
        /// </summary>
        public string RemoteAddress { get; protected set; }

        /// <summary>
        /// Gets the user remote port.
        /// </summary>
        public int RemotePort { get; protected set; }

        /// <summary>
        /// Gets the remote user name.
        /// </summary>
        public string RemoteUser { get; protected set; }

        /// <summary>
        /// Gets the server name.
        /// </summary>
        public string ServerName { get; protected set; }

        /// <summary>
        /// Gets the server port.
        /// </summary>
        public int ServerPort { get; protected set; }

        private string TryGetRemoteAddress()
        {
            string forwardedAddress = TryGet("HTTP_X_FORWARDED_FOR");

            if (String.IsNullOrWhiteSpace(forwardedAddress) || String.Equals("unknown", forwardedAddress, StringComparison.OrdinalIgnoreCase) ||
                forwardedAddress.IndexOf(ForwardedAddressSeparator) == 0)
            {
                return TryGet("REMOTE_ADDR");
            }

            if (forwardedAddress.IndexOf(ForwardedAddressSeparator) > 0)
            {
                string[] forwardedAddresses = forwardedAddress.Split(new[] { ForwardedAddressSeparator }, StringSplitOptions.RemoveEmptyEntries);

                return forwardedAddresses[0].Trim();
            }

            return forwardedAddress;
        }

        private void SetPorts()
        {
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
    }
}
