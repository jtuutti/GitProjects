﻿// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System.Collections;
using System.Linq;

namespace RestFoundation.Results
{
    /// <summary>
    /// Defines an OData provider for <see cref="T:System.Linq.IQueryable`1"/> collection
    /// results.
    /// </summary>
    public interface IODataProvider
    {
        /// <summary>
        /// Performs a query on a collection and returns the resulting collection of
        /// objects.
        /// </summary>
        /// <param name="collection">The collection to perform the query on.</param>
        /// <param name="request">The current HTTP request.</param>
        /// <returns>The resulting collection.</returns>
        IEnumerable PerformQuery(IQueryable collection, IHttpRequest request);
    }
}
