// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using RestFoundation.Results;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation
{
    /// <summary>
    /// Defines a result wrapper.
    /// </summary>
    public interface IResultWrapper
    {
        /// <summary>
        /// Wraps a POCO object returned by the service method with an <see cref="IResult"/>.
        /// </summary>
        /// <param name="handler">The service context handler.</param>
        /// <param name="returnedObj">The returned object.</param>
        /// <param name="methodReturnType">The method return type.</param>
        /// <returns>The wrapper result.</returns>
        IResult Wrap(IServiceContextHandler handler, object returnedObj, Type methodReturnType);
    }
}
