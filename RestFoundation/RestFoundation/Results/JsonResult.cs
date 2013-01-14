// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a JSON result.
    /// </summary>
    public class JsonResult : IResult
    {
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

            context.Response.Output.Clear();
            context.Response.SetHeader(context.Response.HeaderNames.ContentType, ContentType);
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            var serializer = JsonSerializerFactory.Create();

            if (context.Request.IsAjax)
            {
                serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            if (ReturnedType == null && Content != null)
            {
                ReturnedType = Content.GetType();
            }

            if (ReturnedType != null && ReturnedType.IsGenericType && SerializeAsSpecializedCollection(context, serializer))
            {
                return;
            }

            OutputCompressionManager.FilterResponse(context);

            serializer.Serialize(context.Response.Output.Writer, WrapContent ? new { d = Content } : Content);

            LogResponse(Content);
        }

        private void SerializeAsChunkedSequence(IServiceContext context, JsonSerializer serializer)
        {
            var enumerableContent = (IEnumerable) Content;
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
        }

        private bool SerializeAsSpecializedCollection(IServiceContext context, JsonSerializer serializer)
        {
            if (ReturnedType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                SerializeAsChunkedSequence(context, serializer);
                return true;
            }

            if (ReturnedType.GetGenericTypeDefinition() == typeof(IQueryable<>) ||
                ReturnedType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryable<>)))
            {
                Content = ODataHelper.PerformOdataOperations(Content, context.Request);
            }

            return false;
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
            LogUtility.LogResponseBody(serializedContent, ContentType);
        }
    }
}
