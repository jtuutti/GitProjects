// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Web;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents a mock HTTP request.
    /// </summary>
    public class MockHttpRequest : Context.HttpRequest
    {
        /// <summary>
        /// Gets the underlying <see cref="HttpContextBase"/> instance.
        /// </summary>
        protected override HttpContextBase Context
        {
            get
            {
                HttpContextBase context = TestHttpContext.Context;

                if (context == null)
                {
                    throw new InvalidOperationException(RestResources.MissingHttpContext);
                }

                return context;
            }
        }
    }
}
