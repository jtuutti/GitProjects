// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System.Collections;
using System.Linq;

namespace RestFoundation.Runtime
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
        /// <param name="context">The service context.</param>
        /// <param name="collection">The collection to perform the query on.</param>
        /// <returns>The resulting collection.</returns>
        IEnumerable PerformQuery(IServiceContext context, IQueryable collection);
    }
}
