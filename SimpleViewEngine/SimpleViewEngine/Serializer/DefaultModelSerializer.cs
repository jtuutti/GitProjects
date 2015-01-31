using System.Web.Helpers;

namespace SimpleViewEngine.Serializer
{
    /// <summary>
    /// Represents the default model serializer.
    /// </summary>
    public class DefaultModelSerializer : IModelSerializer
    {
        /// <summary>
        /// Serializes the provided model instance.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="string"/> containing the JSON representation of the model.</returns>
        public string Serialize(object model)
        {
            return Json.Encode(model);
        }
    }
}
