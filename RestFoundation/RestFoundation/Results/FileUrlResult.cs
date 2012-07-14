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
        /// Gets or sets the file URL.
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// Gets the <see cref="FileInfo"/> instance using the service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <returns>The file info instance.</returns>
        protected override FileInfo GetFile(IServiceContext context)
        {
            if (String.IsNullOrWhiteSpace(FileUrl))
            {
                return null;
            }

            var file = new FileInfo(context.MapPath(FileUrl));
            return file.Exists ? file : null;
        }
    }
}
