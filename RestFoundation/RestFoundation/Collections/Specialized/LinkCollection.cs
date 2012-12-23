using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RestFoundation.Runtime;

namespace RestFoundation.Collections.Specialized
{
    /// <summary>
    /// A collection of Link header values.
    /// </summary>
    public class LinkCollection : IEnumerable<string>
    {
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
        /// Returns a list <see cref="Link"/> objects representing the Link header values.
        /// </summary>
        /// <returns>A list of <see cref="Link"/> objects.</returns>
        public IList<Link> ToList()
        {
            return (from v in m_linkValues
                    let link = ParseLinkValue(v)
                    where link.Href != null
                    select link).ToList();
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
                string linkValue = linkValues[i].TrimEnd(',', ' ');

                if (linkValue.Contains(','))
                {
                    string[] linkValueArray = linkValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

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

            string[] linkValues = linkValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            string href = linkValues[0].Trim();

            if (!href.StartsWith("<", StringComparison.Ordinal) || !href.EndsWith(">", StringComparison.Ordinal))
            {
                return default(Link);
            }

            Uri hrefUri;

            if (!Uri.TryCreate(href.TrimStart('<').TrimEnd('>'), UriKind.RelativeOrAbsolute, out hrefUri))
            {
                return default(Link);
            }

            string rel, title;

            if (linkValues.Length > 1)
            {
                ParseRelAndTitle(linkValues, out rel, out title);

                if (rel == null)
                {
                    rel = "link";
                }
            }
            else
            {
                rel = "link";
                title = null;
            }

            return new Link(hrefUri, rel, title);
        }

        private static void ParseRelAndTitle(string[] linkValues, out string rel, out string title)
        {
            rel = null;
            title = null;

            for (int i = 1; i < linkValues.Length; i++)
            {
                string linkValueParameter = linkValues[i];
                int indexOfEquals = linkValueParameter.IndexOf('=');

                if (indexOfEquals <= 0 || indexOfEquals == linkValueParameter.Length - 1)
                {
                    continue;
                }

                string name = linkValueParameter.Substring(0, indexOfEquals).Trim();
                string value = linkValueParameter.Substring(indexOfEquals + 1);

                if (String.Equals("rel", name, StringComparison.OrdinalIgnoreCase))
                {
                    rel = value.TrimStart('"', ' ').TrimEnd('"', ' ');
                }
                else if (String.Equals("title", name, StringComparison.OrdinalIgnoreCase))
                {
                    title = value.TrimStart('"', ' ').TrimEnd('"', ' ');
                }

                if (rel != null && title != null)
                {
                    break;
                }
            }
        }
    }
}
