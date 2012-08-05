// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System.IO;

namespace RestFoundation
{
    /// <summary>
    /// Represents an uploaded file for multi-part content requests.
    /// </summary>
    public interface IUploadedFile
    {
        /// <summary>
        /// Gets the file content type.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets the file content length.
        /// </summary>
        int ContentLength { get; }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the stream containing the file data.
        /// </summary>
        Stream Data { get; }

        /// <summary>
        /// Saves the file on disk under the provided file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        void SaveAs(string fileName);

        /// <summary>
        /// Returns the file content data as a byte array.
        /// </summary>
        /// <returns>A byte array containing the file content data.</returns>
        byte[] ReadAsByteArray();

        /// <summary>
        /// Returns the file content data as a <see cref="System.String"/>.
        /// </summary>
        /// <returns>A string containing the file content data.</returns>
        string ReadAsString();
    }
}
