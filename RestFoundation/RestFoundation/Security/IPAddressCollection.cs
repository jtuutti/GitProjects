using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace RestFoundation.Security
{
    /// <summary>
    /// Represents an IP address collection
    /// </summary>
    public sealed class IPAddressCollection : IEnumerable<IPAddress>, IEnumerator<IPAddress>
    {
        private readonly IPNetwork m_ipnetwork;
        private double m_enumerator;

        internal IPAddressCollection(IPNetwork ipnetwork)
        {
            m_ipnetwork = ipnetwork;
            m_enumerator = -1;
        }

        /// <summary>
        /// Gets a count of usable IP addresses.
        /// </summary>
        public double Count
        {
            get
            {
                return m_ipnetwork.Usable + 2;
            }
        }

        /// <summary>
        /// Gets the current IP address in the collection.
        /// </summary>
        public IPAddress Current
        {
            get
            {
                return this[m_enumerator];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Gets an IP address at the provided index.
        /// </summary>
        /// <param name="i">The index.</param>
        /// <returns>An IP address at the specified index.</returns>
        public IPAddress this[double i]
        {
            get
            {
                if (i >= Count)
                {
                    throw new ArgumentOutOfRangeException("i");
                }

                IPNetworkCollection ipn = IPNetwork.Subnet(m_ipnetwork, 32);
                return ipn[i].Network;
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has
        /// passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            m_enumerator++;

            if (m_enumerator >= Count)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            m_enumerator = -1;
        }

        /// <summary>
        /// Disposes the collection instance.
        /// </summary>
        public void Dispose()
        {
        }

        IEnumerator<IPAddress> IEnumerable<IPAddress>.GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}
