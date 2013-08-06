// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.ServiceModel.Syndication;
using RestFoundation.Results;
using RestFoundation.Runtime;

namespace RestFoundation
{
    /// <summary>
    /// Generates results to return from service methods.
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// Gets a JSON format value.
        /// </summary>
        public const string JsonFormat = "json";

        /// <summary>
        /// Gets an XML format value.
        /// </summary>
        public const string XmlFormat = "xml";

        /// <summary>
        /// Gets a result with the status code set to Accepted (202).
        /// </summary>
        public static StatusCodeResult Accepted
        {
            get
            {
                var result = new StatusCodeResult
                             {
                                 Code = HttpStatusCode.Accepted,
                                 Description = "Accepted"
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to Bad Request (400).
        /// </summary>
        public static StatusCodeResult BadRequest
        {
            get
            {
                var result = new StatusCodeResult
                             {
                                 Code = HttpStatusCode.BadRequest,
                                 Description = "Bad Request"
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to Conflict (409).
        /// </summary>
        public static StatusCodeResult Conflict
        {
            get
            {
                var result = new StatusCodeResult
                             {
                                 Code = HttpStatusCode.Conflict,
                                 Description = "Conflict"
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to Forbidden (403).
        /// </summary>
        public static StatusCodeResult Forbidden
        {
            get
            {
                var result = new StatusCodeResult
                             {
                                 Code = HttpStatusCode.Forbidden,
                                 Description = "Forbidden"
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to No Content (204).
        /// </summary>
        public static StatusCodeResult NoContent
        {
            get
            {
                var result = new StatusCodeResult
                             {
                                 Code = HttpStatusCode.NoContent,
                                 Description = "No Content"
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to Not Found (404).
        /// </summary>
        public static StatusCodeResult NotFound
        {
            get
            {
                var result = new StatusCodeResult
                             {
                                 Code = HttpStatusCode.NotFound,
                                 Description = Resources.Global.NotFound
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to Not Implemented (501).
        /// </summary>
        public static StatusCodeResult NotImplemented
        {
            get
            {
                var result = new StatusCodeResult
                             {
                                 Code = HttpStatusCode.NotImplemented,
                                 Description = "Not Implemented"
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to OK (200).
        /// </summary>
        public static StatusCodeResult Ok
        {
            get
            {
                var result = new StatusCodeResult
                             {
                                 Code = HttpStatusCode.OK,
                                 Description = "OK"
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to Service Unavailable (503).
        /// </summary>
        public static StatusCodeResult ServiceUnavailable
        {
            get
            {
                var result = new StatusCodeResult
                             {
                                 Code = HttpStatusCode.ServiceUnavailable,
                                 Description = "Service Unavailable"
                             };

                return result;
            }
        }

        /// <summary>
        /// Returns a content result.
        /// </summary>
        /// <param name="content">The content string.</param>
        /// <returns>The content result.</returns>
        public static ContentResult Content(string content)
        {
            return Content(content, true, null);
        }

        /// <summary>
        /// Returns a content result.
        /// </summary>
        /// <param name="content">The content string.</param>
        /// <param name="clearOutput">A value indicating whether to clear output before sending the content.</param>
        /// <returns>The content result.</returns>
        public static ContentResult Content(string content, bool clearOutput)
        {
            return Content(content, clearOutput, null);
        }

        /// <summary>
        /// Returns a content result.
        /// </summary>
        /// <param name="content">The content string.</param>
        /// <param name="clearOutput">A value indicating whether to clear output before sending the content (true by default).</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>The content result.</returns>
        public static ContentResult Content(string content, bool clearOutput, string contentType)
        {
            var result = new ContentResult
                         {
                             Content = content,
                             ClearOutput = clearOutput,
                             ContentType = contentType
                         };

            return result;
        }

        /// <summary>
        /// Returns a local file result.
        /// </summary>
        /// <param name="filePath">The local file path.</param>
        /// <returns>The file path result.</returns>
        public static FileResult LocalFile(string filePath)
        {
            return LocalFile(filePath, null, null);
        }

        /// <summary>
        /// Returns a local file result.
        /// </summary>
        /// <param name="filePath">The local file path.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>The file path result.</returns>
        public static FileResult LocalFile(string filePath, string contentType)
        {
            return LocalFile(filePath, contentType, null);
        }

        /// <summary>
        /// Returns a local file result.
        /// </summary>
        /// <param name="filePath">The local file path.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="contentDisposition">The content disposition data.</param>
        /// <returns>The file path result.</returns>
        public static FileResult LocalFile(string filePath, string contentType, string contentDisposition)
        {
            var result = new FileResult
                         {
                             FilePath = filePath,
                             ContentType = contentType,
                             ContentDisposition = contentDisposition
                         };

            return result;
        }

        /// <summary>
        /// Returns an HTML file result.
        /// </summary>
        /// <param name="filePath">The local file path.</param>
        /// <returns>The HTML file result.</returns>
        public static HtmlFileResult LocalHtmlFile(string filePath)
        {
            var result = new HtmlFileResult
            {
                FilePath = filePath
            };

            return result;
        }

        /// <summary>
        /// Returns a redirect result for the provided URL with the redirect type <see cref="RedirectType.Found"/>.
        /// </summary>
        /// <param name="url">The URL to redirect to.</param>
        /// <returns>The redirect result.</returns>
        public static RedirectResult RedirectToUrl(string url)
        {
            return RedirectToUrl(url, RedirectType.Found);
        }

        /// <summary>
        /// Returns a redirect result for the provided URL with the provided redirect type.
        /// </summary>
        /// <param name="url">The URL to redirect to.</param>
        /// <param name="redirectType">The redirect type.</param>
        /// <returns>The redirect result.</returns>
        public static RedirectResult RedirectToUrl(string url, RedirectType redirectType)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            return new RedirectResult
            {
                RedirectUrl = url,
                RedirectType = redirectType
            };
        }

        /// <summary>
        /// Returns a redirect result for the service method with the redirect type <see cref="RedirectType.Found"/>.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP context cannot be found; an invalid service URL or a service method provided.
        /// </exception>
        public static RedirectResult RedirectToAction<TContract>(Expression<Action<TContract>> serviceMethod)
        {
            return RedirectToAction(null, serviceMethod, new RouteHash(), RedirectType.Found);
        }

        /// <summary>
        /// Returns a redirect result for the service method with the provided redirect type.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP context cannot be found; an invalid service URL or a service method provided.
        /// </exception>
        public static RedirectResult RedirectToAction<TContract>(Expression<Action<TContract>> serviceMethod, RouteHash routeValues)
        {
            return RedirectToAction(null, serviceMethod, routeValues, RedirectType.Found);
        }

        /// <summary>
        /// Returns a redirect result for the service method with the provided redirect type.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <param name="redirectType">The redirect type.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP context cannot be found; an invalid service URL or a service method provided.
        /// </exception>
        public static RedirectResult RedirectToAction<TContract>(Expression<Action<TContract>> serviceMethod, RouteHash routeValues, RedirectType redirectType)
        {
            return RedirectToAction(null, serviceMethod, routeValues, redirectType);
        }

        /// <summary>
        /// Returns a redirect result for the service method with the redirect type <see cref="RedirectType.Found"/>.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP context cannot be found; an invalid service URL or a service method provided.
        /// </exception>
        public static RedirectResult RedirectToAction<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod)
        {
            return RedirectToAction(serviceUrl, serviceMethod, new RouteHash(), RedirectType.Found);
        }

        /// <summary>
        /// Returns a redirect result for the service method with the redirect type <see cref="RedirectType.Found"/>.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="serviceMethod">The service method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP context cannot be found; an invalid service URL or a service method provided.
        /// </exception>
        public static RedirectResult RedirectToAction<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, RouteHash routeValues)
        {
            return RedirectToAction(serviceUrl, serviceMethod, routeValues, RedirectType.Found);
        }

        /// <summary>
        /// Returns a redirect result for the service method with the provided redirect type.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="serviceMethod">The service method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <param name="redirectType">The redirect type.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP context cannot be found; an invalid service URL or a service method provided.
        /// </exception>
        public static RedirectResult RedirectToAction<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, RouteHash routeValues, RedirectType redirectType)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            return new RedirectResult
            {
                RedirectUrl = GetServiceContext().GetUrl(serviceUrl, serviceMethod, routeValues),
                RedirectType = redirectType
            };
        }

        /// <summary>
        /// Returns a redirect result for the service method with the redirect type <see cref="RedirectType.Found"/>.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP context cannot be found; an invalid service URL or a service method provided.
        /// </exception>
        public static RedirectResult RedirectToAction<TContract>(Expression<Func<TContract, object>> serviceMethod)
        {
            return RedirectToAction(null, serviceMethod, new RouteHash(), RedirectType.Found);
        }

        /// <summary>
        /// Returns a redirect result for the service method with the redirect type <see cref="RedirectType.Found"/>.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP context cannot be found; an invalid service URL or a service method provided.
        /// </exception>
        public static RedirectResult RedirectToAction<TContract>(Expression<Func<TContract, object>> serviceMethod, RouteHash routeValues)
        {
            return RedirectToAction(null, serviceMethod, routeValues, RedirectType.Found);
        }

        /// <summary>
        /// Returns a redirect result for the service method with the redirect type <see cref="RedirectType.Found"/>.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceMethod">The service method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <param name="redirectType">The redirect type.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP context cannot be found; an invalid service URL or a service method provided.
        /// </exception>
        public static RedirectResult RedirectToAction<TContract>(Expression<Func<TContract, object>> serviceMethod, RouteHash routeValues, RedirectType redirectType)
        {
            return RedirectToAction(null, serviceMethod, routeValues, redirectType);
        }

        /// <summary>
        /// Returns a redirect result for the service method with the redirect type <see cref="RedirectType.Found"/>.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP context cannot be found; an invalid service URL or a service method provided.
        /// </exception>
        public static RedirectResult RedirectToAction<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod)
        {
            return RedirectToAction(serviceUrl, serviceMethod, new RouteHash(), RedirectType.Found);
        }

        /// <summary>
        /// Returns a redirect result for the service method with the redirect type <see cref="RedirectType.Found"/>.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="serviceMethod">The service method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP context cannot be found; an invalid service URL or a service method provided.
        /// </exception>
        public static RedirectResult RedirectToAction<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, RouteHash routeValues)
        {
            return RedirectToAction(serviceUrl, serviceMethod, routeValues, RedirectType.Found);
        }

        /// <summary>
        /// Returns a redirect result for the service method with the provided redirect type.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="serviceMethod">The service method.</param>
        /// <param name="routeValues">Additional route values.</param>
        /// <param name="redirectType">The redirect type.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the HTTP context cannot be found; an invalid service URL or a service method provided.
        /// </exception>
        public static RedirectResult RedirectToAction<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, RouteHash routeValues, RedirectType redirectType)
        {
            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            return new RedirectResult
            {
                RedirectUrl = GetServiceContext().GetUrl(serviceUrl, serviceMethod, routeValues),
                RedirectType = redirectType
            };
        }

        /// <summary>
        /// Returns a remote file result.
        /// </summary>
        /// <param name="fileUrl">The remote file URL.</param>
        /// <returns>The file URL result.</returns>
        public static FileResult RemoteFile(string fileUrl)
        {
            return RemoteFile(fileUrl, null, null);
        }

        /// <summary>
        /// Returns a remote file result.
        /// </summary>
        /// <param name="fileUrl">The remote file URL.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>The file URL result.</returns>
        public static FileResult RemoteFile(string fileUrl, string contentType)
        {
            return RemoteFile(fileUrl, contentType, null);
        }

        /// <summary>
        /// Returns a remote file result.
        /// </summary>
        /// <param name="fileUrl">The remote file URL.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="contentDisposition">The content disposition data.</param>
        /// <returns>The file URL result.</returns>
        public static FileResult RemoteFile(string fileUrl, string contentType, string contentDisposition)
        {
            if (String.IsNullOrEmpty(fileUrl))
            {
                throw new ArgumentNullException("fileUrl");
            }

            var result = new FileResult
                         {
                             FilePath = GetServiceContext().MapPath(fileUrl),
                             ContentType = contentType,
                             ContentDisposition = contentDisposition
                         };

            return result;
        }

        /// <summary>
        /// Returns an HTML file result.
        /// </summary>
        /// <param name="fileUrl">The remote file URL.</param>
        /// <returns>The HTML file result.</returns>
        public static HtmlFileResult RemoteHtmlFile(string fileUrl)
        {
            return LocalHtmlFile(!String.IsNullOrEmpty(fileUrl) ? GetServiceContext().MapPath(fileUrl) : null);
        }

        /// <summary>
        /// Returns the response status result.
        /// </summary>
        /// <param name="code">The HTTP status code.</param>
        /// <returns>The response status result.</returns>
        public static StatusCodeResult ResponseStatus(HttpStatusCode code)
        {
            return ResponseStatus(code, null, null);
        }

        /// <summary>
        /// Returns the response status result.
        /// </summary>
        /// <param name="code">The HTTP status code.</param>
        /// <param name="description">The HTTP status description.</param>
        /// <returns>The response status result.</returns>
        public static StatusCodeResult ResponseStatus(HttpStatusCode code, string description)
        {
            return ResponseStatus(code, description, null);
        }

        /// <summary>
        /// Returns the response status result.
        /// </summary>
        /// <param name="code">The HTTP status code.</param>
        /// <param name="description">The HTTP status description.</param>
        /// <param name="responseHeaders">A dictionary of response headers.</param>
        /// <returns>The response status result.</returns>
        public static StatusCodeResult ResponseStatus(HttpStatusCode code, string description, IDictionary<string, string> responseHeaders)
        {
            var result = new StatusCodeResult
                         {
                             Code = code,
                             Description = description
                         };

            if (responseHeaders != null)
            {
                foreach (var header in responseHeaders)
                {
                    result.ResponseHeaders[header.Key] = header.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the provided object and sets the provided response status code.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object to return.</param>
        /// <param name="code">The HTTP status code.</param>
        /// <returns>The object.</returns>
        public static T ObjectWithResponseStatus<T>(T obj, HttpStatusCode code)
        {
            return ObjectWithResponseStatus(obj, code, null, null);
        }

        /// <summary>
        /// Returns the provided object and sets the provided response status code and description.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object to return.</param>
        /// <param name="code">The HTTP status code.</param>
        /// <param name="description">The HTTP status description.</param>
        /// <returns>The object.</returns>
        public static T ObjectWithResponseStatus<T>(T obj, HttpStatusCode code, string description)
        {
            return ObjectWithResponseStatus(obj, code, description, null);
        }

        /// <summary>
        /// Returns the provided object and sets the provided response status code and description.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object to return.</param>
        /// <param name="code">The HTTP status code.</param>
        /// <param name="description">The HTTP status description.</param>
        /// <param name="responseHeaders">A dictionary of response headers.</param>
        /// <returns>The object.</returns>
        public static T ObjectWithResponseStatus<T>(T obj, HttpStatusCode code, string description, IDictionary<string, string> responseHeaders)
        {
            IServiceContext context = GetServiceContext();

            context.Response.SetStatus(code, description ?? String.Empty);

            if (responseHeaders != null)
            {
                foreach (var header in responseHeaders)
                {
                    context.Response.SetHeader(header.Key, header.Value);
                }
            }

            return obj;
        }

        /// <summary>
        /// Returns an ATOM or RSS feed result.
        /// </summary>
        /// <param name="feed">The feed.</param>
        /// <param name="format">The feed format.</param>
        /// <returns>The feed result.</returns>
        public static FeedResult Feed(SyndicationFeed feed, FeedResult.SyndicationFormat format)
        {
            var result = new FeedResult
                         {
                             Feed = feed,
                             Format = format
                         };

            return result;
        }

        /// <summary>
        /// Returns a JSON result.
        /// </summary>
        /// <param name="obj">The object to serialize to JSON.</param>
        /// <returns>The JSON result.</returns>
        public static JsonResult Json(object obj)
        {
            return new JsonResult
            {
                Content = obj
            };
        }

        /// <summary>
        /// Returns a JSONP result inside a callback function with the "jsonpCallback" name.
        /// </summary>
        /// <param name="obj">The object to serialize to JSONP.</param>
        /// <returns>The JSONP result.</returns>
        public static JsonPResult JsonP(object obj)
        {
            return JsonP(obj, null);
        }

        /// <summary>
        /// Returns a JSONP result inside a callback function with the provided name.
        /// </summary>
        /// <param name="obj">The object to serialize to JSONP.</param>
        /// <param name="callback">The callback function name.</param>
        /// <returns>The JSONP result.</returns>
        public static JsonPResult JsonP(object obj, string callback)
        {
            var result = new JsonPResult
            {
                Content = obj,
                Callback = callback,
            };

            return result;
        }

        /// <summary>
        /// Returns a XML result.
        /// </summary>
        /// <param name="obj">The object to serialize to XML.</param>
        /// <returns>The XML result.</returns>
        public static XmlResult Xml(object obj)
        {
            return new XmlResult
            {
                Content = obj
            };
        }

        /// <summary>
        /// Returns a JSON or an XML result based on the <paramref name="format"/> value.
        /// Format value must be equal to <see cref="JsonFormat"/> or <see cref="XmlFormat"/>.
        /// </summary>
        /// <param name="obj">The object to serialize to JSON or XML.</param>
        /// <param name="format">The result content format ("JSON" or "XML").</param>
        /// <returns>The JSON or XML result.</returns>
        public static IResult JsonOrXml(object obj, string format)
        {
            if (format == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, Resources.Global.InvalidResultContentFormat);
            }

            switch (format.ToLowerInvariant().Trim())
            {
                case JsonFormat:
                    return new JsonResult
                    {
                        Content = obj
                    };
                case XmlFormat:
                    return new XmlResult
                    {
                        Content = obj
                    };
                default:
                    throw new HttpResponseException(HttpStatusCode.BadRequest, Resources.Global.InvalidResultContentFormat);
            }
        }

        private static IServiceContext GetServiceContext()
        {
            var context = Rest.Configuration.ServiceLocator.GetService<IServiceContext>();

            if (context == null)
            {
                throw new InvalidOperationException(Resources.Global.MissingHttpContext);
            }

            return context;
        }
    }
}
