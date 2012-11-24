using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Defines the method metadata for a service operation.
    /// </summary>
    public interface IMethodMetadata
    {
        /// <summary>
        /// Sets the authentication for the service operation.
        /// </summary>
        /// <param name="type">The authentication type.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetAuthentication(AuthenticationType type);

        /// <summary>
        /// Sets the authentication for the service operation.
        /// </summary>
        /// <param name="type">The authentication type.</param>
        /// <param name="defaultUserName">The default user name.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetAuthentication(AuthenticationType type, string defaultUserName);

        /// <summary>
        /// Sets the authentication for the service operation.
        /// </summary>
        /// <param name="type">The authentication type.</param>
        /// <param name="defaultUserName">The default user name.</param>
        /// <param name="relativeUrlToMatch">A relative URL to apply authentication to.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetAuthentication(AuthenticationType type, string defaultUserName, string relativeUrlToMatch);

        /// <summary>
        /// Sets the description for the service operation.
        /// </summary>
        /// <param name="operationDescription">The description.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetDescription(string operationDescription);

        /// <summary>
        /// Sets an additional HTTP header for the service operation.
        /// </summary>
        /// <param name="header">The HTTP header for the proxy.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetHeader(ProxyHeader header);

        /// <summary>
        /// Sets an additional HTTP header for the service operation.
        /// </summary>
        /// <param name="name">The HTTP header name.</param>
        /// <param name="value">The HTTP header value.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetHeader(string name, string value);

        /// <summary>
        /// Sets additional HTTP headers for the service operation.
        /// </summary>
        /// <param name="headers">The list of HTTP headers for the proxy.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetHeaders(IList<ProxyHeader> headers);

        /// <summary>
        /// Marks all service operations as IP filtered.
        /// </summary>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetHidden();

        /// <summary>
        /// Sets the service operation to require secure HTTPS connection with port 443.
        /// </summary>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetHttps();

        /// <summary>
        /// Sets the service operation as HTTPS specific.
        /// </summary>
        /// <param name="port">The TCP port number.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetHttps(int port);

        /// <summary>
        /// Marks the service operation as IP filtered.
        /// </summary>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetIPFiltered();

        /// <summary>
        /// Sets a query string parameter for the service operation.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="type">The parameter type.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetQueryParameter(string name, Type type);

        /// <summary>
        /// Sets a query string parameter for the service operation.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="type">The parameter type.</param>
        /// <param name="exampleValue">An example value.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetQueryParameter(string name, Type type, object exampleValue);

        /// <summary>
        /// Sets a query string parameter for the service operation.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="type">The parameter type.</param>
        /// <param name="exampleValue">An example value.</param>
        /// <param name="allowedValues">An optional list of allowed values.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetQueryParameter(string name, Type type, object exampleValue, IList<string> allowedValues);

        /// <summary>
        /// Sets a query string parameter for the service operation.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="type">The parameter type.</param>
        /// <param name="exampleValue">An example value.</param>
        /// <param name="regexConstraint">An optional regular expression constraint.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetQueryParameter(string name, Type type, object exampleValue, string regexConstraint);

        /// <summary>
        /// Sets a query string parameter for the service operation.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="type">The parameter type.</param>
        /// <param name="exampleValue">An example value.</param>
        /// <param name="allowedValues">An optional list of allowed values.</param>
        /// <param name="regexConstraint">An optional regular expression constraint.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetQueryParameter(string name, Type type, object exampleValue, IList<string> allowedValues, string regexConstraint);

        /// <summary>
        /// Sets a route parameter, that was not inferred by the lambda expression or has additional information such as list of allowed values,
        /// for the service operation.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetRouteParameter(string name);

        /// <summary>
        /// Sets a route parameter, that was not inferred by the lambda expression or has additional information such as list of allowed values,
        /// for the service operation.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="exampleValue">An example value.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetRouteParameter(string name, object exampleValue);

        /// <summary>
        /// Sets a route parameter, that was not inferred by the lambda expression or has additional information such as list of allowed values,
        /// for the service operation.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="exampleValue">An example value.</param>
        /// <param name="allowedValues">An optional list of allowed values.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetRouteParameter(string name, object exampleValue, IList<string> allowedValues);

        /// <summary>
        /// Sets a resource instance used as an example for the service operation request data.
        /// </summary>
        /// <param name="instance">The request resource instance.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetRequestResourceExample(object instance);

        /// <summary>
        /// Sets a resource instance used as an example for the service operation request data.
        /// </summary>
        /// <param name="instance">The request resource instance.</param>
        /// <param name="xmlSchemas">Custom XML schemas for the resource.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetRequestResourceExample(object instance, XmlSchemas xmlSchemas);

        /// <summary>
        /// Sets a resource instance used as an example for the service operation response data.
        /// </summary>
        /// <param name="instance">The response resource instance.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetResponseResourceExample(object instance);

        /// <summary>
        /// Sets a resource instance used as an example for the service operation response data.
        /// </summary>
        /// <param name="instance">The response resource instance.</param>
        /// <param name="xmlSchemas">Custom XML schemas for the resource.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetResponseResourceExample(object instance, XmlSchemas xmlSchemas);
        
        /// <summary>
        /// Sets a custom HTTP response status for the service operation.
        /// </summary>
        /// <param name="status">The HTTP status.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetResponseStatus(ProxyStatus status);

        /// <summary>
        /// Sets a custom HTTP response status for the service operation.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetResponseStatus(HttpStatusCode statusCode);

        /// <summary>
        /// Sets a custom HTTP response status for the service operation.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="statusDescription">The HTTP status description.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetResponseStatus(HttpStatusCode statusCode, string statusDescription);

        /// <summary>
        /// Sets custom HTTP response statuses for the service operation.
        /// </summary>
        /// <param name="statuses">A list of HTTP statuses.</param>
        /// <returns>The service method metadata.</returns>
        IMethodMetadata SetResponseStatuses(IList<ProxyStatus> statuses);
    }
}
