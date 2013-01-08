// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation.Results
{
    /// <summary>
    /// Defines a service method result executor.
    /// </summary>
    public interface IResultExecutor
    {
        /// <summary>
        /// Executes the result in the provided service context.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="methodReturnType">The service method return type.</param>
        /// <param name="context">The service context.</param>
        void Execute(IResult result, Type methodReturnType, IServiceContext context);
    }
}