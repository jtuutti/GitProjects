// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Web;
using RestFoundation.Context;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents a mock service context.
    /// </summary>
    public class MockServiceContext : ServiceContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockServiceContext"/> class.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="response">The HTTP response.</param>
        /// <param name="cache">The service cache.</param>
        public MockServiceContext(IHttpRequest request, IHttpResponse response, IServiceCache cache) : base(request, response, cache)
        {
        }

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
                    throw new InvalidOperationException(Resources.Global.MissingHttpContext);
                }

                return context;
            }
        }
    }
}
