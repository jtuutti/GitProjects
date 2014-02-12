// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;

namespace RestFoundation
{
    /// <summary>
    /// Contains URI segments to generate an absolute URI from a relative path.
    /// </summary>
    public sealed class UriSegments
    {
        private const int DefaultHttpPort = 80;
        private const int DefaultHttpsPort = 443;
        private const string DefaultDomain = "localhost";
        private const string Http = "http";
        private const string Https = "https";

        private readonly bool m_usesHttps;
        private readonly string m_host;
        private readonly int? m_port;

        /// <summary>
        /// Initializes a new instance of the <see cref="UriSegments"/> class.
        /// </summary>
        /// <param name="usesHttps">Indicates whether the HTTPS protocol should be used.</param>
        public UriSegments(bool usesHttps) : this(usesHttps, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UriSegments"/> class.
        /// </summary>
        /// <param name="usesHttps">Indicates whether the HTTPS protocol should be used.</param>
        /// <param name="host">Specifies a different host name for the URI.</param>
        public UriSegments(bool usesHttps, string host)
        {
            m_usesHttps = usesHttps;
            m_host = host;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UriSegments"/> class.
        /// </summary>
        /// <param name="usesHttps">Indicates whether the HTTPS protocol should be used.</param>
        /// <param name="port">Specifies a different HTTP/S port for the URI.</param>
        public UriSegments(bool usesHttps, int port) : this(usesHttps, null, port)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UriSegments"/> class.
        /// </summary>
        /// <param name="usesHttps">Indicates whether the HTTPS protocol should be used.</param>
        /// <param name="host">Specifies a different host name for the URI.</param>
        /// <param name="port">Specifies a different HTTP/S port for the URI.</param>
        public UriSegments(bool usesHttps, string host, int port)
        {
            if (port <= 0)
            {
                throw new ArgumentOutOfRangeException("port");
            }

            m_usesHttps = usesHttps;
            m_host = host;
            m_port = port;
        }

        /// <summary>
        /// Creates a <see cref="UriSegments"/> instance from the provided <see cref="IHttpRequest"/> information.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <returns>The generated <see cref="UriSegments"/> instance.</returns>
        public static UriSegments CreateFromHttpRequest(IHttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return new UriSegments(request.IsSecure, request.ServerVariables.ServerName, request.ServerVariables.ServerPort);
        }

        /// <summary>
        /// Returns an absolute URI generated from the segments and the provided relative path.
        /// </summary>
        /// <param name="relativePath">A relative path.</param>
        /// <returns>A <see cref="string"/> containg the generated URI.</returns>
        /// <exception cref="InvalidOperationException">If no host name could be determined.</exception>
        public string GenerateUri(string relativePath)
        {
            return GenerateUri(DefaultDomain, relativePath);
        }

        internal string GenerateUri(string host, string relativePath)
        {
            if (!String.IsNullOrWhiteSpace(m_host))
            {
                host = m_host;
            }

            if (String.IsNullOrWhiteSpace(host))
            {
                throw new InvalidOperationException(Resources.Global.UndeterminedUriHostName);
            }

            var uriBuilder = new UriBuilder
            {
                Scheme = m_usesHttps ? Https : Http,
                Host = host.Trim()
            };           

            if (m_port.HasValue && m_port.Value > 0 &&
                !(!m_usesHttps && m_port.Value == DefaultHttpPort) &&
                !(m_usesHttps && m_port.Value == DefaultHttpsPort))
            {
                uriBuilder.Port = m_port.Value;
            }

            if (!String.IsNullOrWhiteSpace(relativePath))
            {
                uriBuilder.Path = relativePath;
            }

            return uriBuilder.ToString();
        }
    }
}
