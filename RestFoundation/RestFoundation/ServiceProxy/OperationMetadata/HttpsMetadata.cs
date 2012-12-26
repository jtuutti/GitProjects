using System.Diagnostics.CodeAnalysis;

namespace RestFoundation.ServiceProxy.OperationMetadata
{
    /// <summary>
    /// Represents the HTTPS metadata.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class HttpsMetadata
    {
        /// <summary>
        /// Gets or sets the port number.
        /// </summary>
        public int Port { get; set; }
    }
}