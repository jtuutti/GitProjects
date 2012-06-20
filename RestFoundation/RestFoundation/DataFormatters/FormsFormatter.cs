﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using RestFoundation.Runtime;

namespace RestFoundation.DataFormatters
{
    public class FormsFormatter : IDataFormatter
    {
        public object Format(Stream body, Encoding encoding, Type objectType)
        {
            if (body == null) throw new ArgumentNullException("body");
            if (encoding == null) throw new ArgumentNullException("encoding");
            if (objectType == null) throw new ArgumentNullException("objectType");
           
            // A hack to trigger form collection validation
            if (HttpContext.Current.Request.Form.ToString().Length == 0)
            {
                return null;
            }

            object resource = InitializeResource(objectType);

            if (resource == null)
            {
                return null;
            }

            NameValueCollection formData;

            body.Position = 0;

            var streamReader = new StreamReader(HttpContext.Current.Request.InputStream, encoding);
            {
                formData = ParseFormData(streamReader.ReadToEnd());
            }

            if (formData.Count == 0)
            {
                return resource;
            }

            dynamic dynamicResource = resource as DynamicDictionaryObject;

            if (dynamicResource != null)
            {
                PopulateDynamicResourceData(dynamicResource, formData);
                return dynamicResource;
            }

            PopulateResourceData(resource, formData);
            return resource;
        }

        private static object InitializeResource(Type objectType)
        {
            if (objectType == typeof(object))
            {
                return new DynamicDictionaryObject();
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

            string name = HttpUtility.UrlDecode(nameValueArray[0]).Trim();
            string value = HttpUtility.UrlDecode(nameValueArray[1]);

            if (!String.IsNullOrEmpty(name))
            {
                formData.Add(name, value);
            }
        }

        private static void PopulateDynamicResourceData(DynamicDictionaryObject resource, NameValueCollection formData)
        {
            foreach (string name in formData.AllKeys)
            {
                string[] values = formData.GetValues(name);

                if (values == null || values.Length == 0)
                {
                    continue;
                }

                if (values.Length == 1)
                {
                    resource.Add(name, values[0]);
                }
                else
                {
                    resource.Add(name, values);
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
                    property.SetValue(resource, SafeConvert.ChangeType(value[0], property.PropertyType));
                }
            }
        }

        private static bool IsGenericCollection(Type propertyType)
        {
            return propertyType.IsGenericType &&
                   (propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                    propertyType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                    propertyType.GetGenericTypeDefinition() == typeof(IList<>));
        }
    }
}