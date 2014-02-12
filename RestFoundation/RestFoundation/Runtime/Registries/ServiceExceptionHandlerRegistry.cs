// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System.Collections.Concurrent;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.Runtime
{
    internal static class ServiceExceptionHandlerRegistry
    {
        private static readonly ConcurrentDictionary<IRestServiceHandler, IServiceExceptionHandler> handlerExceptionHandlers = new ConcurrentDictionary<IRestServiceHandler, IServiceExceptionHandler>();
        private static IServiceExceptionHandler globalExceptionHandler;

        public static IServiceExceptionHandler GetExceptionHandler(IRestServiceHandler handler)
        {
            IServiceExceptionHandler exceptionHandler;

            return handlerExceptionHandlers.TryGetValue(handler, out exceptionHandler) ? exceptionHandler : globalExceptionHandler;
        }

        public static void SetGlobalExceptionHandler(IServiceExceptionHandler exceptionHandler)
        {
            globalExceptionHandler = exceptionHandler;
        }

        public static void SetExceptionHandler(IRestServiceHandler handler, IServiceExceptionHandler exceptionHandler)
        {
            handlerExceptionHandlers.AddOrUpdate(handler, handlerToAdd => exceptionHandler, (handlerToUpdate, exceptionHandlerToUpdate) => exceptionHandler);
        }
    }
}
