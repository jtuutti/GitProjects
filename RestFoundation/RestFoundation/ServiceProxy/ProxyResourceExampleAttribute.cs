﻿// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Represents a service proxy decorator attribute that specifies a type responsible for
    /// building example resource object instances.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ProxyResourceExampleAttribute : Attribute
    {
        private Type m_requestBuilderType;
        private Type m_responseBuilderType;

        /// <summary>
        /// Gets or sets the request example builder type.
        /// </summary>
        public Type RequestBuilderType
        {
            get
            {
                return m_requestBuilderType;
            }
            set
            {
                ValidateResourceType(value);

                m_requestBuilderType = value;
            }
        }

        /// <summary>
        /// Gets or sets the response example builder type.
        /// </summary>
        public Type ResponseBuilderType
        {
            get
            {
                return m_responseBuilderType;
            }
            set
            {
                ValidateResourceType(value);

                m_responseBuilderType = value;
            }
        }

        private static void ValidateResourceType(Type value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (!value.IsClass || value.IsAbstract || !typeof(IResourceExampleBuilder).IsAssignableFrom(value))
            {
                throw new ArgumentException(RestResources.InvalidResourceExampleType, "value");
            }
        }
    }
}
