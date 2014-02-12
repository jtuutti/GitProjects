// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;
using RestFoundation.Security;
using RestFoundation.ServiceProxy;

namespace RestFoundation.Configuration
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
                throw new InvalidOperationException(Resources.Global.ProxyAlreadyInitialized);
            }

            if (!Regex.IsMatch(relativeUrl, "^[0-9a-zA-Z]+([0-9a-zA-Z-]*[0-9a-zA-Z]+)?$"))
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, Resources.Global.InvalidServiceProxyRelativeUrl, relativeUrl), "relativeUrl");
            }

            options.IsServiceProxyInitialized = true;
            options.ServiceProxyRelativeUrl = relativeUrl.ToLowerInvariant();

            ProxyPathProvider.AppInitialize();
            SetAllowUnsafeHeaderParsing();

            RouteTable.Routes.Add("ProxyCss", new Route(relativeUrl + "/help.css", new CssRouteHandler("help.css")));
            RouteTable.Routes.Add("ProxyIndexOp", new Route(relativeUrl + "/index.op.js", new JavaScriptRouteHandler("index.op.min.js")));
            RouteTable.Routes.Add("ProxyJQuery", new Route(relativeUrl + "/jquery.js", new JavaScriptRouteHandler("jquery-1.10.1.min.js")));
            RouteTable.Routes.Add("ProxyMetadataOp", new Route(relativeUrl + "/metadata.op.js", new JavaScriptRouteHandler("metadata.op.min.js")));
            RouteTable.Routes.Add("ProxyProxyOp", new Route(relativeUrl + "/proxy.op.js", new JavaScriptRouteHandler("proxy.op.min.js")));
            RouteTable.Routes.Add("ProxySubmit", new Route(relativeUrl + "/submit.js", new JavaScriptRouteHandler("submit.min.js")));
            RouteTable.Routes.MapPageRoute("ProxyIndex", relativeUrl + "/index", "~/index.aspx", false);
            RouteTable.Routes.MapPageRoute(String.Empty, relativeUrl + "/metadata", "~/metadata.aspx", false);
            RouteTable.Routes.MapPageRoute(String.Empty, relativeUrl + "/proxy", "~/proxy.aspx", false);
            RouteTable.Routes.Add(new Route(relativeUrl + "/export", new ProxyExportHandler()));
            RouteTable.Routes.Add(new Route(relativeUrl + "/output", new ProxyOutputHandler()));
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

        /// <summary>
        /// Specifies an XML documentation file generated by the .NET compiler for the service contract interfaces/classes.
        /// Service methods must be documented.
        /// </summary>
        /// <param name="filePath">The absolute XMLDOC file path.</param>
        /// <returns>The configuration object.</returns>
        /// <exception cref="FileNotFoundException">If the documentation file was not found.</exception>
        public ProxyConfiguration UseXmlDocFile(string filePath)
        {
            return UseXmlDocFile(filePath, XmlDocPathType.Absolute);
        }

        /// <summary>
        /// Specifies an XML documentation file generated by the .NET compiler for the service contract interfaces/classes.
        /// Service methods must be documented.
        /// </summary>
        /// <param name="filePath">The XMLDOC file path.</param>
        /// <param name="filePathType">The file path type.</param>
        /// <returns>The configuration object.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <paramref name="filePathType"/> contains a virtual path of type <see cref="XmlDocPathType.Virtual"/>
        /// and is not rooted.
        /// </exception>
        /// <exception cref="FileNotFoundException">If the documentation file was not found.</exception>
        public ProxyConfiguration UseXmlDocFile(string filePath, XmlDocPathType filePathType)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            ICollection<Type> contractTypes = ServiceContractTypeRegistry.GetContractTypes();
            var documentation = new Dictionary<Type, IReadOnlyList<XmlDocMetadata>>();

            if (filePathType == XmlDocPathType.AppDomain)
            {
                filePath = Path.Combine(HttpRuntime.AppDomainAppPath, filePath);
            }

            foreach (Type contractType in contractTypes)
            {
                var parser = new XmlDocParser(contractType, filePath);
                documentation[contractType] = parser.GetMetadata();
            }

            Rest.Configuration.Options.XmlDocumentation = documentation;
            return this;
        }

        private static void SetAllowUnsafeHeaderParsing()
        {
            const string FieldName = "useUnsafeHeaderParsing";
            const string SectionMemberName = "Section";
            const string SectionTypeName = "System.Net.Configuration.SettingsSectionInternal";

            const BindingFlags SectionBindingFlags = BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic;

            TypeInfo sectionType = typeof(SettingsSection).Assembly
                                                          .DefinedTypes
                                                          .FirstOrDefault(x => x.FullName == SectionTypeName);

            if (sectionType == null)
            {
                return;
            }

            object section = sectionType.InvokeMember(SectionMemberName,
                                                      SectionBindingFlags,
                                                      null,
                                                      null,
                                                      new object[0],
                                                      CultureInfo.InvariantCulture);

            if (section == null)
            {
                return;
            }

            FieldInfo unsafeHeaderField = sectionType.DeclaredFields.FirstOrDefault(x => x.Name == FieldName);

            if (unsafeHeaderField != null)
            {
                unsafeHeaderField.SetValue(section, true);
            }
        }
    }
}
