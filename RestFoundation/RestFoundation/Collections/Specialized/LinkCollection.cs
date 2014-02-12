// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RestFoundation.Collections.Specialized
{
    /// <summary>
    /// A collection of Link header values.
    /// </summary>
    public class LinkCollection : IEnumerable<string>
    {
        private const string CommaPlaceholder = "<%cm%>";
        private const string SemicolonPlaceholder = "<%sc%>";
        private const string DefaultRelation = "link";
        private const string Comma = ",";
        private const string EqualsSign = "=";
        private const string HashSign = "#";
        private const string LessThan = "<";
        private const string GreaterThan = ">";
        private const string QuotationMark = "\"";
        private const string Semicolon = ";";
        private const string Space = " ";

        private static readonly Regex valueGroupRegex = new Regex(@"=\s*\""([^\""]*[,;][^\""]*)+\""", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly IEnumerable<string> m_linkValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkCollection"/> class.
        /// </summary>
        /// <param name="linkValues">A list of Link header values.</param>
        public LinkCollection(IList<string> linkValues)
        {
            if (linkValues == null)
            {
                throw new ArgumentNullException("linkValues");
            }

            m_linkValues = SeparateLinkValues(linkValues);
        }

        /// <summary>
        /// Returns an array of <see cref="Link"/> objects representing the Link header values.
        /// </summary>
        /// <returns>An array of <see cref="Link"/> objects.</returns>
        public Link[] ToArray()
        {
            return GetLinks().ToArray();
        }

        /// <summary>
        /// Returns a list <see cref="Link"/> objects representing the Link header values.
        /// </summary>
        /// <returns>A list of <see cref="Link"/> objects.</returns>
        public IList<Link> ToList()
        {
            return GetLinks().ToList();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<string> GetEnumerator()
        {
            return m_linkValues.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static IEnumerable<string> SeparateLinkValues(IList<string> linkValues)
        {
            var separatedLinkValues = new List<string>();

            for (int i = 0; i < linkValues.Count; i++)
            {
                string linkValue = linkValues[i].TrimEnd(Comma[0], Space[0]);

                linkValue = valueGroupRegex.Replace(linkValue, m => m.Value.Replace(Comma, CommaPlaceholder).Replace(Semicolon, SemicolonPlaceholder));

                if (linkValue.IndexOf(Comma, StringComparison.Ordinal) >= 0)
                {
                    string[] linkValueArray = linkValue.Split(new[] { Comma }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string distinctLinkValue in linkValueArray)
                    {
                        separatedLinkValues.Add(distinctLinkValue);
                    }
                }
                else
                {
                    separatedLinkValues.Add(linkValue);
                }
            }

            return separatedLinkValues;
        }

        private static Link ParseLinkValue(string linkValue)
        {
            if (String.IsNullOrWhiteSpace(linkValue))
            {
                return default(Link);
            }

            string[] linkParameterValues = linkValue.Split(new[] { Semicolon }, StringSplitOptions.RemoveEmptyEntries);
            string href = linkParameterValues[0].Trim();

            if (!href.StartsWith(LessThan, StringComparison.Ordinal) || !href.EndsWith(GreaterThan, StringComparison.Ordinal))
            {
                return default(Link);
            }

            Uri hrefUri;

            if (!Uri.TryCreate(href.TrimStart(LessThan[0]).TrimEnd(GreaterThan[0]), UriKind.RelativeOrAbsolute, out hrefUri))
            {
                return default(Link);
            }

            string rel, anchor, title;
            var additionalParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (linkParameterValues.Length > 1)
            {
                ParseRelAndTitle(linkParameterValues, additionalParameters, out rel, out anchor, out title);

                if (rel == null)
                {
                    rel = DefaultRelation;
                }
            }
            else
            {
                rel = DefaultRelation;
                anchor = null;
                title = null;
            }

            return new Link(hrefUri, rel, anchor, title, additionalParameters);
        }

        private static void ParseRelAndTitle(string[] linkValues, IDictionary<string, string> additionalParameters, out string rel, out string anchor, out string title)
        {
            rel = null;
            anchor = null;
            title = null;

            for (int i = 1; i < linkValues.Length; i++)
            {
                string linkValueParameter = linkValues[i];
                int indexOfEquals = linkValueParameter.IndexOf(EqualsSign, StringComparison.Ordinal);

                if (indexOfEquals <= 0 || indexOfEquals == linkValueParameter.Length - 1)
                {
                    continue;
                }

                string name = linkValueParameter.Substring(0, indexOfEquals).Trim();
                string value = linkValueParameter.Substring(indexOfEquals + 1);

                if (String.Equals("rel", name, StringComparison.OrdinalIgnoreCase))
                {
                    rel = ParseLinkParameterValue(value);
                }
                else if (String.Equals("title", name, StringComparison.OrdinalIgnoreCase))
                {
                    title = ParseLinkParameterValue(value);
                }
                else if (String.Equals("anchor", name, StringComparison.OrdinalIgnoreCase))
                {
                    anchor = ParseLinkParameterValue(value);

                    if (!anchor.StartsWith(HashSign, StringComparison.Ordinal))
                    {
                        anchor = null;
                    }
                }
                else
                {
                    additionalParameters[name] = ParseLinkParameterValue(value);
                }
             }
        }

        private static string ParseLinkParameterValue(string value)
        {
            return value.Trim(QuotationMark[0], Space[0]).Replace(CommaPlaceholder, Comma).Replace(SemicolonPlaceholder, Semicolon);
        }

        private IEnumerable<Link> GetLinks()
        {
            return from v in m_linkValues
                   let link = ParseLinkValue(v)
                   where link.Href != null
                   select link;
        }
    }
}
