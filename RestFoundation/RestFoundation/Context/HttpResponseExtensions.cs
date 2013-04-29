using System;
using System.Threading;

namespace RestFoundation.Context
{
    /// <summary>
    /// Contains <see cref="IHttpResponse"/> extensions.
    /// </summary>
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Creates a <see cref="CancellationToken"/> for the HTTP response. If the response
        /// already has an associated <see cref="CancellationTokenSource"/>, the existing
        /// <see cref="CancellationToken"/> will be returned.
        /// </summary>
        /// <param name="response">The <see cref="IHttpResponse"/> instance.</param>
        /// <returns>The associated cancellation token.</returns>
        public static CancellationToken CreateCancellationToken(this IHttpResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (response.CancellationTokenSource != null)
            {
                return response.CancellationTokenSource.Token;
            }

            response.CancellationTokenSource = new CancellationTokenSource();

            return response.CancellationTokenSource.Token;
        }

        /// <summary>
        /// Returns a <see cref="CancellationToken"/> associated with the HTTP response.
        /// </summary>
        /// <param name="response">The <see cref="IHttpResponse"/> instance.</param>
        /// <returns>The associated cancellation token.</returns>
        public static CancellationToken GetCancellationToken(this IHttpResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            return response.CancellationTokenSource != null ? response.CancellationTokenSource.Token : CancellationToken.None;
        }
    }
}
