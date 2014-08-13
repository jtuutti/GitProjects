using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SimpleViewEngine.Properties;

namespace SimpleViewEngine.Routing
{
    /// <summary>
    /// Configures a collection of client framework defined action methods
    /// that redirect to the default MVC action.
    /// </summary>
    public static class ClientRouteConfiguration
    {
        private static readonly ConcurrentDictionary<RouteInfo, string> routeRedirects = new ConcurrentDictionary<RouteInfo, string>();

        internal static bool Any
        {
            get
            {
                return routeRedirects.Count > 0;
            }
        }

        /// <summary>
        /// Adds the specified target action redirect.
        /// </summary>
        /// <param name="controller">A controller name.</param>
        /// <param name="targetAction">A target action name.</param>
        /// <param name="clientActions">
        /// A list of client action name to redirect to the target action.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If any of the method arguments is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If no client action names are provided.
        /// </exception>
        public static void AddRedirect(string controller, string targetAction, IReadOnlyList<string> clientActions)
        {
            if (String.IsNullOrEmpty(controller))
            {
                throw new ArgumentNullException("controller");
            }

            if (String.IsNullOrEmpty(targetAction))
            {
                throw new ArgumentNullException("targetAction");
            }

            if (clientActions == null)
            {
                throw new ArgumentNullException("clientActions");
            }

            if (clientActions.Count == 0)
            {
                throw new ArgumentOutOfRangeException("clientActions", Resources.NoClientActions);
            }

            foreach (string clientAction in clientActions)
            {
                routeRedirects.AddOrUpdate(new RouteInfo(controller, clientAction), targetAction, (key, value) => value);
            }
        }

        internal static string GetTargetAction(string controller, string action)
        {
            string targetAction;

            if (!routeRedirects.TryGetValue(new RouteInfo(controller, action), out targetAction) || String.IsNullOrEmpty(targetAction))
            {
                return null;
            }

            return targetAction;
        }
    }
}
