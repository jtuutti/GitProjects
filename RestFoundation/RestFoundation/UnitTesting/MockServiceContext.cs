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
        public MockServiceContext(IHttpRequest request, IHttpResponse response) : base(request, response)
        {
        }

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
                    throw new InvalidOperationException("No HTTP context was found");
                }

                return context;
            }
        }
    }
}
