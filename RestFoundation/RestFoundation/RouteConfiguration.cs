using System;
using System.Collections.Generic;
using System.Linq;
using RestFoundation.Behaviors;
using RestFoundation.DataFormatters;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public sealed class RouteConfiguration
    {
        private readonly IEnumerable<IRestHandler> m_routeHandlers;

        internal RouteConfiguration(IEnumerable<IRestHandler> routeHandlers)
        {
            if (routeHandlers == null) throw new ArgumentNullException("routeHandlers");

            m_routeHandlers = routeHandlers;
        }

        public RouteConfiguration WithBehaviors(params IServiceBehavior[] behaviors)
        {
            if (behaviors == null) throw new ArgumentNullException("behaviors");

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

        public RouteConfiguration WithContentTypesRestrictedTo(params string[] contentTypes)
        {
            if (contentTypes == null) throw new ArgumentNullException("contentTypes");
            if (contentTypes.Length == 0) throw new ArgumentException("At least one content type must be provided", "contentTypes");

            foreach (IRestHandler routeHandler in m_routeHandlers)
            {
                for (int i = 0; i < contentTypes.Length; i++)
                {
                    string contentType = contentTypes[i];

                    if (contentType == null || DataFormatterRegistry.GetFormatter(contentType) == null)
                    {
                        throw new InvalidOperationException(String.Format("Content type '{0}' does not have an associated data formatter", contentType ?? "(null)"));
                    }
                }

                ServiceBehaviorRegistry.AddBehavior(routeHandler, new ContentTypeBehavior(contentTypes));
            }

            return this;
        }

        public RouteConfiguration WithIPsRestrictedBySection(string nameValueSectionName)
        {
            if (String.IsNullOrEmpty(nameValueSectionName)) throw new ArgumentNullException("nameValueSectionName");

            foreach (IRestHandler routeHandler in m_routeHandlers)
            {
                ServiceBehaviorRegistry.AddBehavior(routeHandler, new AclBehavior(nameValueSectionName));
            }

            return this;
        }

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
