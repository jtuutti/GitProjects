using System;
using System.Collections.Generic;
using System.Linq;
using RestFoundation.Behaviors;
using RestFoundation.Formatters;
using RestFoundation.Runtime;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation
{
    /// <summary>
    /// Represents a route configuration.
    /// </summary>
    public sealed class RouteConfiguration
    {
        private readonly IEnumerable<IRestHandler> m_routeHandlers;

        internal RouteConfiguration(IEnumerable<IRestHandler> routeHandlers)
        {
            if (routeHandlers == null)
            {
                throw new ArgumentNullException("routeHandlers");
            }

            m_routeHandlers = routeHandlers;
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

            foreach (IRestHandler routeHandler in m_routeHandlers)
            {
                MediaTypeFormatterRegistry.AddHandlerFormatter(routeHandler, mediaType, blockFormatter);
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

            foreach (IRestHandler routeHandler in m_routeHandlers)
            {
                MediaTypeFormatterRegistry.AddHandlerFormatter(routeHandler, mediaType, formatter);
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

            if (behaviors.GroupBy(s => s.GetType()).Max(g => g.Count()) > 1)
            {
                throw new InvalidOperationException("Multiple service behaviors of the same type are not allowed for the same route");
            }

            foreach (IRestHandler routeHandler in m_routeHandlers)
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

            foreach (IRestHandler routeHandler in m_routeHandlers)
            {
                ServiceExceptionHandlerRegistry.SetExceptionHandler(routeHandler, exceptionHandler);
            }

            return this;
        }

        /// <summary>
        /// Uses WCF-style data contract serializers to format and output the resources for the current route.
        /// </summary>
        /// <returns>The route configuration.</returns>
        public RouteConfiguration WithDataContractFormatters()
        {
            foreach (IRestHandler routeHandler in m_routeHandlers)
            {
                MediaTypeFormatterRegistry.AddHandlerFormatter(routeHandler, "application/json", new DataContractJsonFormatter());
                MediaTypeFormatterRegistry.AddHandlerFormatter(routeHandler, "application/xml", new DataContractXmlFormatter());
                MediaTypeFormatterRegistry.AddHandlerFormatter(routeHandler, "text/xml", new DataContractXmlFormatter());
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

            foreach (IRestHandler routeHandler in m_routeHandlers)
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
            foreach (IRestHandler routeHandler in m_routeHandlers)
            {
                UnvalidatedHandlerRegistry.Add(routeHandler);
            }

            return this;
        }
    }
}
