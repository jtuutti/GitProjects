using System;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace RestFoundation.ServiceProxy
{
    public sealed class ProxyPathProvider : VirtualPathProvider
    {
        public static void AppInitialize()
        {
            var provider = new ProxyPathProvider();           
            var environment = (HostingEnvironment) typeof(HostingEnvironment).InvokeMember("_theHostingEnvironment",
                                                                                           BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField,
                                                                                           null,
                                                                                           null,
                                                                                           null);

            if (environment == null)
            {
                return;
            }

            MethodInfo mi = typeof(HostingEnvironment).GetMethod("RegisterVirtualPathProviderInternal", BindingFlags.NonPublic | BindingFlags.Static);

            if (mi == null)
            {
                return;
            }

            mi.Invoke(environment, new object[] { provider });
        }

        public override bool FileExists(string virtualPath)
        {
            string fileName = GetFileName(virtualPath);

            if (ProxyFile.ResourceMap.ContainsKey(fileName))
            {
                return true;
            }

            return Previous.FileExists(virtualPath);
        }

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
