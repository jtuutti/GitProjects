using System.Collections.Generic;
using System.Reflection;
using RestFoundation.ServiceProxy.OperationMetadata;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Defines a service contract metadata.
    /// </summary>
    public interface IProxyMetadata
    {
        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether the operation is IP filtered.
        /// </summary>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>true if the operation is IP filtered; false otherwise.</returns>
        bool IsIPFiltered(MethodInfo serviceMethod);

        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether the operation is hidden from the service proxy.
        /// </summary>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>true if the operation is hidden from the service proxy; false otherwise.</returns>
        bool IsHidden(MethodInfo serviceMethod);

        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether the operation supports JSON serialization.
        /// </summary>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>true if the operation supports JSON serialization; false otherwise.</returns>
        bool HasJsonSupport(MethodInfo serviceMethod);

        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether the operation supports XML serialization.
        /// </summary>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>true if the operation supports XML serialization; false otherwise.</returns>
        bool HasXmlSupport(MethodInfo serviceMethod);

        /// <summary>
        /// Returns the operation description.
        /// </summary>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>The operation description.</returns>
        string GetDescription(MethodInfo serviceMethod);

        /// <summary>
        /// Returns the operation authentication information, if applicable; or null.
        /// </summary>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>The operation authentication metadata</returns>
        AuthenticationMetadata GetAuthentication(MethodInfo serviceMethod);

        /// <summary>
        /// Returns the operation HTTPS information.
        /// </summary>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>The operation HTTPS metadata</returns>
        HttpsMetadata GetHttps(MethodInfo serviceMethod);

        /// <summary>
        /// Returns the request resource example, if applicable, for the operation; or null.
        /// </summary>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>The request resource example.</returns>
        ResourceExampleMetadata GetRequestResourceExample(MethodInfo serviceMethod);

        /// <summary>
        /// Returns the response resource example, if applicable, for the operation; or null.
        /// </summary>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>The response resource example.</returns>
        ResourceExampleMetadata GetResponseResourceExample(MethodInfo serviceMethod);

        /// <summary>
        /// Returns an operation parameter with the provided name and type.
        /// </summary>
        /// <param name="serviceMethod">The service method.</param>
        /// <param name="name">The parameter name.</param>
        /// <param name="isRouteParameter">
        /// A bool indicating whether the parameter is a route parameter (true) or a query string parameter (false).
        /// </param>
        /// <returns>The operation parameter information.</returns>
        ParameterMetadata GetParameter(MethodInfo serviceMethod, string name, bool isRouteParameter);

        /// <summary>
        /// Returns all operation parameters with the provided type.
        /// </summary>
        /// <param name="serviceMethod">The service method.</param>
        /// <param name="isRouteParameter">
        /// A bool indicating whether the parameters are route parameter (true) or query string parameters (false).
        /// </param>
        /// <returns>The operation parameters.</returns>
        IList<ParameterMetadata> GetParameters(MethodInfo serviceMethod, bool isRouteParameter);

        /// <summary>
        /// Gets all additional HTTP headers associated with the operation.
        /// </summary>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>A list of additional HTTP headers for the operation.</returns>
        IList<HeaderMetadata> GetHeaders(MethodInfo serviceMethod);

        /// <summary>
        /// Gets all additional HTTP response statuses associated with the operation.
        /// </summary>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>A list of additional HTTP response statuses for the operation.</returns>
        IList<StatusCodeMetadata> GetResponseStatuses(MethodInfo serviceMethod);

        /// <summary>
        /// Initializes the service contract metadata.
        /// </summary>
        void Initialize();
    }
}
