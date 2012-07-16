using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Sockets;

namespace RestFoundation.Security
{
    /// <summary>
    /// Represents an IP address range.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class IPAddressRange
    {
        private readonly AddressFamily m_addressFamily;
        private readonly byte[] m_lowerBytes;
        private readonly byte[] m_upperBytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPAddressRange"/> class.
        /// </summary>
        /// <param name="address">
        /// A single IP address to become the lower and the upper range bound.
        /// </param>
        public IPAddressRange(IPAddress address) : this(address, address)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPNetwork"/> class.
        /// </summary>
        /// <param name="cidr">
        /// An IP network that represents a starting IP and a subnet in a CIDR notation.
        /// </param>
        public IPAddressRange(IPNetwork cidr)
        {
            if (cidr == null) throw new ArgumentNullException("cidr");

            m_addressFamily = cidr.FirstUsable.AddressFamily;
            m_lowerBytes = cidr.FirstUsable.GetAddressBytes();
            m_upperBytes = cidr.LastUsable.GetAddressBytes();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPAddressRange"/> class.
        /// </summary>
        /// <param name="lower">An IP address to become the lower range bound.</param>
        /// <param name="upper">An IP address to become the upper range bound.</param>
        public IPAddressRange(IPAddress lower, IPAddress upper)
        {
            if (lower == null) throw new ArgumentNullException("lower");
            if (upper == null) throw new ArgumentNullException("upper");

            if (!Equals(lower.AddressFamily, upper.AddressFamily))
            {
                throw new ArgumentOutOfRangeException("upper", "The upper bound address is from a different family than the lower bound address.");
            }

            m_addressFamily = lower.AddressFamily;
            m_lowerBytes = lower.GetAddressBytes();
            m_upperBytes = upper.GetAddressBytes();
        }

        /// <summary>
        /// Gets a sequence of configured IP ranges from the provided configuration file section.
        /// </summary>
        /// <param name="sectionName">A section name in the web.config/app.config file.</param>
        /// <returns>A sequence of IP ranges.</returns>
        public static IEnumerable<IPAddressRange> GetConfiguredRanges(string sectionName)
        {
            if (String.IsNullOrEmpty(sectionName)) throw new ArgumentNullException("sectionName");

            var section = ConfigurationManager.GetSection(sectionName) as NameValueCollection;

            if (section == null)
            {
                yield break;
            }

            for (int i = 0; i < section.Count; i++)
            {
                string[] values = section.GetValues(i);
                if (values == null) continue;

                foreach (string value in values)
                {
                    IPAddressRange range = CreateIPRange(value);

                    if (range != null)
                    {
                        yield return range;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a value indicating whether the IP address is in the current IP range.
        /// </summary>
        /// <param name="address">An IP address to check.</param>
        /// <returns>
        /// true if the provided address is in the current IP range; otherwise, false.
        /// </returns>
        public bool IsInRange(string address)
        {
            return IsInRange(IPAddress.Parse(address));
        }

        /// <summary>
        /// Returns a value indicating whether the IP address is in the current IP range.
        /// </summary>
        /// <param name="address">An IP address to check.</param>
        /// <returns>
        /// true if the provided address is in the current IP range; otherwise, false.
        /// </returns>
        public bool IsInRange(IPAddress address)
        {
            if (address == null || address.AddressFamily != m_addressFamily)
            {
                return false;
            }

            bool lowerBoundary = true, upperBoundary = true;
            byte[] addressBytes = address.GetAddressBytes();

            for (int i = 0; i < m_lowerBytes.Length && (lowerBoundary || upperBoundary); i++)
            {
                if ((lowerBoundary && addressBytes[i] < m_lowerBytes[i]) || (upperBoundary && addressBytes[i] > m_upperBytes[i]))
                {
                    return false;
                }

                lowerBoundary &= (addressBytes[i] == m_lowerBytes[i]);
                upperBoundary &= (addressBytes[i] == m_upperBytes[i]);
            }

            return true;
        }

        private static IPAddressRange CreateIPRange(string addressString)
        {
            if (String.IsNullOrWhiteSpace(addressString)) return null;

            if (addressString.Contains("/"))
            {
                return new IPAddressRange(IPNetwork.Parse(addressString));
            }

            string[] addressRange = addressString.Split('-');

            if (addressRange.Length == 2)
            {
                return new IPAddressRange(IPAddress.Parse(addressRange[0].Trim()), IPAddress.Parse(addressRange[1].Trim()));
            }

            return new IPAddressRange(IPAddress.Parse(addressString));
        }
    }
}
