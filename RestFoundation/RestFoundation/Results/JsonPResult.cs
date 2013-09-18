// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        /// Initializes a new instance of the <see cref="JsonPResult"/> class.
        /// </summary>
        public JsonPResult()
        {
            WrapContent = Rest.Configuration.Options.JsonSettings.WrapContentResponse;
        }

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
        /// Gets or sets a value indicating whether to wrap the content.
        /// </summary>
        public bool WrapContent { get; set; }

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
                throw new HttpResponseException(HttpStatusCode.BadRequest, Resources.Global.InvalidJsonPCallback);
            }

            if (String.IsNullOrEmpty(ContentType))
            {
                ContentType = "application/javascript";
            }
            else if (context.Request.Headers.AcceptVersion > 0 && ContentType.IndexOf("version=", StringComparison.OrdinalIgnoreCase) < 0)
            {
                ContentType += String.Format(CultureInfo.InvariantCulture, "; version={0}", context.Request.Headers.AcceptVersion);
            }

            context.Response.Output.Clear();
            context.Response.SetHeader(context.Response.HeaderNames.ContentType, ContentType);
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

            var settings = Rest.Configuration.Options.JsonSettings.ToJsonSerializerSettings();

            if (Rest.Configuration.Options.JsonSettings.LowerPropertiesForAjax)
            {
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            string response = new StringBuilder().Append(Callback)
                                                 .Append("(")
                                                 .Append(WrapContent ? "{\"d\":" : String.Empty)
                                                 .Append(Content != null ? JsonConvert.SerializeObject(Content, settings) : "null")
                                                 .Append(WrapContent ? "}" : String.Empty)
                                                 .Append(");")
                                                 .ToString();

            context.Response.Output.Write(response);

            LogUtility.LogResponseBody(response, ContentType);
        }
    }
}
