﻿// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.ServiceModel.Syndication;
using System.Web.Routing;
using RestFoundation.Results;

namespace RestFoundation
{
    /// <summary>
    /// Represents results to return from service methods.
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// Gets a result with the status code set to Accepted (202).
        /// </summary>
        public static StatusResult Accepted
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.Accepted,
                                 StatusDescription = "Accepted"
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to Bad Request (400).
        /// </summary>
        public static StatusResult BadRequest
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.BadRequest,
                                 StatusDescription = "Bad Request"
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to Conflict (409).
        /// </summary>
        public static StatusResult Conflict
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.Conflict,
                                 StatusDescription = "Conflict"
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to Forbidden (403).
        /// </summary>
        public static StatusResult Forbidden
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.Forbidden,
                                 StatusDescription = "Forbidden"
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to No Content (204).
        /// </summary>
        public static StatusResult NoContent
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.NoContent,
                                 StatusDescription = "No Content"
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to Not Found (404).
        /// </summary>
        public static StatusResult NotFound
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.NotFound,
                                 StatusDescription = RestResources.NotFound
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to Not Implemented (501).
        /// </summary>
        public static StatusResult NotImplemented
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.NotImplemented,
                                 StatusDescription = "Not Implemented"
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to OK (200).
        /// </summary>
        public static StatusResult Ok
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.OK,
                                 StatusDescription = "OK"
                             };

                return result;
            }
        }

        /// <summary>
        /// Gets a result with the status code set to Service Unavailable (503).
        /// </summary>
        public static StatusResult ServiceUnavailable
        {
            get
            {
                var result = new StatusResult
                             {
                                 StatusCode = HttpStatusCode.ServiceUnavailable,
                                 StatusDescription = "Service Unavailable"
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
        public static FilePathResult LocalFile(string filePath)
        {
            return LocalFile(filePath, null, null);
        }

        /// <summary>
        /// Returns a local file result.
        /// </summary>
        /// <param name="filePath">The local file path.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>The file path result.</returns>
        public static FilePathResult LocalFile(string filePath, string contentType)
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
        public static FilePathResult LocalFile(string filePath, string contentType, string contentDisposition)
        {
            var result = new FilePathResult
                         {
                             FilePath = filePath,
                             ContentType = contentType,
                             ContentDisposition = contentDisposition
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
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">If the HTTP context cannot be found.</exception>
        public static RedirectResult RedirectToAction<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod)
        {
            return RedirectToAction(serviceUrl, serviceMethod, new RouteValueDictionary(), RedirectType.Found);
        }

        /// <summary>
        /// Returns a redirect result for the service method with the provided redirect type.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="serviceMethod">The service method.</param>
        /// <param name="redirectType">The redirect type.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">If the HTTP context cannot be found.</exception>
        public static RedirectResult RedirectToAction<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, RedirectType redirectType)
        {
            return RedirectToAction(serviceUrl, serviceMethod, new RouteValueDictionary(), redirectType);
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
        /// <exception cref="InvalidOperationException">If the HTTP context cannot be found.</exception>
        public static RedirectResult RedirectToAction<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, object routeValues, RedirectType redirectType)
        {
            return RedirectToAction(serviceUrl, serviceMethod, new RouteValueDictionary(routeValues), redirectType);
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
        /// <exception cref="InvalidOperationException">If the HTTP context cannot be found.</exception>
        public static RedirectResult RedirectToAction<TContract>(string serviceUrl, Expression<Action<TContract>> serviceMethod, RouteValueDictionary routeValues, RedirectType redirectType)
        {
            if (String.IsNullOrEmpty(serviceUrl))
            {
                throw new ArgumentNullException("serviceUrl");
            }

            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            var context = Rest.Configuration.ServiceLocator.GetService<IServiceContext>();

            if (context == null)
            {
                throw new InvalidOperationException(RestResources.MissingHttpContext);
            }

            return new RedirectResult
            {
                RedirectUrl = context.GetPath(serviceUrl, serviceMethod, routeValues),
                RedirectType = redirectType
            };
        }

        /// <summary>
        /// Returns a redirect result for the service method with the redirect type <see cref="RedirectType.Found"/>.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="serviceMethod">The service method.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">If the HTTP context cannot be found.</exception>
        public static RedirectResult RedirectToAction<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod)
        {
            return RedirectToAction(serviceUrl, serviceMethod, new RouteValueDictionary(), RedirectType.Found);
        }

        /// <summary>
        /// Returns a redirect result for the service method with the provided redirect type.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="serviceMethod">The service method.</param>
        /// <param name="redirectType">The redirect type.</param>
        /// <returns>The redirect result.</returns>
        /// <exception cref="InvalidOperationException">If the HTTP context cannot be found.</exception>
        public static RedirectResult RedirectToAction<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, RedirectType redirectType)
        {
            return RedirectToAction(serviceUrl, serviceMethod, new RouteValueDictionary(), redirectType);
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
        /// <exception cref="InvalidOperationException">If the HTTP context cannot be found.</exception>
        public static RedirectResult RedirectToAction<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, object routeValues, RedirectType redirectType)
        {
            return RedirectToAction(serviceUrl, serviceMethod, new RouteValueDictionary(routeValues), redirectType);
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
        /// <exception cref="InvalidOperationException">If the HTTP context cannot be found.</exception>
        public static RedirectResult RedirectToAction<TContract>(string serviceUrl, Expression<Func<TContract, object>> serviceMethod, RouteValueDictionary routeValues, RedirectType redirectType)
        {
            if (String.IsNullOrEmpty(serviceUrl))
            {
                throw new ArgumentNullException("serviceUrl");
            }

            if (serviceMethod == null)
            {
                throw new ArgumentNullException("serviceMethod");
            }

            var context = Rest.Configuration.ServiceLocator.GetService<IServiceContext>();

            if (context == null)
            {
                throw new InvalidOperationException(RestResources.MissingHttpContext);
            }

            return new RedirectResult
            {
                RedirectUrl = context.GetPath(serviceUrl, serviceMethod, routeValues),
                RedirectType = redirectType
            };
        }

        /// <summary>
        /// Returns a remote file result.
        /// </summary>
        /// <param name="fileUrl">The remote file URL.</param>
        /// <returns>The file URL result.</returns>
        public static FileUrlResult RemoteFile(string fileUrl)
        {
            return RemoteFile(fileUrl, null, null);
        }

        /// <summary>
        /// Returns a remote file result.
        /// </summary>
        /// <param name="fileUrl">The remote file URL.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>The file URL result.</returns>
        public static FileUrlResult RemoteFile(string fileUrl, string contentType)
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
        public static FileUrlResult RemoteFile(string fileUrl, string contentType, string contentDisposition)
        {
            var result = new FileUrlResult
                         {
                             FileUrl = fileUrl,
                             ContentType = contentType,
                             ContentDisposition = contentDisposition
                         };

            return result;
        }

        /// <summary>
        /// Returns the response status result.
        /// </summary>
        /// <param name="code">The HTTP status code.</param>
        /// <returns>The response status result</returns>
        public static StatusResult ResponseStatus(HttpStatusCode code)
        {
            return ResponseStatus(code, String.Empty, null);
        }

        /// <summary>
        /// Returns the response status result.
        /// </summary>
        /// <param name="code">The HTTP status code.</param>
        /// <param name="description">The HTTP status description.</param>
        /// <returns>The response status result</returns>
        public static StatusResult ResponseStatus(HttpStatusCode code, string description)
        {
            return ResponseStatus(code, description, null);
        }

        /// <summary>
        /// Returns the response status result.
        /// </summary>
        /// <param name="code">The HTTP status code.</param>
        /// <param name="description">The HTTP status description.</param>
        /// <param name="responseHeaders">A dictionary of response headers.</param>
        /// <returns>The response status result</returns>
        public static StatusResult ResponseStatus(HttpStatusCode code, string description, IDictionary<string, string> responseHeaders)
        {
            var result = new StatusResult
                         {
                             StatusCode = code,
                             StatusDescription = description
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
                Callback = callback
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
    }
}
