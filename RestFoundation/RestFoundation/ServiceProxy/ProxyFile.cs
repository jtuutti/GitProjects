// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web.Hosting;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a virtual service proxy file.
    /// </summary>
    public sealed class ProxyFile : VirtualFile
    {
        private static readonly Dictionary<string, string> resourceMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "help.master", "RestFoundation.ServiceProxy.Resources.help.master" },
            { "index.aspx", "RestFoundation.ServiceProxy.Resources.index.aspx" },
            { "metadata.aspx", "RestFoundation.ServiceProxy.Resources.metadata.aspx" },
            { "output.aspx", "RestFoundation.ServiceProxy.Resources.output.aspx" },
            { "proxy.aspx", "RestFoundation.ServiceProxy.Resources.proxy.aspx" }
        };

        private readonly string m_fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyFile"/> class.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="fileName">The file name.</param>
        public ProxyFile(string virtualPath, string fileName) : base(virtualPath)
        {
            if (virtualPath == null)
            {
                throw new ArgumentNullException("virtualPath");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (!resourceMap.ContainsKey(fileName))
            {
                throw new ArgumentOutOfRangeException("fileName");
            }

            m_fileName = fileName;
        }

        internal static IDictionary<string, string> ResourceMap
        {
            get
            {
                return resourceMap;
            }
        }

        /// <summary>
        /// When overridden in a derived class, returns a read-only stream to the virtual resource.
        /// </summary>
        /// <returns>
        /// A read-only stream to the virtual file.
        /// </returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
                         Justification = "The stream is returned to the outer scope and cannot be closed in this method")]
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
