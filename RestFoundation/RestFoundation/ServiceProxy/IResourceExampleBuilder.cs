// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System.Xml.Serialization;

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
        /// <returns></returns>
        object BuildInstance();

        /// <summary>
        /// Builds and returns XML schemas for the resource class.
        /// </summary>
        /// <returns></returns>
        XmlSchemas BuildSchemas();
    }
}
