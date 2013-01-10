// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents a mock service context manager.
    /// </summary>
    public static class MockContextManager
    {
        private const string RootVirtualPath = "~/";

        /// <summary>
        /// Creates the current HTTP context.
        /// </summary>
        /// <returns>The service context.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the current HTTP context has already been initialized.
        /// </exception>
        public static IServiceContext GenerateContext()
        {
            return GenerateContext(RootVirtualPath, HttpMethod.Get);
        }

        /// <summary>
        /// Creates the current HTTP context with the provided HTTP method.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <returns>The service context.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the current HTTP context has already been initialized.
        /// </exception>
        public static IServiceContext GenerateContext(HttpMethod method)
        {
            return GenerateContext(RootVirtualPath, method);
        }

        /// <summary>
        /// Creates the current HTTP context with the provided virtual path and the HTTP method.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="method">The HTTP method.</param>
        /// <returns>The service context.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the current HTTP context has already been initialized.
        /// </exception>
        public static IServiceContext GenerateContext(string virtualPath, HttpMethod method)
        {
            if (TestHttpContext.Context != null)
            {
                throw new InvalidOperationException("HTTP context has already been initialized");
            }

            TestHttpContext.Context = new TestHttpContext(virtualPath, method.ToString().ToUpperInvariant());

            return Rest.Configuration.ServiceLocator.GetService<IServiceContext>();
        }

        /// <summary>
        /// Destroys the current HTTP context.
        /// </summary>
        public static void DestroyContext()
        {
            TestHttpContext.Context = null;
        }
    }
}
