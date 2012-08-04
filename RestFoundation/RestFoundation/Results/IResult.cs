namespace RestFoundation.Results
{
    /// <summary>
    /// Defines a service method result.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Executes the result against the provided service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        void Execute(IServiceContext context);
    }
}
