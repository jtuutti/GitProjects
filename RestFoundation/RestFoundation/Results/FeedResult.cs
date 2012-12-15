// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents an RSS/Atom feed result.
    /// </summary>
    public class FeedResult : IResult
    {
        /// <summary>
        /// Defines a syndication format.
        /// </summary>
        public enum SyndicationFormat
        {
            /// <summary>
            /// Atom
            /// </summary>
            Atom,

            /// <summary>
            /// RSS
            /// </summary>
            Rss
        }

        /// <summary>
        /// Gets or sets the feed instance.
        /// </summary>
        public SyndicationFeed Feed { get; set; }

        /// <summary>
        /// Gets or sets the syndication format.
        /// </summary>
        public SyndicationFormat Format { get; set; }

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

            if (Feed == null)
            {
                return;
            }

            SyndicationFeedFormatter formatter;
            string contentType;

            if (Format == SyndicationFormat.Rss)
            {
                formatter = new Rss20FeedFormatter(Feed);
                contentType = "application/rss+xml";
            }
            else
            {
                formatter = new Atom10FeedFormatter(Feed);
                contentType = "application/atom+xml";
            }

            context.Response.Output.Clear();
            context.Response.SetHeader(context.Response.Headers.ContentType, contentType);
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

            var responseBuilder = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(responseBuilder, new XmlWriterSettings { Indent = true }))
            {
                formatter.WriteTo(writer);
                writer.Flush();
            }

            string response = responseBuilder.ToString();
            context.Response.Output.Write(response);

            LogResponse(response, contentType);
        }

        private static void LogResponse(string response, string contentType)
        {
            if (response != null && LogUtility.CanLog)
            {
                LogUtility.LogResponseBody(response, contentType);
            }
        }
    }
}
