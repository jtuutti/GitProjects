using System;
using System.Collections;
using System.Collections.Generic;

namespace RestFoundation.Acl
{
    /// <summary>
    /// Represents an IP network collection.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class IPNetworkCollection : IEnumerable<IPNetwork>, IEnumerator<IPNetwork>
    {
        private readonly byte m_cidrSubnet;
        private readonly IPNetwork m_ipnetwork;
        private double m_enumerator;

        internal IPNetworkCollection(IPNetwork ipnetwork, byte cidrSubnet)
        {
            if (cidrSubnet > 32)
            {
                throw new ArgumentOutOfRangeException("cidrSubnet");
            }

            if (cidrSubnet < ipnetwork.Cidr)
            {
                throw new ArgumentException("cidr");
            }

            m_cidrSubnet = cidrSubnet;
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
                double count = Math.Pow(2, m_cidrSubnet - Cidr);
                return count;
            }
        }

        /// <summary>
        /// Gets the current IP network in the collection.
        /// </summary>
        public IPNetwork Current
        {
            get
            {
                return this[m_enumerator];
            }
        }

        /// <summary>
        /// Gets the current IP network in the collection.
        /// </summary>
        public IPNetwork this[double i]
        {
            get
            {
                if (i >= Count)
                {
                    throw new ArgumentOutOfRangeException("i");
                }

                double size = Count;
                var increment = (int)((Broadcast - Network) / size);
                var uintNetwork = (uint)(Network + ((increment + 1) * i));
                var ipn = new IPNetwork(uintNetwork, m_cidrSubnet);
                return ipn;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        private byte Cidr
        {
            get
            {
                return m_ipnetwork.Cidr;
            }
        }

        private uint Broadcast
        {
            get
            {
                return IPNetwork.ToUint(m_ipnetwork.Broadcast);
            }
        }

        private uint Network
        {
            get
            {
                return IPNetwork.ToUint(m_ipnetwork.Network);
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

            return m_enumerator < Count;
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

        IEnumerator<IPNetwork> IEnumerable<IPNetwork>.GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}
