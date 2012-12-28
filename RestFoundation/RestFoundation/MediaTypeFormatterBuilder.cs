// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Globalization;
using System.Linq;
using RestFoundation.Formatters;
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
        /// <exception cref="ArgumentException">If media type parameters are provided.</exception>
        public IMediaTypeFormatter Get(string mediaType)
        {
            if (String.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentNullException("mediaType");
            }

            if (mediaType.IndexOf(';') >= 0 || mediaType.IndexOf(',') >= 0)
            {
                throw new ArgumentException(RestResources.DisallowedMediaTypeParameters, "mediaType");
            }

            return MediaTypeFormatterRegistry.GetFormatter(mediaType.Trim());
        }

        /// <summary>
        /// Sets a formatter for its supported types.
        /// </summary>
        /// <param name="formatter">The media type formatter.</param>
        /// <exception cref="ArgumentException">If media type parameters are provided.</exception>
        public void Set(IMediaTypeFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }

            Type formatterType = formatter.GetType();
            var supportedMediaTypes = formatterType.GetCustomAttributes(typeof(SupportedMediaTypeAttribute), false).Cast<SupportedMediaTypeAttribute>().ToList();

            if (supportedMediaTypes.Count == 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, RestResources.MissingSupportedMediaTypeForFormatter, formatterType.Name), "formatter");
            }

            foreach (SupportedMediaTypeAttribute supportedMediaType in supportedMediaTypes)
            {
                Set(supportedMediaType.MediaType, formatter);
            }
        }

        /// <summary>
        /// Sets a formatter for the provided media type.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <param name="formatter">The media type formatter.</param>
        /// <exception cref="ArgumentException">If media type parameters are provided.</exception>
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

            if (mediaType.IndexOf(';') >= 0 || mediaType.IndexOf(',') >= 0)
            {
                throw new ArgumentException(RestResources.DisallowedMediaTypeParameters, "mediaType");
            }

            MediaTypeFormatterRegistry.SetFormatter(mediaType.Trim(), formatter);
        }

        /// <summary>
        /// Removes an associated formatter for the provided media type.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <returns>
        /// true if a media type formatter was removed; false if no formatter had been associated
        /// for the media type.
        /// </returns>
        /// <exception cref="ArgumentException">If media type parameters are provided.</exception>
        public bool Remove(string mediaType)
        {
            if (String.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentNullException("mediaType");
            }

            if (mediaType.IndexOf(';') >= 0 || mediaType.IndexOf(',') >= 0)
            {
                throw new ArgumentException(RestResources.DisallowedMediaTypeParameters, "mediaType");
            }

            return MediaTypeFormatterRegistry.RemoveFormatter(mediaType.Trim());
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
