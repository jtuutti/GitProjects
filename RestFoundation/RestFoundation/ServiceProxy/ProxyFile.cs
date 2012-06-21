using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;

namespace RestFoundation.ServiceProxy
{
    public sealed class ProxyFile : VirtualFile
    {
        private static readonly Dictionary<string, string> resourceMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "proxy.master", "RestFoundation.ServiceProxy.Resources.proxy.master" },
            { "proxy.aspx", "RestFoundation.ServiceProxy.Resources.proxy.aspx" },
        };

        private readonly string m_fileName;

        public ProxyFile(string virtualPath, string fileName) : base(virtualPath)
        {
            if (virtualPath == null) throw new ArgumentNullException("virtualPath");
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (!resourceMap.ContainsKey(fileName)) throw new ArgumentOutOfRangeException("fileName");

            m_fileName = fileName;
        }

        internal static IDictionary<string, string> ResourceMap
        {
            get
            {
                return resourceMap;
            }
        }

        public override Stream Open()
        {
            var fileStream = new MemoryStream();

            using (var resourceStream = GetType().Assembly.GetManifestResourceStream(resourceMap[m_fileName]))
            {
                if (resourceStream == null)
                {
                    return fileStream;
                }

                resourceStream.CopyTo(fileStream);
            }

            fileStream.Seek(0, SeekOrigin.Begin);

            return fileStream;
        }
    }
}
