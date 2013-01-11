// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using RestFoundation.Runtime;

namespace RestFoundation.TypeBinders
{
    /// <summary>
    /// Represents a type binder that binds simple types from URI query string parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class FromUriAttribute : TypeBinderAttribute
    {
        /// <summary>
        /// Gets or sets a name to resolve from the URI query.
        /// If this value is not set, the parameter name will be used.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Binds data from a URI query string parameter to a service method parameter.
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

            if (!String.IsNullOrWhiteSpace(Name))
            {
                name = Name.Trim();
            }

            return objectType.IsArray ? BindArray(name, objectType, context) : BindObject(name, objectType, context);
        }

        private static object BindObject(string name, Type objectType, IServiceContext context)
        {
            string value = context.Request.QueryString.TryGet(name);
            object changedValue;

            return SafeConvert.TryChangeType(value, objectType, out changedValue) ? changedValue : null;
        }

        private static object BindArray(string name, Type objectType, IServiceContext context)
        {
            Type elementType = objectType.GetElementType();

            IList<string> values = context.Request.QueryString.GetValues(name);
            var changedValues = Array.CreateInstance(elementType, values.Count);

            for (int i = 0; i < values.Count; i++)
            {
                object changedArrayValue;
                changedValues.SetValue(SafeConvert.TryChangeType(values[i], elementType, out changedArrayValue) ? changedArrayValue : null, i);
            }

            return changedValues;
        }
    }
}
