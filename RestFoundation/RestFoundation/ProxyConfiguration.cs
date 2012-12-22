// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Routing;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Security;
using RestFoundation.ServiceProxy;

namespace RestFoundation
{
    /// <summary>
    /// Represents the service help and proxy interface configuration.
    /// </summary>
    public sealed class ProxyConfiguration
    {
        internal ProxyConfiguration()
        {
        }

        /// <summary>
        /// Enables HTML based interface for the service help pages and HTTP proxy under the relative URL "help".
        /// </summary>
        /// <returns>The configuration object.</returns>
        public ProxyConfiguration Enable()
        {
            return EnableWithRelativeUrl("help");
        }

        /// <summary>
        /// Enables HTML based interface for the service help pages and HTTP proxy.
        /// IMPORTANT: If you are setting the relative URL parameter to something other than "help",
        /// make sure to adjust the location/path setting in the Web.config accordingly to avoid
        /// ASP .NET validation errors.
        /// </summary>
        /// <param name="relativeUrl">The relative URL path for the service help and proxy.</param>
        /// <returns>The configuration object.</returns>
        /// <exception cref="ArgumentException">If the relative URL contains invalid characters.</exception>
        public ProxyConfiguration EnableWithRelativeUrl(string relativeUrl)
        {
            if (relativeUrl == null)
            {
                throw new ArgumentNullException("relativeUrl");
            }

            RestOptions options = Rest.Configuration.Options;

            if (options.IsServiceProxyInitialized)
            {
                throw new InvalidOperationException(RestResources.ProxyAlreadyInitialized);
            }

            if (!Regex.IsMatch(relativeUrl, "^[0-9a-zA-Z]+([0-9a-zA-Z-]*[0-9a-zA-Z]+)?$"))
            {
                string message = String.Format(CultureInfo.InvariantCulture,
                                               "{0}. Relative URL '{1}' does not meet those requirements.",
                                               "Service help/proxy relative URL can only contain letters and numbers with optional dashes in between",
                                               relativeUrl);

                throw new ArgumentException(message, "relativeUrl");
            }

            options.IsServiceProxyInitialized = true;
            options.ServiceProxyRelativeUrl = relativeUrl.ToLowerInvariant();

            ProxyPathProvider.AppInitialize();
            SetAllowUnsafeHeaderParsing();

            RouteTable.Routes.Add("ProxyCss", new Route(relativeUrl + "/help.css", new CssRouteHandler()));
            RouteTable.Routes.Add("ProxyJQuery", new Route(relativeUrl + "/jquery.js", new JQueryRouteHandler()));
            RouteTable.Routes.MapPageRoute("ProxyIndex", relativeUrl + "/index", "~/index.aspx");
            RouteTable.Routes.MapPageRoute(String.Empty, relativeUrl + "/metadata", "~/metadata.aspx");
            RouteTable.Routes.MapPageRoute(String.Empty, relativeUrl + "/output", "~/output.aspx");
            RouteTable.Routes.MapPageRoute(String.Empty, relativeUrl + "/proxy", "~/proxy.aspx");
            RouteTable.Routes.Add(new Route(relativeUrl, new ProxyRootHandler()));

            return this;
        }

        /// <summary>
        /// Sets the service description for the service help and proxy pages.
        /// </summary>
        /// <param name="description">The service description.</param>
        /// <returns>The configuration object.</returns>
        public ProxyConfiguration WithServiceDescription(string description)
        {
            Rest.Configuration.Options.ServiceDescription = description;
            return this;
        }

        /// <summary>
        /// Requires the service proxy to be accessed through HTTPS only.
        /// </summary>
        /// <returns>The configuration object.</returns>
        public ProxyConfiguration HttpsOnly()
        {
            Rest.Configuration.Options.ServiceProxyHttpsOnly = true;
            return this;
        }

        /// <summary>
        /// Sets basic authentication for the service proxy and authorizes users through the provided authorization manager
        /// instance.
        /// </summary>
        /// <param name="authorizationManager">The authorization manager.</param>
        /// <returns>The configuration object.</returns>
        public ProxyConfiguration RequireAuthorization(IAuthorizationManager authorizationManager)
        {
            if (authorizationManager == null)
            {
                throw new ArgumentNullException("authorizationManager");
            }

            Rest.Configuration.Options.ServiceProxyAuthorizationManager = authorizationManager;
            return this;
        }

        private static void SetAllowUnsafeHeaderParsing()
        {
            const string AssemblyName = "System.Net.Configuration.SettingsSectionInternal";
            const string SectionName = "Section";
            const string FieldName = "useUnsafeHeaderParsing";

            const BindingFlags SectionBindingFlags = BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic;
            const BindingFlags FieldBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            Assembly assembly = Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));

            if (assembly == null)
            {
                return;
            }

            Type assemblyType = assembly.GetType(AssemblyName);

            if (assemblyType == null)
            {
                return;
            }

            object section = assemblyType.InvokeMember(SectionName, SectionBindingFlags, null, null, new object[0], CultureInfo.InvariantCulture);

            if (section == null)
            {
                return;
            }

            FieldInfo unsafeHeaderField = assemblyType.GetField(FieldName, FieldBindingFlags);

            if (unsafeHeaderField != null)
            {
                unsafeHeaderField.SetValue(section, true);
            }
        }
    }
}
