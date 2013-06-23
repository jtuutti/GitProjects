// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Specialized;
using System.Text;

namespace RestFoundation.UnitTesting
{
    /// <summary>
    /// Represents a mock service context manager.
    /// </summary>
    public static class MockContextManager
    {
        /// <summary>
        /// Gets the application root virtual path.
        /// </summary>
        public const string RootVirtualPath = "~/";

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
                throw new InvalidOperationException(Resources.Global.AlreadyInitializedHttpContext);
            }

            TestHttpContext.Context = new TestHttpContext(virtualPath, method.ToString().ToUpperInvariant());

            return Rest.Configuration.ServiceLocator.GetService<IServiceContext>();
        }

        /// <summary>
        /// Creates the current HTTP context with the provided virtual path and the HTTP method.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="method">The HTTP method.</param>
        /// <param name="headers">A collection of headers to initialize the context with.</param>
        /// <returns>The service context.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the current HTTP context has already been initialized.
        /// </exception>
        public static IServiceContext GenerateContext(string virtualPath, HttpMethod method, NameValueCollection headers)
        {
            if (TestHttpContext.Context != null)
            {
                throw new InvalidOperationException(Resources.Global.AlreadyInitializedHttpContext);
            }

            TestHttpContext.Context = new TestHttpContext(virtualPath, method.ToString().ToUpperInvariant());

            if (headers != null)
            {
                TestHttpContext.Context.Request.Headers.Add(headers);
            }

            return Rest.Configuration.ServiceLocator.GetService<IServiceContext>();
        }

        /// <summary>
        /// Destroys the current HTTP context.
        /// </summary>
        public static void DestroyContext()
        {
            TestHttpContext.Context = null;
        }

        /// <summary>
        /// Sets the provided accept media types to the generated HTTP context.
        /// </summary>
        /// <param name="acceptTypes">The accepted media types.</param>
        public static void SetAcceptTypes(string[] acceptTypes)
        {
            if (acceptTypes == null)
            {
                throw new ArgumentNullException("acceptTypes");
            }

            ValidateContext();

            ((TestHttpRequest) TestHttpContext.Context.Request).SetAcceptTypes(acceptTypes);

            if (acceptTypes.Length > 0)
            {
                TestHttpContext.Context.Request.Headers["Accept"] = String.Join(",", acceptTypes);
            }
        }

        /// <summary>
        /// Sets the provided content type to the generated HTTP context.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        public static void SetContentType(string contentType)
        {
            if (String.IsNullOrEmpty(contentType))
            {
                throw new ArgumentNullException("contentType");
            }

            ValidateContext();

            TestHttpContext.Context.Request.ContentType = contentType;
            TestHttpContext.Context.Request.Headers["Content-Type"] = contentType;
        }

        /// <summary>
        /// Sets the provided content encoding to the generated HTTP context.
        /// </summary>
        /// <param name="contentEncoding">The content encoding.</param>
        public static void SetContentEncoding(Encoding contentEncoding)
        {
            if (contentEncoding == null)
            {
                throw new ArgumentNullException("contentEncoding");
            }

            ValidateContext();

            TestHttpContext.Context.Request.ContentEncoding = contentEncoding;
            TestHttpContext.Context.Request.Headers["Content-Language"] = contentEncoding.WebName;
        }

        /// <summary>
        /// Sets a header with the provided name and value to the generated HTTP context.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        public static void SetHeader(string name, string value)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            ValidateContext();

            TestHttpContext.Context.Request.Headers.Add(name, value);
        }

        /// <summary>
        /// Sets the provided headers to the generated HTTP context.
        /// </summary>
        /// <param name="headers">The headers.</param>
        public static void SetHeaders(NameValueCollection headers)
        {
            if (headers == null)
            {
                return;
            }

            ValidateContext();

            TestHttpContext.Context.Request.Headers.Add(headers);
        }

        /// <summary>
        /// Sets a form data item with the provided name and value to the generated HTTP context.
        /// </summary>
        /// <param name="name">The form data item name.</param>
        /// <param name="value">The form data item value.</param>
        public static void SetFormData(string name, string value)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            ValidateContext();

            TestHttpContext.Context.Request.Form.Add(name, value);
        }

        /// <summary>
        /// Sets the provided form data collection to the generated HTTP context.
        /// </summary>
        /// <param name="formData">The form data collection.</param>
        public static void SetFormData(NameValueCollection formData)
        {
            if (formData == null)
            {
                return;
            }

            ValidateContext();

            TestHttpContext.Context.Request.Form.Add(formData);
        }

        /// <summary>
        /// Sets a query string item with the provided name and value to the generated HTTP context.
        /// </summary>
        /// <param name="name">The query string name.</param>
        /// <param name="value">The query string value.</param>
        public static void SetQuery(string name, string value)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            ValidateContext();

            TestHttpContext.Context.Request.QueryString.Add(name, value);
        }

        /// <summary>
        /// Sets the provided query string to the generated HTTP context.
        /// </summary>
        /// <param name="queryString">The query string keys and values.</param>
        public static void SetQuery(NameValueCollection queryString)
        {
            if (queryString == null)
            {
                return;
            }

            ValidateContext();

            TestHttpContext.Context.Request.QueryString.Add(queryString);
        }

        /// <summary>
        /// Sets a server variable with the provided name and value to the generated HTTP context.
        /// </summary>
        /// <param name="name">The server variable name.</param>
        /// <param name="value">The server variable value.</param>
        public static void SetServerVariable(string name, string value)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            ValidateContext();

            TestHttpContext.Context.Request.ServerVariables.Add(name, value);
        }

        /// <summary>
        /// Sets the provided server variable to the generated HTTP context.
        /// </summary>
        /// <param name="queryString">The server variable keys and values.</param>
        public static void SetServerVariables(NameValueCollection queryString)
        {
            if (queryString == null)
            {
                return;
            }

            ValidateContext();

            TestHttpContext.Context.Request.ServerVariables.Add(queryString);
        }

        private static void ValidateContext()
        {
            if (TestHttpContext.Context == null)
            {
                throw new InvalidOperationException(Resources.Global.MissingHttpContext);
            }
        }
    }
}
