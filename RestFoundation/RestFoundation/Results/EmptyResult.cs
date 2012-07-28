using System;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents an empty result that will not be processed.
    /// It is not that same as returning the null value which can be serialized.
    /// </summary>
    public sealed class EmptyResult : IResult
    {
        /// <summary>
        /// Executes the result against the provided service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        public void Execute(IServiceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
        }
    }
}
