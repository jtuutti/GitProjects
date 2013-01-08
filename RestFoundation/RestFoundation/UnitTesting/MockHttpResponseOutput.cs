// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Web;
using RestFoundation.Context;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents a mock HTTP response output.
    /// </summary>
    public class MockHttpResponseOutput : HttpResponseOutput
    {
        /// <summary>
        /// Gets the underlying <see cref="HttpContextBase"/> instance.
        /// </summary>
        protected override HttpContextBase Context
        {
            get
            {
                HttpContextBase context = MockHandlerFactory.Context;

                if (context == null)
                {
                    throw new InvalidOperationException(RestResources.MissingHttpContext);
                }

                return context;
            }
        }
    }
}
