using System;
using RestFoundation.Runtime;

namespace RestFoundation
{
    /// <summary>
    /// Represents a media type formatter builder.
    /// </summary>
    public sealed class MediaTypeFormatterBuilder
    {
        internal MediaTypeFormatterBuilder()
        {
        }

        /// <summary>
        /// Gets a formatter by the media type.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <returns>The associated media type formatter or null.</returns>
        public IMediaTypeFormatter Get(string mediaType)
        {
            if (String.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentNullException("mediaType");
            }

            return MediaTypeFormatterRegistry.GetFormatter(mediaType);
        }

        /// <summary>
        /// Sets a formatter for the provided media type.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <param name="formatter">The media type formatter.</param>
        public void Set(string mediaType, IMediaTypeFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }

            if (String.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentNullException("mediaType");
            }

            MediaTypeFormatterRegistry.SetFormatter(mediaType, formatter);
        }

        /// <summary>
        /// Removes an associated formatter for the provided media type.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <returns>
        /// true if a media type formatter was removed; false if no formatter had been associated
        /// for the media type.
        /// </returns>
        public bool Remove(string mediaType)
        {
            if (String.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentNullException("mediaType");
            }

            return MediaTypeFormatterRegistry.RemoveFormatter(mediaType);
        }

        /// <summary>
        /// Clears all associated media type formatters.
        /// </summary>
        public void Clear()
        {
            MediaTypeFormatterRegistry.Clear();
        }
    }
}
