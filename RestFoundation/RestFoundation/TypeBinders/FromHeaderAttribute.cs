﻿// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Net;
using RestFoundation.Runtime;

namespace RestFoundation.TypeBinders
{
    /// <summary>
    /// Represents a type binder that binds simple types from HTTP headers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class FromHeaderAttribute : TypeBinderAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FromHeaderAttribute"/> class.
        /// </summary>
        public FromHeaderAttribute()
        {
            ConvertUnderscoresToDashes = true;
        }

        /// <summary>
        /// Gets or sets a name to resolve from the HTTP headers.
        /// If this value is not set, the parameter name will be used.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether to convert underscores in the parameter to dashes
        /// when finding the HTTP header. The default value is true. This value has no effect
        /// if the <see cref="Name"/> property is set.
        /// </summary>
        public bool ConvertUnderscoresToDashes { get; set; }

        /// <summary>
        /// Binds data from an HTTP header to a service method parameter.
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

            if (objectType.IsArray)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, Resources.Global.UnsupportedFromHeaderBinderParameter);
            }

            return BindObject(name, objectType, context);
        }

        private string GetHeaderName(string name)
        {
            if (!String.IsNullOrWhiteSpace(Name))
            {
                return Name.Trim();
            }

            return ConvertUnderscoresToDashes ? name.Replace("_", "-") : name;
        }

        private object BindObject(string name, Type objectType, IServiceContext context)
        {
            string value = context.Request.Headers.TryGet(GetHeaderName(name));
            object changedValue;

            return SafeConvert.TryChangeType(value, objectType, out changedValue) ? changedValue : null;
        }
    }
}
