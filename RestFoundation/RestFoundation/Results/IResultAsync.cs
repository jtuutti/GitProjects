using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestFoundation.Results
{
    /// <summary>
    /// Defines an asynchronous service method result.
    /// </summary>
    public interface IResultAsync : IResult
    {
        /// <summary>
        /// Executes the result against the provided service synchronously.
        /// Asynchronous method should throw a <see cref="NotSupportedException"/> and implement
        /// the <see cref="ExecuteAsync"/> method instead.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <exception cref="NotSupportedException">
        /// When called from a service method result that implements the <see cref="IResultAsync"/>
        /// interface.
        /// </exception>
        new void Execute(IServiceContext context);

        /// <summary>
        /// Executes the result against the provided service context asynchronously.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task executing the result.</returns>
        Task ExecuteAsync(IServiceContext context, CancellationToken cancellationToken);
    }
}
