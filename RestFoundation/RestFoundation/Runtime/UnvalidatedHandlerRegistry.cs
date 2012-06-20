using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace RestFoundation.Runtime
{
    internal static class UnvalidatedHandlerRegistry
    {
        private static readonly HashSet<IRouteHandler> unvalidatedHandlers = new HashSet<IRouteHandler>();

        public static bool IsUnvalidated(IRouteHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            return unvalidatedHandlers.Contains(handler);
        }

        public static void Add(IRouteHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            unvalidatedHandlers.Add(handler);
        }
    }
}
