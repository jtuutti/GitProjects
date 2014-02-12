// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Globalization;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a virtual service proxy path provider.
    /// </summary>
    public sealed class ProxyPathProvider : VirtualPathProvider
    {
        /// <summary>
        /// Initializes the virtual path provider.
        /// </summary>
        /// <remarks>
        /// For some security reason .NET does not allow registering virtual path providers
        /// for pre-compiled web application. REST Foundation requires full trust and has
        /// no issues with the virtual provider in a pre-compiled web application.
        /// The code in this method is a work around that restriction.
        /// </remarks>
        public static void AppInitialize()
        {
            var provider = new ProxyPathProvider();           
            var environment = (HostingEnvironment) typeof(HostingEnvironment).InvokeMember("_theHostingEnvironment",
                                                                                           BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField,
                                                                                           null,
                                                                                           null,
                                                                                           null,
                                                                                           CultureInfo.InvariantCulture);

            if (environment == null)
            {
                return;
            }

            MethodInfo method = typeof(HostingEnvironment).GetMethod("RegisterVirtualPathProviderInternal", BindingFlags.NonPublic | BindingFlags.Static);

            if (method == null)
            {
                return;
            }

            method.Invoke(environment, new object[] { provider });
        }

        /// <summary>
        /// Gets a value that indicates whether a file exists in the virtual file system.
        /// </summary>
        /// <returns>
        /// true if the file exists in the virtual file system; otherwise, false.
        /// </returns>
        /// <param name="virtualPath">The path to the virtual file.</param>
        public override bool FileExists(string virtualPath)
        {
            string fileName = GetFileName(virtualPath);

            if (ProxyFile.ResourceMap.ContainsKey(fileName))
            {
                return true;
            }

            return Previous.FileExists(virtualPath);
        }

        /// <summary>
        /// Gets a virtual file from the virtual file system.
        /// </summary>
        /// <returns>
        /// A descendent of the <see cref="T:System.Web.Hosting.VirtualFile"/> class that represents a file in the virtual file system.
        /// </returns>
        /// <param name="virtualPath">The path to the virtual file.</param>
        public override VirtualFile GetFile(string virtualPath)
        {
            string fileName = GetFileName(virtualPath);

            if (ProxyFile.ResourceMap.ContainsKey(fileName))
            {
                return new ProxyFile(virtualPath, fileName);
            }

            return Previous.GetFile(virtualPath);
        }

        private static string GetFileName(string virtualPath)
        {
            string simpleVirtualPath = virtualPath.Trim();
            string applicationPath;

            try
            {
                applicationPath = HttpContext.Current.Request.ApplicationPath;
            }
            catch (Exception)
            {
                applicationPath = null;
            }

            if (!String.IsNullOrEmpty(applicationPath) && simpleVirtualPath.StartsWith(applicationPath, StringComparison.OrdinalIgnoreCase))
            {
                simpleVirtualPath = simpleVirtualPath.Substring(applicationPath.Length, simpleVirtualPath.Length - applicationPath.Length);
            }

            return simpleVirtualPath.TrimStart('~', '/');
        }
    }
}
