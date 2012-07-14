namespace RestFoundation
{
    /// <summary>
    /// Defines a result factory.
    /// </summary>
    public interface IResultFactory
    {
        /// <summary>
        /// Creates an <see cref="IResult"/> instance from a POCO object returned by
        /// the service method.
        /// </summary>
        /// <param name="returnedObj">The returned object.</param>
        /// <param name="context">The service context.</param>
        /// <returns>The created result instance.</returns>
        IResult Create(object returnedObj, IServiceContext context);
    }
}
