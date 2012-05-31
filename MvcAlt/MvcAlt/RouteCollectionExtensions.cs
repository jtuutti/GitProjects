using System;
using System.Linq;
using System.Reflection;
using System.Web.Routing;
using MvcAlt.Infrastructure;

namespace MvcAlt
{
    public static class RouteCollectionExtensions
    {
        public static void AddControllerDefinedRoutes(this RouteCollection routes, string controllerAssemblyName)
        {
            if (String.IsNullOrEmpty(controllerAssemblyName)) throw new ArgumentNullException("controllerAssemblyName");

            AddControllerDefinedRoutes(routes, Assembly.Load(controllerAssemblyName));
        }

        public static void AddControllerDefinedRoutes(this RouteCollection routes, Assembly controllerAssembly)
        {
            if (routes == null) throw new ArgumentNullException("routes");
            if (controllerAssembly == null) throw new ArgumentNullException("controllerAssembly");

            Type[] controllerTypes = controllerAssembly.GetTypes().Where(t => t.IsClass && t.IsPublic && !t.IsAbstract &&
                                                                              t.GetInterface(typeof(IController).FullName) != null)
                                                                  .ToArray();

            foreach (Type controllerType in controllerTypes)
            {
                MethodInfo[] actionMethods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (MethodInfo actionMethod in actionMethods)
                {
                    UrlAttribute[] urlAttributes = Attribute.GetCustomAttributes(actionMethod, typeof(UrlAttribute), true)
                                                            .Cast<UrlAttribute>()
                                                            .OrderByDescending(a => a.Priority)
                                                            .ToArray();

                    foreach (UrlAttribute urlAttribute in urlAttributes)
                    {
                        RegisterRoute(routes, controllerType, actionMethod, urlAttribute);
                    }
                }
            }
        }

        private static void RegisterRoute(RouteCollection routes, Type controllerType, MethodInfo actionMethod, UrlAttribute urlAttribute)
        {
            var systemRoutes = new RouteValueDictionary
            {
                { "_controller", controllerType.Name },
                { "_method", actionMethod.Name }
            };

            var constraints = new RouteValueDictionary();

            if (urlAttribute.Verbs.Length > 0)
            {
                constraints.Add("httpMethod", new HttpVerbConstraint(urlAttribute.Verbs));
            }

            var route = new Route(urlAttribute.Url, systemRoutes, constraints, new RouteHandler());
            routes.Add(route);
        }
    }
}
