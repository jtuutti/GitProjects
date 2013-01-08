﻿// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Net;
using RestFoundation.Runtime;

namespace RestFoundation.TypeBinders
{
    /// <summary>
    /// Represents a type binder that binds simple types from HTTP body key value pairs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class FromBodyAttribute : TypeBinderAttribute
    {
        /// <summary>
        /// Binds data from an HTTP body key value pair to a service method parameter.
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

            if (context.Request.Method != HttpMethod.Post && context.Request.Method != HttpMethod.Put && context.Request.Method != HttpMethod.Patch)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.InvalidHttpMethodForFromBodyBinder);
            }

            return objectType.IsArray ? BindArray(name, objectType, context) : BindObject(name, objectType, context);
        }

        private static object BindObject(string name, Type objectType, IServiceContext context)
        {
            string value = context.GetHttpContext().Request.Form.Get(name);
            object changedValue;

            return SafeConvert.TryChangeType(value, objectType, out changedValue) ? changedValue : null;
        }

        private static object BindArray(string name, Type objectType, IServiceContext context)
        {
            Type elementType = objectType.GetElementType();

            string[] values = context.GetHttpContext().Request.Form.GetValues(name) ?? new string[0];
            var changedValues = Array.CreateInstance(elementType, values.Length);

            for (int i = 0; i < values.Length; i++)
            {
                object changedArrayValue;
                changedValues.SetValue(SafeConvert.TryChangeType(values[i], elementType, out changedArrayValue) ? changedArrayValue : null, i);
            }

            return changedValues;
        }
    }
}
