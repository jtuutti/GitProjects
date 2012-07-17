using System;
using System.IO;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a local file path result.
    /// </summary>
    public class FilePathResult : FileResultBase
    {
        /// <summary>
        /// Gets or sets the local file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets the <see cref="FileInfo"/> instance using the service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <returns>The file info instance.</returns>
        protected override FileInfo GetFile(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (String.IsNullOrWhiteSpace(FilePath))
            {
                return null;
            }

            var file = new FileInfo(FilePath);
            return file.Exists ? file : null;
        }
    }
}
