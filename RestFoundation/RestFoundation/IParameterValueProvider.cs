﻿// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System.Reflection;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation
{
    /// <summary>
    /// Defines a service method parameter value provider that orchestrates media type formatters,
    /// object binders, route and HTTP request data to populate service method parameter values.
    /// </summary>
    public interface IParameterValueProvider
    {
        /// <summary>
        /// Creates a parameter value based on the routing and HTTP data.
        /// </summary>
        /// <param name="handler">The REST handler associated with the HTTP request.</param>
        /// <param name="parameter">The service method parameters.</param>
        /// <param name="isResource">
        /// true if the parameter represents a REST resource; otherwise, false. Only 1 resource per
        /// service method is allowed.
        /// </param>
        /// <returns>The created parameter value.</returns>
        object CreateValue(IRestServiceHandler handler, ParameterInfo parameter, out bool isResource);
    }
}
