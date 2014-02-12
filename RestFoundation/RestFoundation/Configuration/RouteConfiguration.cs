// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using RestFoundation.Behaviors;
using RestFoundation.Formatters;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.Configuration
{
    /// <summary>
    /// Represents a route configuration.
    /// </summary>
    public sealed class RouteConfiguration
    {
        private readonly IEnumerable<IRestServiceHandler> m_routeHandlers;

        internal RouteConfiguration(IEnumerable<IRestServiceHandler> routeHandlers)
        {
            if (routeHandlers == null)
            {
                throw new ArgumentNullException("routeHandlers");
            }

            m_routeHandlers = routeHandlers;
        }

        /// <summary>
        /// Prevents media types supported by the provided formatter from being supported by the
        /// service through the current route.
        /// </summary>
        /// <typeparam name="TFormatter">The type of the media type formatter.</typeparam>
        /// <returns>The route configuration.</returns>
        public RouteConfiguration BlockMediaTypesForFormatter<TFormatter>()
            where TFormatter : class, IMediaTypeFormatter
        {
            Type formatterType = typeof(TFormatter);
            var supportedMediaTypes = formatterType.GetCustomAttributes<SupportedMediaTypeAttribute>(false).ToList();

            if (supportedMediaTypes.Count == 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, Resources.Global.MissingSupportedMediaTypeForFormatter, formatterType.Name));
            }

            foreach (SupportedMediaTypeAttribute supportedMediaType in supportedMediaTypes)
            {
                BlockMediaType(supportedMediaType.MediaType);
            }

            return this;
        }

        /// <summary>
        /// Prevents a provided media type from being supported by the service through the current route.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <returns>The route configuration.</returns>
        public RouteConfiguration BlockMediaType(string mediaType)
        {
            if (String.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentNullException("mediaType");
            }

            var blockFormatter = new BlockFormatter();

            foreach (IRestServiceHandler routeHandler in m_routeHandlers)
            {
                MediaTypeFormatterRegistry.AddHandlerFormatter(routeHandler, mediaType.Trim(), blockFormatter);
            }

            return this;
        }

        /// <summary>
        /// Sets a route specific formatter for its supported media types for the current route.
        /// </summary>
        /// <param name="formatter">The media formatter.</param>
        /// <returns>The route configuration.</returns>
        public RouteConfiguration SetMediaTypeFormatter(IMediaTypeFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }

            Type formatterType = formatter.GetType();
            var supportedMediaTypes = formatterType.GetCustomAttributes<SupportedMediaTypeAttribute>(false).ToList();

            if (supportedMediaTypes.Count == 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, Resources.Global.MissingSupportedMediaTypeForFormatter, formatterType.Name), "formatter");
            }

            foreach (SupportedMediaTypeAttribute supportedMediaType in supportedMediaTypes)
            {
                SetMediaTypeFormatter(supportedMediaType.MediaType, formatter);
            }

            return this;
        }

        /// <summary>
        /// Sets a route specific formatter for the provided media type for the current route.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <param name="formatter">The media formatter.</param>
        /// <returns>The route configuration.</returns>
        public RouteConfiguration SetMediaTypeFormatter(string mediaType, IMediaTypeFormatter formatter)
        {
            if (String.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentNullException("mediaType");
            }

            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }

            foreach (IRestServiceHandler routeHandler in m_routeHandlers)
            {
                MediaTypeFormatterRegistry.AddHandlerFormatter(routeHandler, mediaType.Trim(), formatter);
            }

            return this;
        }

        /// <summary>
        /// Adds behaviors to the current route.
        /// </summary>
        /// <param name="behaviors">An array of behavior instances.</param>
        /// <returns>The route configuration.</returns>
        public RouteConfiguration WithBehaviors(params IServiceBehavior[] behaviors)
        {
            if (behaviors == null)
            {
                throw new ArgumentNullException("behaviors");
            }

            if (behaviors.Length > 1 && behaviors.GroupBy(s => s.GetType()).Max(g => g.Count()) > 1)
            {
                throw new InvalidOperationException(Resources.Global.DuplicateRouteBehaviors);
            }

            foreach (IRestServiceHandler routeHandler in m_routeHandlers)
            {
                for (int i = 0; i < behaviors.Length; i++)
                {
                    IServiceBehavior behavior = behaviors[i];

                    if (behavior != null)
                    {
                        ServiceBehaviorRegistry.AddBehavior(routeHandler, behaviors[i]);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Sets an exception handler for the current route.
        /// </summary>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns>The route configuration.</returns>
        public RouteConfiguration WithExceptionHandler(IServiceExceptionHandler exceptionHandler)
        {
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException("exceptionHandler");
            }

            foreach (IRestServiceHandler routeHandler in m_routeHandlers)
            {
                ServiceExceptionHandlerRegistry.SetExceptionHandler(routeHandler, exceptionHandler);
            }

            return this;
        }

        /// <summary>
        /// Specifies a Web.config name-value section that contains an ACL list of allowed IPs.
        /// All other IPs will be blocked for the current route.
        /// </summary>
        /// <param name="nameValueSectionName">The section name.</param>
        /// <returns>The route configuration.</returns>
        public RouteConfiguration WithIPsRestrictedBySection(string nameValueSectionName)
        {
            if (String.IsNullOrEmpty(nameValueSectionName))
            {
                throw new ArgumentNullException("nameValueSectionName");
            }

            foreach (IRestServiceHandler routeHandler in m_routeHandlers)
            {
                ServiceBehaviorRegistry.AddBehavior(routeHandler, new AclBehavior(nameValueSectionName));
            }

            return this;
        }

        /// <summary>
        /// Skip HTTP request validation that checks query string, form data and cookies for dangerous characters for the current route.
        /// </summary>
        /// <returns>The route configuration.</returns>
        public RouteConfiguration DoNotValidateRequests()
        {           
            foreach (IRestServiceHandler routeHandler in m_routeHandlers)
            {
                UnvalidatedHandlerRegistry.Add(routeHandler);
            }

            return this;
        }
    }
}
