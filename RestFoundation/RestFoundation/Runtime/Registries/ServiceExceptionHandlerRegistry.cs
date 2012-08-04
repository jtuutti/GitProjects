using System.Collections.Concurrent;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.Runtime
{
    internal static class ServiceExceptionHandlerRegistry
    {
        private static readonly ConcurrentDictionary<IRestHandler, IServiceExceptionHandler> handlerExceptionHandlers = new ConcurrentDictionary<IRestHandler, IServiceExceptionHandler>();
        private static IServiceExceptionHandler globalExceptionHandler;

        public static IServiceExceptionHandler GetExceptionHandler(IRestHandler handler)
        {
            IServiceExceptionHandler exceptionHandler;

            return handlerExceptionHandlers.TryGetValue(handler, out exceptionHandler) ? exceptionHandler : globalExceptionHandler;
        }

        public static void SetGlobalExceptionHandler(IServiceExceptionHandler exceptionHandler)
        {
            globalExceptionHandler = exceptionHandler;
        }

        public static void SetExceptionHandler(IRestHandler handler, IServiceExceptionHandler exceptionHandler)
        {
            handlerExceptionHandlers.AddOrUpdate(handler, handlerToAdd => exceptionHandler, (handlerToUpdate, exceptionHandlerToUpdate) => exceptionHandler);
        }
    }
}
