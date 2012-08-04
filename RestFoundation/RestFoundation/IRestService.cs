using System.Collections.Generic;
using RestFoundation.Behaviors;

namespace RestFoundation
{
    /// <summary>
    /// Defines an optional interface for a restful service that allows
    /// to associate behaviors to a specific service implementation.
    /// </summary>
    public interface IRestService
    {
        /// <summary>
        /// Gets an exception handler associated with the service implementation.
        /// </summary>
        IServiceExceptionHandler ExceptionHandler { get; }

        /// <summary>
        /// Gets a sequence of service behaviors associated with the service implementation.
        /// </summary>
        IEnumerable<IServiceBehavior> Behaviors { get; }
    }
}
