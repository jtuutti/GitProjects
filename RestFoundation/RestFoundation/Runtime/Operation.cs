// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a REST operation associated to a service method.
    /// </summary>
    [Serializable]
    public class Operation
    {
        /// <summary>
        /// Gets a relative URL template.
        /// </summary>
        public string RelativeUrlTemplate { get; set; }

        /// <summary>
        /// Gets an HTTP method.
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// Gets an optional description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets an optional sample URL.
        /// </summary>
        public string SampleUrl { get; set; }
    }
}
