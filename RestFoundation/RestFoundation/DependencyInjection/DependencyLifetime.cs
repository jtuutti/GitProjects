namespace RestFoundation.DependencyInjection
{
    /// <summary>
    /// A dependency lifetime.
    /// </summary>
    public enum DependencyLifetime
    {
        /// <summary>
        /// Per instance
        /// </summary>
        Transient,

        /// <summary>
        /// Single instance
        /// </summary>
        Singleton
    }
}
