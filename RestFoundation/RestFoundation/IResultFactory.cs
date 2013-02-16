// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using RestFoundation.Results;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation
{
    /// <summary>
    /// Defines a result factory.
    /// </summary>
    public interface IResultFactory
    {
        /// <summary>
        /// Creates an <see cref="IResult"/> instance from a POCO object returned by the service method.
        /// </summary>
        /// <param name="returnedObj">The returned object.</param>
        /// <param name="methodReturnType">The method return type.</param>
        /// <param name="handler">The service context handler.</param>
        /// <returns>The created result instance.</returns>
        IResult Create(object returnedObj, Type methodReturnType, IServiceContextHandler handler);
    }
}
