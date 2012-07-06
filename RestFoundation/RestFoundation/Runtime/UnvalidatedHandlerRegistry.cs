using System.Collections.Generic;

namespace RestFoundation.Runtime
{
    internal static class UnvalidatedHandlerRegistry
    {
        private static readonly HashSet<IRestHandler> unvalidatedHandlers = new HashSet<IRestHandler>();

        public static bool IsUnvalidated(IRestHandler handler)
        {
            return unvalidatedHandlers.Contains(handler);
        }

        public static void Add(IRestHandler handler)
        {
            unvalidatedHandlers.Add(handler);
        }
    }
}
