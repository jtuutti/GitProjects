using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RestFoundation.Runtime;

namespace RestFoundation.Collections.Specialized
{
    /// <summary>
    /// A collection of an Accept* HTTP header values.
    /// </summary>
    public class AcceptValueCollection : List<AcceptValue>
    {
        private readonly bool m_acceptWildcard;

        /// <summary>
        /// Initializes a new instance of the <see cref="AcceptValueCollection"/> class from
        /// the given string of comma delimited values.
        /// </summary>
        /// <param name="values">The raw <see cref="string"/> of acceptValues to load</param>
        public AcceptValueCollection(string values)
            : this(!String.IsNullOrWhiteSpace(values) ? values.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : new string[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AcceptValueCollection"/> class from
        /// the given string a sequence of strings of comma delimited values.
        /// </summary>
        /// <param name="values">The array of <see cref="AcceptValue"/> strings</param>
        public AcceptValueCollection(IEnumerable<string> values)
        {
            if (values == null)
            {
                values = new string[0];
            }

            int ordinal = -1;

            foreach (string value in values)
            {
                AcceptValue acceptValue = AcceptValue.Parse(value.Trim(), ++ordinal);

                if (acceptValue.Name.Equals("*") || acceptValue.Name.Equals("*/*"))
                {
                    m_acceptWildcard = acceptValue.CanAccept;
                }

                Add(acceptValue);
            }

            Sort(false);
            AutoSort = true;
        }

        /// <summary>
        /// Gets a value indicating whether or not the wildcarded encoding is available and allowed.
        /// </summary>
        public bool AcceptWildcard
        {
            get
            {
                return m_acceptWildcard;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether, after an add operation, the list should be resorted.
        /// </summary>
        public bool AutoSort { get; set; }

        /// <summary>
        /// Gets a collection of all accepted type names.
        /// </summary>
        public ICollection<string> AcceptedNames
        {
            get
            {
                return this.Where(v => v.CanAccept).Select(v => v.Name).ToArray();
            }
        }

        /// <summary>
        /// Adds an item to the list, then applies sorting if <see cref="AutoSort"/> is enabled.
        /// </summary>
        /// <param name="item">The item to add</param>
        public new void Add(AcceptValue item)
        {
            base.Add(item);

            Sort(true);
        }

        /// <summary>
        /// Adds a range of items to the list, then applies sorting if <see cref="AutoSort"/> is enabled.
        /// </summary>
        /// <param name="collection">The items to add</param>
        public new void AddRange(IEnumerable<AcceptValue> collection)
        {
            bool state = AutoSort;
            AutoSort = false;

            base.AddRange(collection);

            AutoSort = state;
            Sort(true);
        }

        /// <summary>
        /// Returns a value indicating whether the accepted item with the provided name is acceptable.
        /// </summary>
        /// <param name="name">The name of the item to search for</param>
        /// <returns>
        /// true if the item is acceptable; otherwise, false.
        /// </returns>
        public bool CanAccept(string name)
        {
            return CanAccept(name, AcceptValueOptionType.None);
        }

        /// <summary>
        /// Returns a value indicating whether the accepted item with the provided name is acceptable.
        /// </summary>
        /// <param name="name">The name of the item to search for</param>
        /// <param name="optionType">The value indicating how to match the accepted values.</param>
        /// <returns>
        /// true if the item is acceptable; otherwise, false.
        /// </returns>
        public bool CanAccept(string name, AcceptValueOptionType optionType)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            AcceptValue value = Find(item => item.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase));

            if (String.Equals(name, value.Name, StringComparison.OrdinalIgnoreCase))
            {
                return value.CanAccept;
            }

            if (optionType != AcceptValueOptionType.IgnoreWildcards && !m_acceptWildcard)
            {
                string[] valueParts = name.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (valueParts.Length == 2 && FindIndex(item => item.Name.Equals(String.Format(CultureInfo.InvariantCulture, "{0}/*", valueParts[0].Trim()))) >= 0)
                {
                    return true;
                }
            }

            return optionType != AcceptValueOptionType.IgnoreWildcards && m_acceptWildcard;
        }

        /// <summary>
        /// Returns the preferred value from the full collection of values that can be accepted.
        /// </summary>
        /// <returns>The preferred value to accept.</returns>
        public AcceptValue? GetPreferredValue()
        {
            for (int i = 0; i < Count; i++)
            {
                AcceptValue currentValue = this[i];

                if (currentValue.CanAccept)
                {
                    return currentValue;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the preferred value from the sequence of provided names that can be accepted.
        /// </summary>
        /// <param name="names">The sequence of accepted names.</param>
        /// <returns>The preferred value sequence to accept in the order of priority.</returns>
        public IEnumerable<AcceptValue> GetPreferredValues(params string[] names)
        {
            if (names == null)
            {
                throw new ArgumentNullException("names");
            }

            if (names.Any(n => n == null || n.IndexOf('*') >= 0))
            {
                throw new ArgumentException("An accepted name cannot be null or contain a wildcard", "names");
            }

            if (names.Length == 0)
            {
                yield break;
            }

            for (int i = 0; i < Count; i++)
            {
                AcceptValue currentValue = this[i];

                if (currentValue.CanAccept && Array.IndexOf(names, currentValue.Name) >= 0)
                {
                    yield return currentValue;
                }
            }
        }

        /// <summary>
        /// Returns the preferred name from the full collection of values that can be accepted.
        /// </summary>
        /// <returns>The preferred name to accept.</returns>
        public string GetPreferredName()
        {
            for (int i = 0; i < Count; i++)
            {
                AcceptValue currentValue = this[i];

                if (currentValue.CanAccept)
                {
                    return currentValue.Name;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the preferred names from the sequence of provided names that can be accepted.
        /// </summary>
        /// <param name="names">The sequence of accepted names.</param>
        /// <returns>The preferred name sequence to accept in the order of priority.</returns>
        public IEnumerable<string> GetPreferredNames(params string[] names)
        {
            if (names == null)
            {
                throw new ArgumentNullException("names");
            }

            if (names.Any(n => n == null || n.IndexOf('*') >= 0))
            {
                throw new ArgumentException("An accepted name cannot be null or contain a wildcard", "names");
            }

            if (names.Length == 0)
            {
                yield break;
            }

            var previousNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < Count; i++)
            {
                AcceptValue currentValue = this[i];

                if (!currentValue.CanAccept || previousNames.Contains(currentValue.Name) || Array.IndexOf(names, currentValue.Name) < 0)
                {
                    continue;
                }

                previousNames.Add(currentValue.Name);

                yield return currentValue.Name;
            }
        }

        /// <summary>
        /// Sorts the list comparing by weight in descending order.
        /// </summary>
        /// <param name="autosort">A <see cref="bool"/> indicating whether to perform auto-sort</param>
        public void Sort(bool autosort)
        {
            if (!autosort || AutoSort)
            {
                Sort(AcceptValue.CompareByWeightDescending);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Join("; ", this);
        }
    }
}
