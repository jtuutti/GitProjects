using System;
using System.Collections.Generic;
using System.Reflection;
using MvcAlt.Infrastructure;

namespace MvcAlt.Binders
{
    public class RouteBinder : IBinder
    {
        public string[] Bind(IHttpRequest request, string parameterName, Type resourceType, ref object resource)
        {
            if (request == null) throw new ArgumentNullException("request");

            return !TypeHelper.IsSimpleType(resourceType) ? BindComplexType(request, resource) : BindSimpleType(request, parameterName, resourceType, ref resource);
        }

        private static string[] BindSimpleType(IHttpRequest request, string parameterName, Type resourceType, ref object resource)
        {
            object value;

            if (request.RouteValues.TryGetValue(parameterName, out value) && value != null)
            {
                resource = Coerce.ChangeType(value, resourceType);
            }

            return new string[0];
        }

        private static string[] BindComplexType(IHttpRequest request, object resource)
        {
            if (resource == null) throw new ArgumentNullException("resource");

            PropertyInfo[] properties = resource.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var caseInsensitiveRouteValues = new Dictionary<string, object>(request.RouteValues, StringComparer.OrdinalIgnoreCase);
            var bindedProperties = new List<string>();

            foreach (PropertyInfo property in properties)
            {
                if (BindComplexProperty(caseInsensitiveRouteValues, resource, property))
                {
                    bindedProperties.Add(property.Name);
                }
            }

            return bindedProperties.ToArray();
        }

        private static bool BindComplexProperty(IDictionary<string, object> routeValues, object resource, PropertyInfo property)
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
