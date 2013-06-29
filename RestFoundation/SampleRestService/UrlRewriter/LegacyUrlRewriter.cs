using System.Collections.Specialized;
using System.Text.RegularExpressions;
using RestFoundation;

namespace SampleRestService.UrlRewriter
{
    public class LegacyUrlRewriter : IUrlRewriter
    {
        // Replaces "/example" with "/sample".
        private const string NewUrlSegment = "/sample";

        // Matches relative URLs such as "/example", "/example/", "/example/cat/sub-cat", "/example.xml" and "/example?key=val",
        // but not "/example2", "/example-new" or "/examples".
        private static readonly Regex OldUrlSegmentRegex = new Regex(@"^/example([/\.\?].*)?$", RegexOptions.IgnoreCase |                                                                                        
                                                                                                RegexOptions.Compiled |
                                                                                                RegexOptions.CultureInvariant);

        public string RewriteUrl(string relativeUrl, NameValueCollection requestHeaders)
        {
            if (OldUrlSegmentRegex.IsMatch(relativeUrl))
            {
                return OldUrlSegmentRegex.Replace(relativeUrl, match => NewUrlSegment + match.Groups[1].Value);
            }

            return null;
        }
    }
}