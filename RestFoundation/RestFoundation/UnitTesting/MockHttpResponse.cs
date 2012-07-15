using System;
using System.Web;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents a mock HTTP response.
    /// </summary>
    public class MockHttpResponse : Context.HttpResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockHttpResponse"/> class.
        /// </summary>
        /// <param name="output">The HTTP response output.</param>
        public MockHttpResponse(IHttpResponseOutput output) : base(output)
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
