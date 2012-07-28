using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a dynamic XML document.
    /// </summary>
    public class DynamicXDocument : DynamicObject, IEnumerable
    {
        private readonly List<XElement> m_elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicXDocument"/> class.
        /// </summary>
        /// <param name="xml">A <see cref="string"/> containing the XML.</param>
        public DynamicXDocument(string xml)
        {
            if (String.IsNullOrWhiteSpace(xml)) throw new ArgumentNullException("xml");

            XDocument document = XDocument.Parse(xml);
            m_elements = new List<XElement> { StripNamespaces(document.Root) };
        }

        private DynamicXDocument(XElement element)
        {
            m_elements = new List<XElement> { element };
        }

        private DynamicXDocument(IEnumerable<XElement> elements)
        {
            m_elements = new List<XElement>(elements);
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/>
        /// class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior.
        /// (In most cases, a run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">
        /// Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the
        /// dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance
        /// of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase
        /// property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to
        /// <paramref name="result"/>.
        /// </param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder == null) throw new ArgumentNullException("binder");

            result = null;

            switch (binder.Name)
            {
                case "Value":
                    result = m_elements[0].Value;
                    break;
                case "Count":
                    result = m_elements.Count;
                    break;
                default:
                    List<XElement> items = m_elements.Descendants(XName.Get(binder.Name)).ToList();

                    if (items.Count == 1 && !items[0].HasElements)
                    {
                        result = items[0].Value;                           
                    }
                    else if (items.Count > 0)
                    {
                        result = GetItemWithElementsValue(items);
                    }

                    break;
            }

            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that get a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/>
        /// class can override this method to specify dynamic behavior for indexing operations.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior.
        /// (In most cases, a run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">
        /// Provides information about the operation.
        /// </param>
        /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] operation in C# (sampleObject(3) in Visual Basic),
        /// where sampleObject is derived from the DynamicObject class, <paramref name="indexes"/>[0] is equal to 3.
        /// </param>
        /// <param name="result">The result of the index operation.</param>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (binder == null) throw new ArgumentNullException("binder");
            if (indexes == null) throw new ArgumentNullException("indexes");

            var index = (int) indexes[0];
            result = new DynamicXDocument(m_elements[index]);

            return true;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            if (m_elements.Count > 0 && m_elements[0] != null)
            {
                return m_elements[0].Descendants().Any() ? m_elements[0].ToString(SaveOptions.None) : m_elements[0].Value;
            }

            return String.Empty;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public IEnumerator GetEnumerator()
        {
            return m_elements.Select(element => new DynamicXDocument(element)).GetEnumerator();
        }

        private static XElement StripNamespaces(XElement rootElement)
        {
            foreach (XElement element in rootElement.DescendantsAndSelf())
            {
                if (element.Name.Namespace != XNamespace.None)
                {
                    element.Name = XNamespace.None.GetName(element.Name.LocalName);
                }

                bool hasDefinedNamespaces = element.Attributes().Any(attribute => attribute.IsNamespaceDeclaration ||
                                                                                  (attribute.Name.Namespace != XNamespace.None && attribute.Name.Namespace != XNamespace.Xml));
                if (!hasDefinedNamespaces) continue;

                IEnumerable<XAttribute> attributes = element.Attributes().Where(attribute => !attribute.IsNamespaceDeclaration)
                                                                         .Select(attribute =>
                                                                                 attribute.Name.Namespace != XNamespace.None && attribute.Name.Namespace != XNamespace.Xml
                                                                                     ? new XAttribute(XNamespace.None.GetName(attribute.Name.LocalName), attribute.Value)
                                                                                     : attribute);

                element.ReplaceAttributes(attributes);
            }

            return rootElement;
        }

        private static object GetItemWithElementsValue(List<XElement> items)
        {
            if (items.Any(item => item.HasElements))
            {
                return new DynamicXDocument(items);
            }

            var valueList = new List<string>();

            for (int i = 0; i < items.Count; i++)
            {
                valueList.Add(items[i].Value);
            }

            return valueList.ToArray();
        }
    }
}
