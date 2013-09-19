// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestFoundation.Resources;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a JSON result.
    /// </summary>
    public class JsonResult : IResult
    {
        private static readonly Regex methodNamePattern = new Regex("^[A-Za-z$_][A-Za-z0-9$_]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResult"/> class.
        /// </summary>
        public JsonResult()
        {
            WrapContent = Rest.Configuration.Options.JsonSettings.WrapContentResponse;
        }

        /// <summary>
        /// Gets or sets the object to serialize to JSON.
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// Gets or sets the content type. The "application/json" content type is used by default.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the JSONP callback function name. JSON is returned if this property is
        /// set to null.
        /// </summary>
        public string Callback { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to wrap the content.
        /// </summary>
        public bool WrapContent { get; set; }

        internal Type ReturnedType { get; set; }

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

            if (String.IsNullOrEmpty(ContentType))
            {
                ContentType = "application/json";
            }
            else if (context.Request.Headers.AcceptVersion > 0 && ContentType.IndexOf("version=", StringComparison.OrdinalIgnoreCase) < 0)
            {
                ContentType += String.Format(CultureInfo.InvariantCulture, "; version={0}", context.Request.Headers.AcceptVersion);
            }

            if (!String.IsNullOrEmpty(Callback) && !methodNamePattern.IsMatch(Callback))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, Global.InvalidJsonPCallback);
            }

            context.Response.Output.Clear();
            context.Response.SetHeader(context.Response.HeaderNames.ContentType, ContentType);
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            var serializer = JsonSerializerFactory.Create();

            if (context.Request.IsAjax && Rest.Configuration.Options.JsonSettings.LowerPropertiesForAjax)
            {
                serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            if (ReturnedType == null && Content != null)
            {
                ReturnedType = Content.GetType();
            }

            if (!NonGenericCollectionValidator.ValidateType(ReturnedType))
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, Global.NonGenericResultCollections);
            }

            if (ReturnedType != null && ReturnedType.IsGenericType && SerializeAsSpecializedCollection(context, serializer))
            {
                return;
            }

            OutputCompressionManager.FilterResponse(context);

            TryAddCallbackStart(context.Response.Output.Writer);
            serializer.Serialize(context.Response.Output.Writer, WrapContent ? new { d = Content } : Content);
            TryAddCallbackEnd(context.Response.Output.Writer);

            LogResponse(Content);
        }

        private void SerializeAsChunkedSequence(IServiceContext context, JsonSerializer serializer)
        {
            var enumerableContent = (IEnumerable) Content;

            TryAddCallbackStart(context.Response.Output.Writer);
            context.Response.Output.Write(WrapContent ? "{\"d\":[" : "[").Flush();

            bool arrayStart = true;

            foreach (object enumeratedContent in enumerableContent)
            {
                if (!arrayStart)
                {
                    context.Response.Output.Write(",");
                }
                else
                {
                    arrayStart = false;
                }

                serializer.Serialize(context.Response.Output.Writer, enumeratedContent);
                context.Response.Output.Flush();

                LogResponse(enumerableContent);
            }

            context.Response.Output.Write(WrapContent ? "]}" : "]");
            TryAddCallbackEnd(context.Response.Output.Writer);
        }

        private bool SerializeAsSpecializedCollection(IServiceContext context, JsonSerializer serializer)
        {
            Type returnedGenericType = ReturnedType.GetGenericTypeDefinition();

            if (Rest.Configuration.Options.EnumerableChunkedSupport && returnedGenericType == typeof(IEnumerable<>))
            {
                SerializeAsChunkedSequence(context, serializer);
                return true;
            }

            if (Rest.Configuration.Options.QueryableODataSupport && (returnedGenericType == typeof(IQueryable<>) ||
                ReturnedType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryable<>))))
            {
                var odataProvider = Rest.Configuration.ServiceLocator.GetService<IODataProvider>();
                Content = odataProvider.PerformQuery(context, (IQueryable) Content);
            }

            return false;
        }

        private void TryAddCallbackStart(TextWriter writer)
        {
            if (!String.IsNullOrEmpty(Callback))
            {
                writer.Write("{0}(", Callback);
            }
        }

        private void TryAddCallbackEnd(TextWriter writer)
        {
            if (!String.IsNullOrWhiteSpace(Callback))
            {
                writer.Write(");");
            }
        }

        private void LogResponse(object content)
        {
            if (content == null || !LogUtility.CanLog)
            {
                return;
            }

            var options = Rest.Configuration.Options.JsonSettings.ToJsonSerializerSettings();
            options.Formatting = Formatting.Indented;

            string serializedContent = JsonConvert.SerializeObject(WrapContent ? new { d = content } : content, options);

            if (!String.IsNullOrEmpty(Callback))
            {
                serializedContent = String.Concat(Callback, "(", serializedContent, ");");
            }

            LogUtility.LogResponseBody(serializedContent, ContentType);
        }
    }
}
