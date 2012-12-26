using System.Diagnostics.CodeAnalysis;

namespace RestFoundation.ServiceProxy.OperationMetadata
{
    /// <summary>
    /// Represents an authentication metadata.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class AuthenticationMetadata
    {
        /// <summary>
        /// Gets or sets the authentication type.
        /// </summary>
        public AuthenticationType Type { get; set; }

        /// <summary>
        /// Gets or sets the default username.
        /// </summary>
        public string DefaultUserName { get; set; }

        /// <summary>
        /// Gets or sets the relative URL to match.
        /// </summary>
        public string RelativeUrlToMatch { get; set; }
    }
}