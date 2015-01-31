namespace SimpleViewEngine.Serializer
{
    /// <summary>
    /// Defines a JSON model serializer.
    /// </summary>
    public interface IModelSerializer
    {
        /// <summary>
        /// Serializes the provided model instance.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="string"/> containing the JSON representation of the model.</returns>
        string Serialize(object model);
    }
}
