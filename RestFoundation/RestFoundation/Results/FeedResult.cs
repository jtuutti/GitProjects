using System;
using System.Globalization;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;

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
        /// Gets or sets a value indicating whether dates should be in the XML (ISO 8601) format.
        /// </summary>
        public bool XmlStyleDates { get; set; }

        /// <summary>
        /// Executes the result against the provided service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        public virtual void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

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

            using (XmlWriter writer = new FeedWriter(context.Response.Output.Writer, XmlStyleDates))
            {
                formatter.WriteTo(writer);
            }
        }

        private sealed class FeedWriter : XmlTextWriter
        {
            private readonly bool m_xmlStyleDates;

            public FeedWriter(TextWriter writer, bool xmlStyleDates) : base(writer)
            {
                m_xmlStyleDates = xmlStyleDates;
            }

            public override void WriteString(string text)
            {
                DateTimeOffset time;

                if (m_xmlStyleDates && text != null && text.EndsWith("Z", StringComparison.Ordinal) && DateTimeOffset.TryParse(text, out time))
                {
                    base.WriteString(time.ToString("yyyy-MM-dd'T'HH:mm:sszzz", CultureInfo.InvariantCulture));
                }
                else
                {
                    base.WriteString(text);
                }
            }
        }
    }
}
