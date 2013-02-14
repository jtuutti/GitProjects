using System;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents data from a Service Proxy response.
    /// </summary>
    public sealed class ProxyResponseData
    {
        /// <summary>
        /// Gets or sets the response data.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the HTTP protocol version.
        /// </summary>
        public string ProtocolVersion { get; set; }

        /// <summary>
        /// Gets or sets the service execution duration.
        /// </summary>
        public TimeSpan Duration { get; set; }
    }
}
