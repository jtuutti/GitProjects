// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
namespace RestFoundation.Client
{
    /// <summary>
    /// Describes a REST HTTP resource type.
    /// </summary>
    public enum RestResourceType
    {
        /// <summary>
        /// A resource without a body.
        /// </summary>
        None,

        /// <summary>
        /// An object serialized as JSON.
        /// </summary>
        Json,

        /// <summary>
        /// An object serialized as XML.
        /// </summary>
        Xml
    }
}
