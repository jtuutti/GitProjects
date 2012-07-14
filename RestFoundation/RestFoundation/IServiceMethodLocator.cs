using RestFoundation.Runtime;

namespace RestFoundation
{
    /// <summary>
    /// Defines a service method locator.
    /// </summary>
    public interface IServiceMethodLocator
    {
        /// <summary>
        /// Locates the service method associated with the provided REST handler and returns the associated data.
        /// </summary>
        /// <param name="handler">A REST handler associated with HTTP request.</param>
        /// <returns>The service method data.</returns>
        ServiceMethodLocatorData Locate(IRestHandler handler);
    }
}
