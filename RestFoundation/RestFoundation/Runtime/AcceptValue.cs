// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Web;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents an Accept* HTTP header value including its name, weight and an optional level.
    /// </summary>
    public struct AcceptValue : IComparable<AcceptValue>, IEquatable<AcceptValue>
    {
        private const int DefaultLevel = 0;
        private const float DefaultWeight = 1.0f;

        private const string AcceptKey = "accept-key";
        private const string LevelKey = "level";
        private const string WeightKey = "q";
        private const string CharsetKey = "charset";

        private const string InvalidAcceptHeaderFormat = "One of the Accept* HTTP headers has an invalid format";
        private const string InvalidValueMessage = "Invalid {0} value to parse";

        private readonly string m_name;
        private readonly float m_weight;
        private readonly int m_level;
        private readonly string m_charset;
        private readonly int m_ordinal;

        /// <summary>
        /// Initializes a new instance of the <see cref="AcceptValue"/> struct
        /// by parsing the given value for name and weight and assigns the given
        /// ordinal.
        /// </summary>
        /// <param name="value">The value to be parsed e.g. gzip=0.3</param>
        public AcceptValue(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            this = Parse(value);
        }

        private AcceptValue(string name, int ordinal, float weight, int level, string charset)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (weight < 0.0 || weight > DefaultWeight)
            {
                throw new ArgumentOutOfRangeException("weight");
            }

            if (level < 0)
            {
                throw new ArgumentOutOfRangeException("level");
            }

            m_name = name.Trim().ToLowerInvariant();
            m_ordinal = ordinal;
            m_weight = weight;
            m_level = level;
            m_charset = charset;
        }

        /// <summary>
        /// Gets the name of the value part.
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
        }

        /// <summary>
        /// Gets the weight (or qvalue, quality value) of the item.
        /// </summary>
        public float Weight
        {
            get
            {
                return m_weight;
            }
        }

        /// <summary>
        /// Gets the level of the item. If a level is not specified, it is assumed to be 0.
        /// </summary>
        public int Level
        {
            get
            {
                return m_level;
            }
        }

        /// <summary>
        /// Gets the item encoding. If an encoding is not specified, UTF-8 is assumed.
        /// </summary>
        public string Charset
        {
            get
            {
                return m_charset;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value can be accepted
        /// i.e. it's weight is greater than zero.
        /// </summary>
        public bool CanAccept
        {
            get
            {
                return m_weight > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value is empty
        /// i.e. has no name.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(m_name);
            }
        }

        /// <summary>
        /// Compares two <see cref="AcceptValue"/> objects for equality.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if both objects are equivalent; otherwise, false.</returns>
        public static bool operator ==(AcceptValue left, AcceptValue right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="AcceptValue"/> objects for inequality.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if both objects are not equivalent; otherwise, false.</returns>
        public static bool operator !=(AcceptValue left, AcceptValue right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compares whether the first <see cref="AcceptValue"/> is less than the second.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if the first object is less than the second; otherwise, false.</returns>
        public static bool operator <(AcceptValue left, AcceptValue right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Compares whether the first <see cref="AcceptValue"/> is greater than the second.
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true if the first object is greater than the second; otherwise, false.</returns>
        public static bool operator >(AcceptValue left, AcceptValue right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Returns a string for the name and weight (qvalue).
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse</param>
        /// <returns>The parsed <see cref="AcceptValue"/> instance</returns>
        public static AcceptValue Parse(string value)
        {
            return Parse(value, 0);
        }

        /// <summary>
        /// Returns a string for the name and weight (qvalue).
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse</param>
        /// <param name="ordinal">The order of item in sequence</param>
        /// <returns>The parsed <see cref="AcceptValue"/> instance</returns>
        public static AcceptValue Parse(string value, int ordinal)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("value");
            }

            string[] items = value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            var itemList = new List<Tuple<string, string>>();

            foreach (var item in items)
            {
                string[] parts = item.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 1 && itemList.Count == 0)
                {
                    itemList.Add(Tuple.Create(AcceptKey, parts[0].Trim().ToLowerInvariant()));
                }
                else if (parts.Length > 1)
                {
                    itemList.Add(Tuple.Create(parts[0].Trim().ToLowerInvariant(), parts[1].Trim().ToLowerInvariant()));
                }
            }

            try
            {
                return new AcceptValue(GetAcceptValue(itemList), ordinal, GetWeightValue(itemList), GetLevelValue(itemList), GetCharsetValue(itemList));
            }
            catch (ArgumentException)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, InvalidAcceptHeaderFormat);
            }
            catch (InvalidOperationException)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, InvalidAcceptHeaderFormat);
            }
        }

        /// <summary>
        /// Compares two <see cref="AcceptValue"/> instances in ascending order.
        /// </summary>
        /// <param name="first">The first <see cref="AcceptValue"/></param>
        /// <param name="second">The second <see cref="AcceptValue"/></param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// </returns>
        public static int CompareByWeightAscending(AcceptValue first, AcceptValue second)
        {
            return first.CompareTo(second);
        }

        /// <summary>
        /// Compares two <see cref="AcceptValue"/> instances in descending order.
        /// </summary>
        /// <param name="first">The first <see cref="AcceptValue"/></param>
        /// <param name="second">The second <see cref="AcceptValue"/></param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value
        /// </returns>
        public static int CompareByWeightDescending(AcceptValue first, AcceptValue second)
        {
            return -first.CompareTo(second);
        }

        /// <summary>
        /// Compares this instance to another <see cref="AcceptValue"/> by comparing
        /// first weights, then ordinals.
        /// </summary>
        /// <param name="other">The <see cref="AcceptValue"/> to compare</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value
        /// </returns>
        public int CompareTo(AcceptValue other)
        {
            int value = m_weight.CompareTo(other.m_weight);

            if (value == 0)
            {
                value = m_level.CompareTo(other.m_level);
            }

            if (value == 0)
            {
                value = -m_ordinal.CompareTo(other.m_ordinal);
            }

            return value;
        }

        /// <summary>
        /// Indicates whether this instance and the provided <see cref="AcceptValue"/> are equal.
        /// </summary>
        /// <param name="other">Another <see cref="AcceptValue"/> object to compare to.</param>
        /// <returns>
        /// true if <paramref name="other"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public bool Equals(AcceptValue other)
        {
            return Equals(other.m_name, m_name) && other.m_weight.Equals(m_weight) && other.m_ordinal == m_ordinal;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is AcceptValue && Equals((AcceptValue)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = m_name != null ? m_name.GetHashCode() : 0;
                result = (result * 397) ^ m_weight.GetHashCode();
                result = (result * 397) ^ m_ordinal;
                return result;
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}, {1}", Name, Weight);
        }

        private static string GetAcceptValue(List<Tuple<string, string>> itemList)
        {
            Tuple<string, string> acceptItem = itemList.Find(i => i.Item1 == AcceptKey);

            if (acceptItem == null || String.IsNullOrWhiteSpace(acceptItem.Item1) || String.IsNullOrWhiteSpace(acceptItem.Item2))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, InvalidValueMessage, "accept"));
            }

            return acceptItem.Item2;
        }

        private static float GetWeightValue(List<Tuple<string, string>> itemList)
        {
            Tuple<string, string> weightItem = itemList.Find(i => i.Item1 == WeightKey);
            float weight;

            if (weightItem == null || String.IsNullOrWhiteSpace(weightItem.Item1))
            {
                weight = DefaultWeight;
            }
            else if (!Single.TryParse(weightItem.Item2, out weight))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, InvalidValueMessage, "weight"));
            }

            return weight;
        }

        private static int GetLevelValue(List<Tuple<string, string>> itemList)
        {
            Tuple<string, string> levelItem = itemList.Find(i => i.Item1 == LevelKey);
            int level;

            if (levelItem == null || String.IsNullOrWhiteSpace(levelItem.Item1))
            {
                level = DefaultLevel;
            }
            else if (!Int32.TryParse(levelItem.Item2, out level))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, InvalidValueMessage, "level"));
            }

            return level;
        }

        private static string GetCharsetValue(List<Tuple<string, string>> itemList)
        {
            Tuple<string, string> charsetItem = itemList.Find(i => i.Item1 == CharsetKey);

            return charsetItem != null && !String.IsNullOrWhiteSpace(charsetItem.Item2) ? charsetItem.Item2.Trim().ToUpperInvariant() : null;
        }
    }
}
