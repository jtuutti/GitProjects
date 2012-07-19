using System;

namespace RestFoundation.DependencyInjection
{
    /// <summary>
    /// Represents a dependency registry for an IoC container.
    /// </summary>
    public interface IDependencyRegistry
    {
        /// <summary>
        /// Register the provided abstraction to the provided implementation type with the specified object lifetime
        /// under the provided optional registration key.
        /// </summary>
        /// <param name="abstractionType">The abstraction type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="lifetime">The object lifetime.</param>
        /// <param name="key">The object registration key.</param>
        void Register(Type abstractionType, Type implementationType, DependencyLifetime lifetime, string key);
    }
}
