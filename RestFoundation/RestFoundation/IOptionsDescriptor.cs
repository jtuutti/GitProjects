// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;

namespace RestFoundation
{
    /// <summary>
    /// Overrides the default allowed HTTP method detection for REST services by the calling URL.
    /// </summary>
    public interface IOptionsDescriptor
    {
        /// <summary>
        /// Returns all the available HTTP methods for the provided URL.
        /// </summary>
        /// <param name="relativeUrl">The relative URL being called.</param>
        /// <returns>The sequence of available HTTP methods or null to revert to the default behavior.</returns>
        IEnumerable<HttpMethod> ReturnHttpMethodsFor(Uri relativeUrl);
    }
}
