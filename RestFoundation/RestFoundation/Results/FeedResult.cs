using System;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;

namespace RestFoundation.Results
{
    public class FeedResult : IResult
    {
        public enum SyndicationFormat { Atom, Rss }

        public IServiceContext Context { get; set; }
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }
        public SyndicationFeed Feed { get; set; }
        public SyndicationFormat Format { get; set; }
        public bool XmlStyleDates { get; set; }

        public void Execute()
        {
            if (Response == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No HTTP context found");
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

            Response.Output.Clear();
            Response.SetHeader("Content-Type", contentType);
            Response.SetCharsetEncoding(Request.Headers.AcceptCharsetEncoding);

            using (XmlWriter writer = new FeedWriter(Response.Output.Writer, XmlStyleDates))
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

                if (m_xmlStyleDates && text != null && text.EndsWith("Z") && DateTimeOffset.TryParse(text, out time))
                {
                    base.WriteString(time.ToString("yyyy-MM-dd'T'HH:mm:sszzz"));
                }
                else
                {
                    base.WriteString(text);
                }
            }
        }
    }
}
