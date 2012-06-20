﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Web.Routing;
using System.Web.Util;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public static class RoutingExtensions
    {
        private const char Slash = '/';
        private const char Tilda = '~';

        private static readonly Type urlAttributeType = typeof(UrlAttribute);

        static RoutingExtensions()
        {
            RequestValidator.Current = new ServiceRequestValidator();
        }

        public static RouteExtensionMap MapRestRoute<TContract>(this RouteCollection routes, string url)
        {
            return MapRestRoute(routes, url, typeof(TContract), false);
        }

        public static RouteExtensionMap MapRestRoute(this RouteCollection routes, string url, Type serviceContractType)
        {
            return MapRestRoute(routes, url, serviceContractType, false);
        }

        public static RouteExtensionMap MapRestRouteAsync<TContract>(this RouteCollection routes, string url)
        {
            return MapRestRoute(routes, url, typeof(TContract), true);
        }

        public static RouteExtensionMap MapRestRouteAsync(this RouteCollection routes, string url, Type serviceContractType)
        {
            return MapRestRoute(routes, url, serviceContractType, true);
        }

        private static RouteExtensionMap MapRestRoute(this RouteCollection routes, string url, Type serviceContractType, bool isAsync)
        {
            if (routes == null) throw new ArgumentNullException("routes");
            if (url == null) throw new ArgumentNullException("url");
            if (url.Trim().Length == 0) throw new ArgumentException("Route url cannot be null or empty", "url");
            if (serviceContractType == null) throw new ArgumentNullException("serviceContractType");
            if (!serviceContractType.IsInterface) throw new ArgumentException("Service contract type must be an interface", "serviceContractType");

            var actionMethods = serviceContractType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                                   .Where(m => m.GetCustomAttributes(urlAttributeType, true).Length > 0);

            List<ActionMethodMetadata> actionMethodMetadata = GenerateActionMethodMetadata(serviceContractType, actionMethods);
            ActionMethodRegistry.ActionMethods.AddOrUpdate(serviceContractType, t => actionMethodMetadata, (t, u) => actionMethodMetadata);

            IEnumerable<IRouteHandler> routeHandlers = MapRoutes(actionMethodMetadata, routes, url, serviceContractType, isAsync);
            return new RouteExtensionMap(routeHandlers);
        }

        public static void AddGlobalBehaviors(this RouteCollection routes, params IServiceBehavior[] behaviors)
        {
            if (routes == null) throw new ArgumentNullException("routes");
            if (behaviors == null) throw new ArgumentNullException("behaviors");

            if (behaviors.GroupBy(s => s.GetType()).Max(g => g.Count()) > 1)
            {
                throw new InvalidOperationException("Multiple global service behaviors of the same type are not allowed");
            }

            for (int i = 0; i < behaviors.Length; i++)
            {
                BehaviorRegistry.AddGlobalBehavior(behaviors[i]);
            }
        }

        public static IEnumerable<IServiceBehavior> GetGlobalBehaviors(this RouteCollection routes)
        {
            return new ReadOnlyCollection<IServiceBehavior>(BehaviorRegistry.GetGlobalBehaviors());
        }

        public static bool RemoveGlobalBehavior(this RouteCollection routes, IServiceBehavior behavior)
        {
            if (routes == null) throw new ArgumentNullException("routes");
            if (behavior == null) throw new ArgumentNullException("behavior");

            return BehaviorRegistry.RemoveGlobalBehavior(behavior);
        }

        public static void ClearGlobalBehaviors(this RouteCollection routes)
        {
            if (routes == null) throw new ArgumentNullException("routes");

            BehaviorRegistry.ClearGlobalBehaviors();
        }

        private static List<ActionMethodMetadata> GenerateActionMethodMetadata(Type serviceContractType, IEnumerable<MethodInfo> actionMethods)
        {
            var urlAttributes = new List<ActionMethodMetadata>();

            foreach (MethodInfo actionMethod in actionMethods)
            {
                foreach (UrlAttribute urlAttribute in Attribute.GetCustomAttributes(actionMethod, urlAttributeType, true).Cast<UrlAttribute>())
                {
                    var actionMetadata = new ActionMethodMetadata(urlAttribute,
                                                                  actionMethod,
                                                                  (OutputCacheAttribute) Attribute.GetCustomAttribute(actionMethod, typeof(OutputCacheAttribute), true));
                    urlAttributes.Add(actionMetadata);

                    var urlMethods = urlAttribute.HttpMethods;

                    HttpMethodRegistry.HttpMethods.AddOrUpdate(new RouteMetadata(serviceContractType.AssemblyQualifiedName, urlAttribute.UrlTemplate),
                                                               template => AddHttpMethods(urlMethods),
                                                               (template, allowedMethods) => UpdateHttpMethods(allowedMethods, urlMethods));
                }
            }

            return urlAttributes;
        }

        private static HashSet<HttpMethod> AddHttpMethods(IEnumerable<HttpMethod> urlMethods)
        {
            var allowedMethods = new HashSet<HttpMethod>();

            foreach (var urlMethod in urlMethods)
            {
                allowedMethods.Add(urlMethod);
            }

            return allowedMethods;
        }

        private static HashSet<HttpMethod> UpdateHttpMethods(HashSet<HttpMethod> allowedMethods, IEnumerable<HttpMethod> urlMethods)
        {
            foreach (var urlMethod in urlMethods)
            {
                allowedMethods.Add(urlMethod);
            }

            return allowedMethods;
        }

        private static IEnumerable<IRouteHandler> MapRoutes(IEnumerable<ActionMethodMetadata> actionMetadata, RouteCollection routes, string url, Type serviceContractType, bool isAsync)
        {
            var routeHandlers = new List<IRouteHandler>();

            url = url.Trim();

            foreach (ActionMethodMetadata urlAttribute in actionMetadata)
            {
                var defaults = new RouteValueDictionary
                                   {
                                       { RouteConstants.ServiceContractType, serviceContractType.AssemblyQualifiedName },
                                       { RouteConstants.ServiceUrl, url },
                                       { RouteConstants.UrlTemplate, urlAttribute.UrlInfo.UrlTemplate },
                                   };

                var routeHandler = isAsync ? (IRouteHandler) ObjectActivator.Create<RestAsyncHandler>() : ObjectActivator.Create<RestHandler>();
                routeHandlers.Add(routeHandler);

                routes.Add(new Route(ConcatUrl(url, urlAttribute.UrlInfo.UrlTemplate.Trim()), defaults, routeHandler));
            }

            return routeHandlers;
        }

        private static string ConcatUrl(string url, string urlTemplate)
        {
            return String.Concat(url.TrimEnd(Slash), Slash, urlTemplate.TrimStart(Slash, Tilda));
        }
    }
}
