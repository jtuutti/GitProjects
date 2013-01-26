// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy session data.
    /// </summary>
    public class ProxySession
    {
        /// <summary>
        /// Gets or sets the base service URL.
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the relative endpoint URL.
        /// </summary>
        public string OperationUrl { get; set; }

        /// <summary>
        /// Gets or sets the resource format.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the HTTP verb.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the headers associated with the service operation.
        /// </summary>
        public string Headers { get; set; }

        /// <summary>
        /// Gets or sets the resource body.
        /// </summary>
        public string Body { get; set; }
    }
}
