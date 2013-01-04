// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a dynamic dictionary result.
    /// </summary>
    [XmlRoot("ComplexType")]
    public class DynamicResult : DynamicObject, IXmlSerializable
    {
        private readonly IDictionary<string, object> m_dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicResult"/> class.
        /// </summary>
        public DynamicResult()
        {
            m_dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets a count of properties.
        /// </summary>
        public int Count
        {
            get
            {
                return m_dictionary.Keys.Count;
            }
        }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether the object contains a property with the provided name.
        /// </summary>
        /// <param name="propertyName">A property name.</param>
        /// <returns><see langword="true"/> if the object contains the property; <see langword="false"/> otherwise.</returns>
        public bool Contains(string propertyName)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            return m_dictionary.ContainsKey(propertyName);
        }

        /// <summary>
        /// Adds a property with the provided name and value.
        /// </summary>
        /// <param name="propertyName">A property name.</param>
        /// <param name="value">A property value.</param>
        /// <returns><see langword="true"/> if the property did not exist and was added; <see langword="false"/> otherwise.</returns>
        public bool Add(string propertyName, object value)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            if (m_dictionary.ContainsKey(propertyName))
            {
                return false;
            }

            m_dictionary.Add(propertyName, value);
            return true;
        }

        /// <summary>
        /// Removes a property with the provided name.
        /// </summary>
        /// <param name="propertyName">A property name.</param>
        /// <returns><see langword="true"/> if the property was removed; <see langword="false"/> otherwise.</returns>
        public bool Remove(string propertyName)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            if (!m_dictionary.ContainsKey(propertyName))
            {
                return false;
            }

            m_dictionary.Remove(propertyName);
            return true;
        }

        /// <summary>
        /// Returns a dictionary of property names and values.
        /// </summary>
        /// <returns>A dictionary of property names and values.</returns>
        public IDictionary<string, object> ToDictionary()
        {
            var dictionary = new SortedDictionary<string, object>();

            foreach (KeyValuePair<string, object> value in m_dictionary)
            {
                dictionary.Add(value.Key, value.Value);
            }

            return dictionary;
        }

        /// <summary>
        /// Clears all the object properties.
        /// </summary>
        public void Clear()
        {
            m_dictionary.Clear();
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the
        /// <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic
        /// behavior for operations such as getting a value for a property.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time
        /// binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">
        /// Provides information about the object that called the dynamic operation. The binder.Name property
        /// provides the name of the member on which the dynamic operation is performed. For example, for the
        /// Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the
        /// class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder. Name returns
        /// "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="result">
        /// The result of the get operation. For example, if the method is called for a property, you can assign the property value
        /// to <paramref name="result"/>.
        /// </param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }

            m_dictionary.TryGetValue(binder.Name, out result);
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the
        /// <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations
        /// such as setting a value for a property.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language
        /// determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">
        /// Provides information about the object that called the dynamic operation. The binder.Name property provides the name of
        /// the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test",
        /// where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class,
        /// binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="value">
        /// The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance
        /// of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test".
        /// </param>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }

            m_dictionary[binder.Name] = value;
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that invoke a member. Classes derived from the
        /// <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations
        /// such as calling a method.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language
        /// determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">
        /// Provides information about the dynamic operation. The binder.Name property provides the name of the member on which the
        /// dynamic operation is performed. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is an
        /// instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns
        /// "SampleMethod". The binder.IgnoreCase property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="args">
        /// The arguments that are passed to the object member during the invoke operation. For example, for the statement
        /// sampleObject.SampleMethod(100), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class,
        /// <paramref name="args"/>[0] is equal to 100.</param>
        /// <param name="result">The result of the member invocation.</param>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }

            if (m_dictionary.ContainsKey(binder.Name) && m_dictionary[binder.Name] is Delegate)
            {
                var del = (Delegate) m_dictionary[binder.Name];
                result = del.DynamicInvoke(args);
                return true;
            }

            return base.TryInvokeMember(binder, args, out result);
        }

        /// <summary>
        /// Provides the implementation for operations that delete an object member. This method is not intended for use in C# or
        /// Visual Basic.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language
        /// determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">Provides information about the deletion.</param>
        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }

            if (m_dictionary.ContainsKey(binder.Name))
            {
                m_dictionary.Remove(binder.Name);
                return true;
            }

            return base.TryDeleteMember(binder);
        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names. 
        /// </summary>
        /// <returns>
        /// A sequence that contains dynamic member names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (string name in m_dictionary.Keys)
            {
                yield return name;
            }
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface,
        /// you should return null (Nothing in Visual Basic) from this method, and instead, if specifying
        /// a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/>
        /// to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object
        /// that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/>
        /// method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/>
        /// method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            foreach (KeyValuePair<string, object> property in m_dictionary)
            {
                writer.WriteStartElement(property.Key);

                var serializableProperty = property.Value as IXmlSerializable;

                if (serializableProperty != null)
                {
                    serializableProperty.WriteXml(writer);
                }
                else if (property.Value != null)
                {
                    SerializeUnknownType(writer, property);
                }

                writer.WriteEndElement();
            }
        }

        private static void SerializeUnknownType(XmlWriter writer, KeyValuePair<string, object> property)
        {
            Type propertyType = property.Value.GetType();

            if (propertyType.IsPrimitive || propertyType == typeof(string) || propertyType == typeof(decimal) || propertyType == typeof(decimal?) ||
                propertyType == typeof(Guid) || propertyType == typeof(Guid?) || propertyType == typeof(DateTime) || propertyType == typeof(DateTime?) ||
                propertyType == typeof(TimeSpan) || propertyType == typeof(TimeSpan?) || propertyType == typeof(DateTimeOffset) || propertyType == typeof(DateTimeOffset?) ||
                propertyType == typeof(CultureInfo) || propertyType == typeof(Uri))
            {
                writer.WriteValue(property.Value);
                return;
            }

            try
            {
                var serializer = XmlSerializerRegistry.Get(propertyType);
                serializer.Serialize(writer, property.Value);
            }
            catch (Exception)
            {
                writer.WriteValue(property.Value);
            }
        }
    }
}
