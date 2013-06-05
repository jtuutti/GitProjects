// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.IO;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a file URL result.
    /// </summary>
    public class FileUrlResult : FileResultBase
    {
        /// <summary>
        /// Gets or sets the file URL located in the same application as the service.
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// Gets the <see cref="FileInfo"/> instance using the service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <returns>The file info instance.</returns>
        protected override FileInfo GetFile(IServiceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return !String.IsNullOrWhiteSpace(FileUrl) ? new FileInfo(context.MapPath(FileUrl)) : null;
        }
    }
}
