using System;
using System.Collections.Generic;
using System.Reflection;
using MvcAlt.Infrastructure;

namespace MvcAlt.Binders
{
    public class QueryBinder : IBinder
    {
        public string[] Bind(IHttpRequest request, string parameterName, Type resourceType, ref object resource)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (resourceType == null) throw new ArgumentNullException("resourceType");

            return !IsPrimitiveType(resourceType) ? BindComplexType(request, resource) : BindSimpleType(request, parameterName, resourceType, ref resource);
        }

        private static bool IsPrimitiveType(Type resourceType)
        {
            return resourceType.IsPrimitive || resourceType.IsEnum || resourceType == typeof(Guid) || resourceType == typeof(decimal) ||
                  (resourceType.IsGenericType && resourceType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        private static string[] BindSimpleType(IHttpRequest request, string parameterName, Type resourceType, ref object resource)
        {
            object value = Coerce.ChangeType(request.Query[parameterName], resourceType);

            if (value != null)
            {
                resource = value;
            }

            return new string[0];
        }

        private static string[] BindComplexType(IHttpRequest request, object resource)
        {
            if (resource == null) throw new ArgumentNullException("resource");

            PropertyInfo[] properties = resource.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var bindedProperties = new List<string>();

            foreach (PropertyInfo property in properties)
            {
                if (BindProperty(request, resource, property))
                {
                    bindedProperties.Add(property.Name);
                }
            }

            return bindedProperties.ToArray();
        }

        private static bool BindProperty(IHttpRequest request, object resource, PropertyInfo property)
        {
            if (!property.CanWrite || property.GetIndexParameters().Length > 0)
            {
                return false;
            }

            object propertyValue = request.Query[property.Name];

            if (propertyValue == null && property.Name.IndexOf('_') >= 0)
            {
                propertyValue = request.Query[property.Name.Replace('_', '-')];
            }

            if (propertyValue != null)
            {
                property.SetValue(resource, Coerce.ChangeType(propertyValue, property.PropertyType), null);
                return true;
            }

            return false;
        }
    }
}