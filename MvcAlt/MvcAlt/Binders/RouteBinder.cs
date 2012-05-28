using System;
using System.Collections.Generic;
using System.Reflection;
using MvcAlt.Infrastructure;

namespace MvcAlt.Binders
{
    public class RouteBinder : IBinder
    {
        public string[] Bind(object resource, IHttpRequest request)
        {
            if (resource == null) throw new ArgumentNullException("resource");
            if (request == null) throw new ArgumentNullException("request");

            Type resourceType = resource.GetType();

            if (!resourceType.IsPrimitive)
            {
                return BindComplexType(resource, request);
            }

            return new string[0];
        }

        private static string[] BindComplexType(object resource, IHttpRequest request)
        {
            PropertyInfo[] properties = resource.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var caseInsensitiveRouteValues = new Dictionary<string, object>(request.RouteValues, StringComparer.OrdinalIgnoreCase);
            var bindedProperties = new List<string>();

            foreach (PropertyInfo property in properties)
            {
                if (BindComplexProperty(resource, caseInsensitiveRouteValues, property))
                {
                    bindedProperties.Add(property.Name);
                }
            }

            return bindedProperties.ToArray();
        }

        private static bool BindComplexProperty(object resource, IDictionary<string, object> routeValues, PropertyInfo property)
        {
            if (!property.CanWrite || property.GetIndexParameters().Length > 0)
            {
                return false;
            }

            object propertyValue;

            if (routeValues.TryGetValue(property.Name, out propertyValue) && propertyValue != null)
            {
                property.SetValue(resource, Coerce.ChangeType(propertyValue, property.PropertyType), null);
                return true;
            }

            return false;
        }
    }
}
