// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
namespace RestFoundation
{
    /// <summary>
    /// Defines a dependency instance lifetime for the default service locator.
    /// </summary>
    public enum InstanceLifetime
    {
        /// <summary>
        /// A new instance is created every time the dependency is requested.
        /// </summary>
        PerInstance,

        /// <summary>
        /// A single instance is created per HTTP context.
        /// </summary>
        PerHttpContext,

        /// <summary>
        /// A single instance is used every time the dependency is requested.
        /// </summary>
        Singleton
    }
}
