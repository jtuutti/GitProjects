﻿// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a JSONP result.
    /// </summary>
    public class JsonPResult : IResult
    {
        private static readonly Regex methodNamePattern = new Regex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Gets or sets the object to serialize to JSONP/JavaScript function.
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// Gets or sets the content type. The "application/javascript" content type is used by default.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the callback function name. The "jsonpCallback" name is used
        /// if no value is set to this property.
        /// </summary>
        public string Callback { get; set; }

        /// <summary>
        /// Executes the result against the provided service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        public virtual void Execute(IServiceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (String.IsNullOrEmpty(Callback))
            {
                Callback = "jsonpCallback";
            }
            else if (!methodNamePattern.IsMatch(Callback))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, RestResources.InvalidJsonPCallback);
            }

            if (String.IsNullOrEmpty(ContentType))
            {
                ContentType = "application/javascript";
            }

            context.Response.Output.Clear();
            context.Response.SetHeader(context.Response.HeaderNames.ContentType, ContentType);
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

            string response = new StringBuilder().Append(Callback)
                                                 .Append("(")
                                                 .Append(Content != null ? JsonConvert.SerializeObject(Content, Rest.Configuration.Options.JsonSettings.ToJsonSerializerSettings()) : "null")
                                                 .Append(");")
                                                 .ToString();

            context.Response.Output.Write(response);

            LogResponse(response);
        }

        private void LogResponse(string response)
        {
            if (LogUtility.CanLog)
            {
                LogUtility.LogResponseBody(response, ContentType);
            }
        }
    }
}
