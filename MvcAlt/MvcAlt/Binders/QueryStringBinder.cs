using System;
using System.Collections.Generic;
using System.Reflection;
using MvcAlt.Infrastructure;

namespace MvcAlt.Binders
{
    public class QueryStringBinder : IBinder
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

            var bindedProperties = new List<string>();

            foreach (PropertyInfo property in properties)
            {
                if (BindProperty(resource, request, property))
                {
                    bindedProperties.Add(property.Name);
                }
            }

            return bindedProperties.ToArray();
        }

        private static bool BindProperty(object resource, IHttpRequest request, PropertyInfo property)
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