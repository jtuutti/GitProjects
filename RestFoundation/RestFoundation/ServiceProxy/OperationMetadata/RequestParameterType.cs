namespace RestFoundation.ServiceProxy.OperationMetadata
{
    /// <summary>
    /// Defines an HTTP request parameter type.
    /// </summary>
    public enum RequestParameterType
    {
        /// <summary>
        /// A route parameter.
        /// </summary>
        Route,

        /// <summary>
        /// A query string parameter.
        /// </summary>
        Query,

        /// <summary>
        /// A request body parameter.
        /// </summary>
        Body
    }
}
