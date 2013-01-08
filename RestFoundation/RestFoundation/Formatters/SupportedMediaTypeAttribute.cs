// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation.Formatters
{
    /// <summary>
    /// Represents a supported media type for a media type formatter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class SupportedMediaTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedMediaTypeAttribute"/> class.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        public SupportedMediaTypeAttribute(string mediaType)
        {
            if (String.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentNullException("mediaType");
            }

            MediaType = mediaType;
        }

        /// <summary>
        /// Gets the media type.
        /// </summary>
        public string MediaType { get; private set; }
    }
}
