namespace RestFoundation
{
    /// <summary>
    /// Represents an HTTP protocol scheme.
    /// </summary>
    public enum HttpScheme
    {
        /// <summary>
        /// No scheme - a relative URL
        /// </summary>
        None,

        /// <summary>
        /// HTTP scheme
        /// </summary>
        Http,

        /// <summary>
        /// HTTPS (secure) scheme
        /// </summary>
        Https
    }
}
