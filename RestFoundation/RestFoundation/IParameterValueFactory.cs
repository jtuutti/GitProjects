using System.Reflection;

namespace RestFoundation
{
    /// <summary>
    /// Defines a service method parameter value factory that orchestrates content type formatters,
    /// object type binders, route and HTTP request data to populate service method parameter values.
    /// </summary>
    public interface IParameterValueFactory
    {
        /// <summary>
        /// Creates a parameter value based on the routing and HTTP data.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="parameter">The service method parameters.</param>
        /// <param name="isResource">
        /// true if the parameter represents a REST resource; false otherwise. Only 1 resource per
        /// service method is allowed.
        /// </param>
        /// <returns>The created parameter value.</returns>
        object CreateValue(IServiceContext context, ParameterInfo parameter, out bool isResource);
    }
}
