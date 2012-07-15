namespace RestFoundation.Collections
{
    /// <summary>
    /// Defines a server variable collection.
    /// </summary>
    public interface IServerVariableCollection : IStringValueCollection
    {
        /// <summary>
        /// Gets the IIS application pool identifier.
        /// </summary>
        string ApplicationPoolId { get; }

        /// <summary>
        /// Gets the HTTP protocol version.
        /// </summary>
        string HttpVersion { get; }

        /// <summary>
        /// Gets the local server IP address.
        /// </summary>
        string LocalAddress { get; }

        /// <summary>
        /// Gets the user IP address.
        /// </summary>
        string RemoteAddress { get; }

        /// <summary>
        /// Gets the user remote port.
        /// </summary>
        int RemotePort { get; }

        /// <summary>
        /// Gets the remote user name.
        /// </summary>
        string RemoteUser { get; }

        /// <summary>
        /// Gets the server name.
        /// </summary>
        string ServerName { get; }

        /// <summary>
        /// Gets the server port.
        /// </summary>
        int ServerPort { get; }
    }
}
