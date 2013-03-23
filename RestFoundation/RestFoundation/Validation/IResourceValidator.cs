// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System.Collections.Generic;

namespace RestFoundation.Validation
{
    /// <summary>
    /// Defines a resource validator.
    /// </summary>
    public interface IResourceValidator
    {
        /// <summary>
        /// Returns a <see cref="bool"/> value indicating whether the resource is valid along with the associated
        /// collection of errors.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="errors">A read-only collection of the validation errors.</param>
        /// <returns>true if the resource is valid; otherwise, false.</returns>
        bool IsValid(object resource, out IReadOnlyCollection<ValidationError> errors);
    }
}
