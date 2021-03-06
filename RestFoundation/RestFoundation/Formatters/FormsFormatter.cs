﻿// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Web;
using RestFoundation.Resources;
using RestFoundation.Results;
using RestFoundation.Runtime;

namespace RestFoundation.Formatters
{
    /// <summary>
    /// Represents an HTTP form data formatter.
    /// </summary>
    [SupportedMediaType("application/x-www-form-urlencoded")]
    public class FormsFormatter : IMediaTypeFormatter
    {
        /// <summary>
        /// Gets a value indicating whether the formatter can format message body in HTTP
        /// requests.
        /// </summary>
        public virtual bool CanFormatRequest
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the formatter can format objects returned by service
        /// methods into HTTP response.
        /// </summary>
        public virtual bool CanFormatResponse
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Deserializes HTTP message body data into an object instance of the provided type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="objectType">The object type.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="HttpResponseException">If the object cannot be deserialized.</exception>
        public virtual object FormatRequest(IServiceContext context, Type objectType)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            TryValidateFormValues(context.GetHttpContext());

            object resource = InitializeResource(objectType);

            if (resource == null)
            {
                return null;
            }

            NameValueCollection formData = PopulateFormData(context);

            if (formData.Count == 0)
            {
                return resource;
            }

            dynamic dynamicResource = resource as DynamicResult;

            if (dynamicResource != null)
            {
                PopulateDynamicResourceData(dynamicResource, formData);
                return dynamicResource;
            }

            PopulateResourceData(resource, formData);
            return resource;
        }

        /// <summary>
        /// Serializes the object instance into the HTTP response stream using the accepted media type.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="methodReturnType">The method return type.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="preferredMediaType">The preferred media type.</param>
        /// <returns>A service method result containing the serialized object representation.</returns>
        /// <exception cref="HttpResponseException">If the object could not be serialized.</exception>
        public virtual IResult FormatResponse(IServiceContext context, Type methodReturnType, object obj, string preferredMediaType)
        {
            throw new NotSupportedException();
        }

        private static void TryValidateFormValues(HttpContextBase httpContext)
        {
            if (!ServiceRequestValidator.IsUnvalidatedRequest(httpContext))
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                httpContext.Request.Form.ToString();
            }
        }

        private static NameValueCollection PopulateFormData(IServiceContext context)
        {
            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            var streamReader = new StreamReader(context.Request.Body, context.Request.Headers.ContentCharsetEncoding);
            NameValueCollection formData = ParseFormData(streamReader.ReadToEnd());

            return formData;
        }

        private static object InitializeResource(Type objectType)
        {
            if (objectType == typeof(object))
            {
                return new DynamicResult();
            }

            try
            {
                return FormatterServices.GetUninitializedObject(objectType);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static NameValueCollection ParseFormData(string formInput)
        {
            var formData = new NameValueCollection();

            if (String.IsNullOrWhiteSpace(formInput))
            {
                return formData;
            }

            string[] formInputArray = formInput.Split('&');

            foreach (var nameValuePair in formInputArray)
            {
                ParseNameValuePairs(nameValuePair, formData);
            }

            return formData;
        }

        private static void ParseNameValuePairs(string nameValuePair, NameValueCollection formData)
        {
            if (String.IsNullOrEmpty(nameValuePair) || nameValuePair.IndexOf('=') <= 0)
            {
                return;
            }

            string[] nameValueArray = nameValuePair.Split('=');

            if (nameValueArray.Length != 2)
            {
                return;
            }

            string name = (HttpUtility.UrlDecode(nameValueArray[0]) ?? String.Empty).Trim();
            string value = HttpUtility.UrlDecode(nameValueArray[1]);

            if (!String.IsNullOrEmpty(name))
            {
                formData.Add(name, value);
            }
        }

        private static void PopulateDynamicResourceData(DynamicResult resource, NameValueCollection formData)
        {
            foreach (string name in formData.AllKeys)
            {
                string[] values = formData.GetValues(name);

                if (values == null || values.Length == 0)
                {
                    continue;
                }

                try
                {
                    if (values.Length == 1)
                    {
                        resource.Add(name, values[0]);
                    }
                    else
                    {
                        resource.Add(name, values);
                    }
                }
                catch (ArgumentException)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest, Global.InvalidDynamicPropertyName);
                }
            }
        }

        private static void PopulateResourceData(object resource, NameValueCollection formData)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(resource);

            foreach (PropertyDescriptor property in properties)
            {
                if (property.IsReadOnly)
                {
                    continue;
                }

                string[] value = formData.GetValues(property.Name);

                if (value == null || value.Length == 0)
                {
                    continue;
                }

                if (property.PropertyType.IsArray || IsGenericCollection(property.PropertyType))
                {
                    property.SetValue(resource, value);
                }
                else
                {
                    object propertyValue;

                    if (!SafeConvert.TryChangeType(value[0], property.PropertyType, out propertyValue))
                    {
                        propertyValue = null;
                    }

                    property.SetValue(resource, propertyValue);
                }
            }
        }

        private static bool IsGenericCollection(Type propertyType)
        {
            return propertyType.IsGenericType &&
                   (propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                    propertyType.GetGenericTypeDefinition().GetInterface(typeof(IEnumerable<>).FullName) != null);
        }
    }
}
