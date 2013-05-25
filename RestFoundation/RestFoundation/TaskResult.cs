// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using RestFoundation.Context;
using RestFoundation.Results;

namespace RestFoundation
{
    /// <summary>
    /// Generates results wrapped in a <see cref="Task{T}"/> to return from service methods.
    /// </summary>
    public static class TaskResult
    {
        /// <summary>
        /// Gets a result task with the status code set to Accepted (202).
        /// </summary>
        public static Task<IResult> Accepted
        {
            get
            {
                return Task.FromResult<IResult>(Result.Accepted);
            }
        }

        /// <summary>
        /// Gets a result task with the status code set to Bad Request (400).
        /// </summary>
        public static Task<IResult> BadRequest
        {
            get
            {
                return Task.FromResult<IResult>(Result.BadRequest);
            }
        }

        /// <summary>
        /// Gets a result task with the status code set to Conflict (409).
        /// </summary>
        public static Task<IResult> Conflict
        {
            get
            {
                return Task.FromResult<IResult>(Result.Conflict);
            }
        }

        /// <summary>
        /// Gets a result task with the status code set to Forbidden (403).
        /// </summary>
        public static Task<IResult> Forbidden
        {
            get
            {
                return Task.FromResult<IResult>(Result.Forbidden);
            }
        }

        /// <summary>
        /// Gets a result task with the status code set to No Content (204).
        /// </summary>
        public static Task<IResult> NoContent
        {
            get
            {
                return Task.FromResult<IResult>(Result.NoContent);
            }
        }

        /// <summary>
        /// Gets a result task with the status code set to Not Found (404).
        /// </summary>
        public static Task<IResult> NotFound
        {
            get
            {
                return Task.FromResult<IResult>(Result.NotFound);
            }
        }

        /// <summary>
        /// Gets a result task with the status code set to Not Implemented (501).
        /// </summary>
        public static Task<IResult> NotImplemented
        {
            get
            {
                return Task.FromResult<IResult>(Result.NotImplemented);
            }
        }

        /// <summary>
        /// Gets a result task with the status code set to OK (200).
        /// </summary>
        public static Task<IResult> Ok
        {
            get
            {
                return Task.FromResult<IResult>(Result.Ok);
            }
        }

        /// <summary>
        /// Returns a content result task.
        /// </summary>
        /// <param name="content">The content string.</param>
        /// <returns>The content result task.</returns>
        public static Task<IResult> Content(string content)
        {
            return Task.FromResult<IResult>(Result.Content(content));
        }

        /// <summary>
        /// Returns a content result task.
        /// </summary>
        /// <param name="content">The content string.</param>
        /// <param name="clearOutput">A value indicating whether to clear output before sending the content.</param>
        /// <returns>The content result task.</returns>
        public static Task<IResult> Content(string content, bool clearOutput)
        {
            return Task.FromResult<IResult>(Result.Content(content, clearOutput));
        }

        /// <summary>
        /// Returns a content result task.
        /// </summary>
        /// <param name="content">The content string.</param>
        /// <param name="clearOutput">A value indicating whether to clear output before sending the content (true by default).</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>The content result task.</returns>
        public static Task<IResult> Content(string content, bool clearOutput, string contentType)
        {
            return Task.FromResult<IResult>(Result.Content(content, clearOutput, contentType));
        }

        /// <summary>
        /// Returns a local file result task.
        /// </summary>
        /// <param name="filePath">The local file path.</param>
        /// <returns>The file path result task.</returns>
        public static Task<IResult> LocalFile(string filePath)
        {
            return Task.FromResult<IResult>(Result.LocalFile(filePath));
        }

        /// <summary>
        /// Returns a local file result task.
        /// </summary>
        /// <param name="filePath">The local file path.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>The file path result task.</returns>
        public static Task<IResult> LocalFile(string filePath, string contentType)
        {
            return Task.FromResult<IResult>(Result.LocalFile(filePath, contentType));
        }

        /// <summary>
        /// Returns a local file result task.
        /// </summary>
        /// <param name="filePath">The local file path.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="contentDisposition">The content disposition data.</param>
        /// <returns>The file path result task.</returns>
        public static Task<IResult> LocalFile(string filePath, string contentType, string contentDisposition)
        {
            return Task.FromResult<IResult>(Result.LocalFile(filePath, contentType, contentDisposition));
        }

        /// <summary>
        /// Returns a redirect result task for the provided URL with the redirect type <see cref="RedirectType.Found"/>.
        /// </summary>
        /// <param name="url">The URL to redirect to.</param>
        /// <returns>The redirect result task.</returns>
        public static Task<IResult> RedirectToUrl(string url)
        {
            return Task.FromResult<IResult>(Result.RedirectToUrl(url));
        }

        /// <summary>
        /// Returns a redirect result task for the provided URL with the provided redirect type.
        /// </summary>
        /// <param name="url">The URL to redirect to.</param>
        /// <param name="redirectType">The redirect type.</param>
        /// <returns>The redirect result task.</returns>
        public static Task<IResult> RedirectToUrl(string url, RedirectType redirectType)
        {
            return Task.FromResult<IResult>(Result.RedirectToUrl(url, redirectType));
        }

        /// <summary>
        /// Returns a remote file result task.
        /// </summary>
        /// <param name="fileUrl">The remote file URL.</param>
        /// <returns>The file URL result task.</returns>
        public static Task<IResult> RemoteFile(string fileUrl)
        {
            return Task.FromResult<IResult>(Result.RemoteFile(fileUrl));
        }

