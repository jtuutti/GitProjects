﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public sealed class RouteConfiguration
    {
        private readonly IEnumerable<IRouteHandler> m_routeHandlers;

        internal RouteConfiguration(IEnumerable<IRouteHandler> routeHandlers)
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

            foreach (IRouteHandler routeHandler in m_routeHandlers)
            {
                for (int i = 0; i < behaviors.Length; i++)
                {
                    IServiceBehavior behavior = behaviors[i];

                    if (behavior != null)
                    {
                        BehaviorRegistry.AddBehavior(routeHandler, behaviors[i]);
                    }
                }
            }

            return this;
        }

        public RouteConfiguration DoNotValidateRequests()
        {           
            foreach (IRouteHandler routeHandler in m_routeHandlers)
            {
                UnvalidatedHandlerRegistry.Add(routeHandler);
            }

            return this;
        }
    }
}