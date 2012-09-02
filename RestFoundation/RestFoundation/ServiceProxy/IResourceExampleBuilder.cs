// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System.Xml.Serialization;
using RestFoundation.Runtime;

namespace RestFoundation.ServiceProxy
{
    /// <summary>
    /// Defines a resource example builder.
    /// </summary>
    public interface IResourceExampleBuilder
    {
        /// <summary>
        /// Builds and returns an instance of the example resource class.
        /// </summary>
        /// <returns>An example resource object instance.</returns>
        object BuildInstance();

        /// <summary>
        /// Builds and returns XML schemas for the resource class.
        /// See <seealso cref="XmlSchemaGenerator"/> for an automatic XML schema generator.
        /// Return null from this method, if you do not need XML schemas to be displayed.
        /// </summary>
        /// <returns>
        /// An list of XML schemas for the response object.
        /// </returns>
        XmlSchemas BuildSchemas();
    }
}
