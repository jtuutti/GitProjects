using System;
using RestFoundation.Runtime;

namespace RestFoundation
{
    /// <summary>
    /// Represents a content formatter builder.
    /// </summary>
    public sealed class ContentFormatterBuilder
    {
        internal ContentFormatterBuilder()
        {
        }

        /// <summary>
        /// Gets a content formatter by the content type.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <returns>The associated content formatter or null.</returns>
        public IContentFormatter Get(string contentType)
        {
            if (String.IsNullOrEmpty(contentType)) throw new ArgumentNullException("contentType");

            return ContentFormatterRegistry.GetFormatter(contentType);
        }

        /// <summary>
        /// Sets a content formatter for the provided content type.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <param name="formatter">The content formatter.</param>
        public void Set(string contentType, IContentFormatter formatter)
        {
            if (formatter == null) throw new ArgumentNullException("formatter");
            if (String.IsNullOrEmpty(contentType)) throw new ArgumentNullException("contentType");

            ContentFormatterRegistry.SetFormatter(contentType, formatter);
        }

        /// <summary>
        /// Removes an associated content formatter for the provided content type.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <returns>
        /// true if a content formatter was removed; false if no content formatter had been associated
        /// for the content type.
        /// </returns>
        public bool Remove(string contentType)
        {
            if (String.IsNullOrEmpty(contentType)) throw new ArgumentNullException("contentType");

            return ContentFormatterRegistry.RemoveFormatter(contentType);
        }

        /// <summary>
        /// Clears all associated content formatters.
        /// </summary>
        public void Clear()
        {
            ContentFormatterRegistry.Clear();
        }
    }
}
