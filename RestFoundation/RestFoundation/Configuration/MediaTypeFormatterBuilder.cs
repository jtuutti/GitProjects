// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using RestFoundation.Formatters;
using RestFoundation.Runtime;

namespace RestFoundation.Configuration
{
    /// <summary>
    /// Represents a global media type formatter builder.
    /// </summary>
    public sealed class MediaTypeFormatterBuilder
    {
        internal MediaTypeFormatterBuilder()
        {
        }

        /// <summary>
        /// Gets a global formatter by the media type.
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
                throw new ArgumentException(Resources.Global.DisallowedMediaTypeParameters, "mediaType");
            }

            return MediaTypeFormatterRegistry.GetFormatter(mediaType.Trim());
        }

        /// <summary>
        /// Gets a sequence of all global media formatters.
        /// </summary>
        /// <returns>The media formatters.</returns>
        public IEnumerable<IMediaTypeFormatter> GetAll()
        {
            return MediaTypeFormatterRegistry.GetMediaFormatters().Distinct();
        }

        /// <summary>
        /// Sets a global formatter for its supported types.
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
            var supportedMediaTypes = formatterType.GetCustomAttributes<SupportedMediaTypeAttribute>(false).ToList();

            if (supportedMediaTypes.Count == 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture,
                                                          Resources.Global.MissingSupportedMediaTypeForFormatter,
                                                          formatterType.Name), "formatter");
            }

            foreach (SupportedMediaTypeAttribute supportedMediaType in supportedMediaTypes)
            {
                Set(supportedMediaType.MediaType, formatter);
            }
        }

        /// <summary>
        /// Sets a global formatter for the provided media type.
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
                throw new ArgumentException(Resources.Global.DisallowedMediaTypeParameters, "mediaType");
            }

            MediaTypeFormatterRegistry.SetFormatter(mediaType.Trim(), formatter);
        }

        /// <summary>
        /// Removes a global formatter for the provided media type.
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
                throw new ArgumentException(Resources.Global.DisallowedMediaTypeParameters, "mediaType");
            }

            return MediaTypeFormatterRegistry.RemoveFormatter(mediaType.Trim());
        }

        /// <summary>
        /// Removes the provided media type formatter.
        /// </summary>
        /// <param name="formatter">The media formatter.</param>
        /// <returns>true if the media type formatter was removed; false otherwise.</returns>
        public bool Remove(IMediaTypeFormatter formatter)
        {
            return MediaTypeFormatterRegistry.RemoveFormatter(formatter);
        }

        /// <summary>
        /// Clears all global media type formatters.
        /// </summary>
        public void Clear()
        {
            MediaTypeFormatterRegistry.Clear();
        }
    }
}
