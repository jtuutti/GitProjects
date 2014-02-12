// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using RestFoundation.Resources;
using RestFoundation.Results;
using RestFoundation.Runtime;

namespace RestFoundation.TypeBinders
{
    /// <summary>
    /// Represents a type binder that binds complex class types from URI query string parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class FromUriAsComplexTypeAttribute : TypeBinderAttribute
    {
        /// <summary>
        /// Indicates whether the type binder should return fault collection if a query string value
        /// cannot be converted into the property type.
        /// </summary>
        public bool AssertTypeConversion { get; set; }

        /// <summary>
        /// Binds data from an HTTP route, query string or message to a service method parameter.
        /// </summary>
        /// <param name="name">The service method parameter name.</param>
        /// <param name="objectType">The binded object type.</param>
        /// <param name="context">The service context.</param>
        /// <returns>The object instance with the data or null.</returns>
        public override object Bind(string name, Type objectType, IServiceContext context)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (!objectType.IsClass || objectType.IsAbstract)
            {
                throw new InvalidOperationException(Global.InvalidComplexType);
            }

            if (objectType == typeof(object))
            {
                return PopulateDynamicObject(context);
            }

            object instance = Activator.CreateInstance(objectType, true);

            List<string> faultMessages = BindProperties(instance, context);

            if (faultMessages.Count > 0)
            {
                throw new HttpResourceFaultException(faultMessages);
            }

            return instance;
        }

        private static dynamic PopulateDynamicObject(IServiceContext context)
        {
            dynamic instance = new DynamicResult();

            foreach (string key in context.Request.QueryString.Keys)
            {
                IList<string> values = context.Request.QueryString.GetValues(key);
                string propertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(key);

                try
                {
                    if (values.Count == 1)
                    {
                        instance.Add(propertyName, values[0]);
                    }
                    else
                    {
                        instance.Add(propertyName, values);
                    }
                }
                catch (ArgumentException)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest, Global.InvalidDynamicPropertyName);
                }
            }

            return instance;
        }

        private static IList<string> GetUriValues(Type objectType, PropertyInfo property, IServiceContext context)
        {
            IList<string> uriValues = context.Request.QueryString.GetValues(property.Name);

            if (uriValues.Count == 0)
            {
                const BindingFlags PropertyFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

                var pluralization = PluralizationService.CreateService(CultureInfo.CurrentCulture);
                var singularName = pluralization.Singularize(property.Name);

                if (objectType.GetProperties(PropertyFlags).All(x => !String.Equals(singularName, x.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    uriValues = context.Request.QueryString.GetValues(singularName);
                }
            }

            return uriValues;
        }

        private List<string> BindProperties(object instance, IServiceContext context)
        {
            Type objectType = instance.GetType();

            var properties = objectType.GetProperties().Where(p => p.CanWrite && p.GetIndexParameters().Length == 0);
            var faultMessages = new List<string>();

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType.IsArray)
                {
                    List<string> propertyFaultMessages;

                    if (!BindArray(instance, property, context, out propertyFaultMessages))
                    {
                        faultMessages.AddRange(propertyFaultMessages);
                    }
                }
                else
                {
                    string propertyFaultMessage;

                    if (!BindObject(instance, property, context, out propertyFaultMessage))
                    {
                        faultMessages.Add(propertyFaultMessage);
                    }
                }
            }

            return faultMessages;
        }

        private bool BindObject(object instance, PropertyInfo property, IServiceContext context, out string faultMessage)
        {
            string uriValue = context.Request.QueryString.TryGet(property.Name);

            if (uriValue == null)
            {
                faultMessage = null;
                return true;
            }

            object changedValue;

            if (SafeConvert.TryChangeType(uriValue, property.PropertyType, out changedValue))
            {
                property.SetValue(instance, changedValue, null);
            }
            else if (AssertTypeConversion)
            {
                faultMessage = String.Format(CultureInfo.InvariantCulture,
                                             Global.InvalidComplexTypePropertyValue,
                                             uriValue,
                                             property.Name,
                                             property.PropertyType.Name,
                                             instance.GetType().Name);

                return false;
            }

            faultMessage = null;
            return true;
        }

        private bool BindArray(object instance, PropertyInfo property, IServiceContext context, out List<string> faultMessages)
        {
            faultMessages = new List<string>();

            Type elementType = property.PropertyType.GetElementType();
            IList<string> uriValues = GetUriValues(instance.GetType(), property, context);

            if (uriValues.Count == 0)
            {
                property.SetValue(instance, Array.CreateInstance(elementType, 0), null);
                return true;
            }

            var changedValues = Array.CreateInstance(elementType, uriValues.Count);

            for (int i = 0; i < uriValues.Count; i++)
            {
                object changedArrayValue;

                if (SafeConvert.TryChangeType(uriValues[i], elementType, out changedArrayValue))
                {
                    changedValues.SetValue(changedArrayValue, i);
                }
                else if (AssertTypeConversion)
                {
                    string faultMessage = String.Format(CultureInfo.InvariantCulture,
                                                        Global.InvalidComplexTypePropertyValue,
                                                        uriValues[i] ?? "(null)",
                                                        property.Name,
                                                        elementType.Name + "[]",
                                                        instance.GetType().Name);

                    faultMessages.Add(faultMessage);
                }
            }

            property.SetValue(instance, changedValues, null);

            return faultMessages.Count == 0;
        }
    }
}
