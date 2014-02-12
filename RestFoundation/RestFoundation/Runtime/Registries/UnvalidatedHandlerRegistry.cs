// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System.Collections.Generic;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.Runtime
{
    internal static class UnvalidatedHandlerRegistry
    {
        private static readonly HashSet<IRestServiceHandler> unvalidatedHandlers = new HashSet<IRestServiceHandler>();

        public static bool IsUnvalidated(IRestServiceHandler handler)
        {
            return unvalidatedHandlers.Contains(handler);
        }

        public static void Add(IRestServiceHandler handler)
        {
            unvalidatedHandlers.Add(handler);
        }
    }
}
