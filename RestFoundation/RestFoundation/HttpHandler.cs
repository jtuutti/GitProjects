// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using RestFoundation.Collections;
using RestFoundation.Results;
using RestFoundation.Runtime;

namespace RestFoundation
{
    /// <summary>
    /// Represents a base class for an HTTP handler implementation. HTTP handlers are designed for non-RESTful
    /// operations. This class cannot be instantiated.
    /// </summary>
    public abstract class HttpHandler : HttpTaskAsyncHandler, IRouteHandler
    {
        private const string FormDataMediaType = "application/x-www-form-urlencoded";

        private Lazy<NameValueCollection> m_paramsBuilder;

        /// <summary>
        /// Gets a combined collection of route, query string, form data and header values.
        /// </summary>
        /// <remarks>
        /// Form data is only populated if the content type is "application/x-www-form-urlencoded".
        /// </remarks>
        public NameValueCollection Params
        {
            get
            {
                return m_paramsBuilder != null ? m_paramsBuilder.Value : new NameValueCollection();
            }
        }

        /// <summary>
        /// When overridden in a derived class, provides code that performs the HTTP handler operations.
        /// </summary>
        /// <returns>The asynchronous task containing the <see cref="IResult"/> object.</returns>
        /// <param name="context">The service context.</param>
        public abstract Task<IResult> ExecuteAsync(IServiceContext context);

        /// <summary>
        /// When overridden in a derived class, provides code that handles an asynchronous task.
        /// </summary>
        /// <returns>
        /// The asynchronous task.
        /// </returns>
        /// <param name="context">The HTTP context.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed override async Task ProcessRequestAsync(HttpContext context)
        {
            var serviceContext = Rest.Configuration.ServiceLocator.GetService<IServiceContext>();

            m_paramsBuilder = new Lazy<NameValueCollection>(() => PopulateParams(serviceContext), true);

            IResult result = await ExecuteAsync(serviceContext);

            if (result == null)
            {
                return;
            }

            var resultExecutor = new ResultExecutor();
            await resultExecutor.Execute(result, serviceContext);
        }

        /// <summary>
        /// When overridden in a derived class, provides code that handles a synchronous task.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The method is implemented but does not provide any default handling for synchronous tasks.
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed override void ProcessRequest(HttpContext context)
        {
            base.ProcessRequest(context);
        }

        /// <summary>
        /// Provides the object that processes the request.
        /// </summary>
        /// <returns>An object that processes the request.</returns>
        /// <param name="requestContext">An object that encapsulates information about the request.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        private static NameValueCollection PopulateParams(IServiceContext context)
        {
            var values = new NameValueCollection();

            PopulateObjectValues(context.Request.RouteValues, values);
            PopulateStringValues(context.Request.QueryString, values);

            if (String.Equals(FormDataMediaType, context.Request.Headers.ContentType, StringComparison.OrdinalIgnoreCase))
            {
                PopulateStringValues(context.Request.Form, values);
            }

            PopulateStringValues(context.Request.Headers, values);

            return values;
        }

        private static void PopulateObjectValues(IRouteValueCollection source, NameValueCollection values)
        {
            if (source == null)
            {
                return;
            }

            foreach (string key in source.Keys)
            {
                var value = source.TryGet(key);

                if (value != null)
                {
                    values.Add(key, Convert.ToString(value, CultureInfo.InvariantCulture));
                }
            }
        }

        private static void PopulateStringValues(IStringValueCollection source, NameValueCollection values)
        {
            if (source == null)
            {
                return;
            }

            foreach (string key in source.Keys)
            {
                var value = source.TryGet(key);

                if (value != null)
                {
                    values.Add(key, value);
                }
            }
        }
    }
}
