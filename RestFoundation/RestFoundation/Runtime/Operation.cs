// <copyright>
// Dmitry Starosta, 2012-2014
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
        /// Gets or sets a relative URL template.
        /// </summary>
        public string RelativeUrlTemplate { get; set; }

        /// <summary>
        /// Gets or sets an HTTP method.
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// Gets or sets an optional description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets an optional sample URL.
        /// </summary>
        public string SampleUrl { get; set; }
    }
}