        /// <summary>
        /// Returns a remote file result task.
        /// </summary>
        /// <param name="fileUrl">The remote file URL.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>The file URL result task.</returns>
        public static Task<IResult> RemoteFile(string fileUrl, string contentType)
        {
            return Task.FromResult<IResult>(Result.RemoteFile(fileUrl, contentType));
        }

        /// <summary>
        /// Returns a remote file result task.
        /// </summary>
        /// <param name="fileUrl">The remote file URL.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="contentDisposition">The content disposition data.</param>
        /// <returns>The file URL result task.</returns>
        public static Task<IResult> RemoteFile(string fileUrl, string contentType, string contentDisposition)
        {
            return Task.FromResult<IResult>(Result.RemoteFile(fileUrl, contentType, contentDisposition));
        }

        /// <summary>
        /// Returns the response status result task.
        /// </summary>
        /// <param name="code">The HTTP status code.</param>
        /// <returns>The response status result task.</returns>
        public static Task<IResult> ResponseStatus(HttpStatusCode code)
        {
            return Task.FromResult<IResult>(Result.ResponseStatus(code));
        }

        /// <summary>
        /// Returns the response status result task.
        /// </summary>
        /// <param name="code">The HTTP status code.</param>
        /// <param name="description">The HTTP status description.</param>
        /// <returns>The response status result task.</returns>
        public static Task<IResult> ResponseStatus(HttpStatusCode code, string description)
        {
            return Task.FromResult<IResult>(Result.ResponseStatus(code, description));
        }

        /// <summary>
        /// Returns the response status result task.
        /// </summary>
        /// <param name="code">The HTTP status code.</param>
        /// <param name="description">The HTTP status description.</param>
        /// <param name="responseHeaders">A dictionary of response headers.</param>
        /// <returns>The response status result task.</returns>
        public static Task<IResult> ResponseStatus(HttpStatusCode code, string description, IDictionary<string, string> responseHeaders)
        {
            return Task.FromResult<IResult>(Result.ResponseStatus(code, description, responseHeaders));
        }

        /// <summary>
        /// Returns an ATOM or RSS feed result task.
        /// </summary>
        /// <param name="feed">The feed.</param>
        /// <param name="format">The feed format.</param>
        /// <returns>The feed result task.</returns>
        public static Task<IResult> Feed(SyndicationFeed feed, FeedResult.SyndicationFormat format)
        {
            return Task.FromResult<IResult>(Result.Feed(feed, format));
        }

        /// <summary>
        /// Returns a JSON result task.
        /// </summary>
        /// <param name="obj">The object to serialize to JSON.</param>
        /// <returns>The JSON result task.</returns>
        public static Task<IResult> Json(object obj)
        {
            return Task.FromResult<IResult>(Result.Json(obj));
        }

        /// <summary>
        /// Returns a JSONP result task inside a callback function with the "jsonpCallback" name.
        /// </summary>
        /// <param name="obj">The object to serialize to JSONP.</param>
        /// <returns>The JSONP result task.</returns>
        public static Task<IResult> JsonP(object obj)
        {
            return Task.FromResult<IResult>(Result.JsonP(obj));
        }

        /// <summary>
        /// Returns a JSONP result task inside a callback function with the provided name.
        /// </summary>
        /// <param name="obj">The object to serialize to JSONP.</param>
        /// <param name="callback">The callback function name.</param>
        /// <returns>The JSONP result task.</returns>
        public static Task<IResult> JsonP(object obj, string callback)
        {
            return Task.FromResult<IResult>(Result.JsonP(obj, callback));
        }

        /// <summary>
        /// Returns a XML result task.
        /// </summary>
        /// <param name="obj">The object to serialize to XML.</param>
        /// <returns>The XML result task.</returns>
        public static Task<IResult> Xml(object obj)
        {
            return Task.FromResult<IResult>(Result.Xml(obj));
        }

        /// <summary>
        /// Returns a JSON or an XML result task based on the <paramref name="format"/> value.
        /// Format value must be equal to <see cref="Result.JsonFormat"/> or <see cref="Result.XmlFormat"/>.
        /// </summary>
        /// <param name="obj">The object to serialize to JSON or XML.</param>
        /// <param name="format">The result content format ("JSON" or "XML").</param>
        /// <returns>The JSON or XML result task.</returns>
        public static Task<IResult> JsonOrXml(object obj, string format)
        {
            return Task.FromResult(Result.JsonOrXml(obj, format));
        }

        /// <summary>
        /// Creates and starts a new task returning an <see cref="IResult"/>.
        /// </summary>
        /// <param name="action">The action delegate that creates an <see cref="IResult"/>.</param>
        /// <returns>The created result task.</returns>
        public static Task<IResult> Start(Func<IResult> action)
        {
            var context = Rest.Configuration.ServiceLocator.GetService<IServiceContext>();

            if (context == null)
            {
                throw new InvalidOperationException(Resources.Global.MissingHttpContext);
            }

            return Task.Factory.StartNew(action, context.Response.GetCancellationToken());
        }

        /// <summary>
        /// Creates and starts a new task returning an <see cref="IResult"/>.
        /// </summary>
        /// <param name="action">The action delegate that creates an <see cref="IResult"/>.</param>
        /// <param name="cancellationToken">The cancellation token for the task.</param>
        /// <returns>The created result task.</returns>
        public static Task<IResult> Start(Func<IResult> action, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(action, cancellationToken);
        }

        /// <summary>
        /// Returns a result task wrapped in a task.
        /// </summary>
        /// <param name="result">The <see cref="IResult"/> to wrap.</param>
        /// <returns>The result task.</returns>
        public static Task<IResult> Wrap(IResult result)
        {
            return Task.FromResult(result);
        }
    }
}
