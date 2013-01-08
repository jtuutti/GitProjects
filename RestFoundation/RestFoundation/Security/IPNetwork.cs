// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace RestFoundation.Security
{
    /// <summary>
    /// Represents an IP network including a starting IP and a subnet.
    /// </summary>
    /// <remarks>
    /// Unlike the <see cref="IPAddressRange"/> class, currently this class only works with IPv4.
    /// Use an address range instead of CIDR notation in the configuration file for IPv6 addresses.
    /// </remarks>
    [CLSCompliant(false)]
    public sealed class IPNetwork : IComparable<IPNetwork>
    {
        private readonly uint m_ipaddress;
        private readonly byte m_cidr;

        internal IPNetwork(uint ipaddress, byte cidr)
        {
            if (cidr > 32)
            {
                throw new ArgumentOutOfRangeException("cidr");
            }

            m_ipaddress = ipaddress;
            m_cidr = cidr;
        }

        /// <summary>
        /// Gets a broadcast IP address.
        /// </summary>
        public IPAddress Broadcast
        {
            get
            {
                return ToIPAddress(BroadcastAsInt);
            }
        }

        /// <summary>
        /// Gets the first usable IP address.
        /// </summary>
        public IPAddress FirstUsable
        {
            get
            {
                uint uintFirstUsable = (Usable <= 0) ? NetworkAsInt : NetworkAsInt + 1;
                return ToIPAddress(uintFirstUsable);
            }
        }

        /// <summary>
        /// Gets the last usable IP address.
        /// </summary>
        public IPAddress LastUsable
        {
            get
            {
                uint uintLastUsable = (Usable <= 0) ? NetworkAsInt : BroadcastAsInt - 1;
                return ToIPAddress(uintLastUsable);
            }
        }

        /// <summary>
        /// Gets the net mask.
        /// </summary>
        public IPAddress Netmask
        {
            get
            {
                return ToIPAddress(NetMaskAsInt);
            }
        }

        /// <summary>
        /// Gets the network as an IP address.
        /// </summary>
        public IPAddress Network
        {
            get
            {
                return ToIPAddress(NetworkAsInt);
            }
        }

        /// <summary>
        /// Gets the CIDR byte.
        /// </summary>
        public byte Cidr
        {
            get
            {
                return m_cidr;
            }
        }

        /// <summary>
        /// Gets a number of usable IP addresses.
        /// </summary>
        public uint Usable
        {
            get
            {
                int cidrValue = ToCidr(NetMaskAsInt);
                uint usableIps = (cidrValue > 30) ? 0 : ((0xffffffff >> cidrValue) - 1);
                return usableIps;
            }
        }

        private uint NetworkAsInt
        {
            get
            {
                uint uintNetwork = m_ipaddress & NetMaskAsInt;
                return uintNetwork;
            }
        }

        private uint NetMaskAsInt
        {
            get
            {
                return ToUint(m_cidr);
            }
        }

        private uint BroadcastAsInt
        {
            get
            {
                uint uintBroadcast = NetworkAsInt + ~NetMaskAsInt;
                return uintBroadcast;
            }
        }

        /// <summary>
        /// Parses an IP address and a netmask into a <see cref="IPNetwork"/> object.
        /// </summary>
        /// <param name="ipaddress">The address.</param>
        /// <param name="netmask">The netmask.</param>
        /// <returns>A new <see cref="IPNetwork"/> instance.</returns>
        public static IPNetwork Parse(string ipaddress, string netmask)
        {
            IPNetwork ipnetwork;
            InternalParse(false, ipaddress, netmask, out ipnetwork);

            return ipnetwork;
        }

        /// <summary>
        /// Parses an IP address and a CIDR byte into a <see cref="IPNetwork"/> object.
        /// </summary>
        /// <param name="ipaddress">The address.</param>
        /// <param name="cidr">The CIDR byte.</param>
        /// <returns>A new <see cref="IPNetwork"/> instance.</returns>
        public static IPNetwork Parse(string ipaddress, byte cidr)
        {
            IPNetwork ipnetwork;
            InternalParse(false, ipaddress, cidr, out ipnetwork);

            return ipnetwork;
        }

        /// <summary>
        /// Parses an IP address and a netmask into a <see cref="IPNetwork"/> object.
        /// </summary>
        /// <param name="ipaddress">The address.</param>
        /// <param name="netmask">The netmask.</param>
        /// <returns>A new <see cref="IPNetwork"/> instance.</returns>
        public static IPNetwork Parse(IPAddress ipaddress, IPAddress netmask)
        {
            IPNetwork ipnetwork;
            InternalParse(false, ipaddress, netmask, out ipnetwork);

            return ipnetwork;
        }

        /// <summary>
        /// Parses a network string in CIDR notation into a <see cref="IPNetwork"/> object.
        /// </summary>
        /// <param name="network">The network string.</param>
        /// <returns>A new <see cref="IPNetwork"/> instance.</returns>
        public static IPNetwork Parse(string network)
        {
            IPNetwork ipnetwork;
            InternalParse(false, network, out ipnetwork);

            return ipnetwork;
        }

        /// <summary>
        /// Tries to parse an IP address and a netmask into a <see cref="IPNetwork"/> object.
        /// </summary>
        /// <param name="ipaddress">The address.</param>
        /// <param name="netmask">The netmask.</param>
        /// <param name="ipnetwork">The network object.</param>
        /// <returns>true if the address was parsed sucessfully; otherwise, false.</returns>
        public static bool TryParse(string ipaddress, string netmask, out IPNetwork ipnetwork)
        {
            IPNetwork ipnetwork2;
            InternalParse(true, ipaddress, netmask, out ipnetwork2);
            bool parsed = ipnetwork2 != null;
            ipnetwork = ipnetwork2;
            return parsed;
        }

        /// <summary>
        /// Tries to parse an IP address and a CIDR byte into a <see cref="IPNetwork"/> object.
        /// </summary>
        /// <param name="ipaddress">The address.</param>
        /// <param name="cidr">The CIDR byte.</param>
        /// <param name="ipnetwork">The network object.</param>
        /// <returns>true if the address was parsed sucessfully; otherwise, false.</returns>
        public static bool TryParse(string ipaddress, byte cidr, out IPNetwork ipnetwork)
        {
            IPNetwork ipnetwork2;
            InternalParse(true, ipaddress, cidr, out ipnetwork2);
            bool parsed = ipnetwork2 != null;
            ipnetwork = ipnetwork2;
            return parsed;
        }

        /// <summary>
        /// Tries to parse a network string in CIDR notation into a <see cref="IPNetwork"/> object.
        /// </summary>
        /// <param name="network">The network string.</param>
        /// <param name="ipnetwork">The network object.</param>
        /// <returns>true if the address was parsed sucessfully; otherwise, false.</returns>
        public static bool TryParse(string network, out IPNetwork ipnetwork)
        {
            IPNetwork ipnetwork2;
            InternalParse(true, network, out ipnetwork2);
            bool parsed = ipnetwork2 != null;
            ipnetwork = ipnetwork2;
            return parsed;
        }

        /// <summary>
        /// Tries to parse an IP address and a netmask into a <see cref="IPNetwork"/> object.
        /// </summary>
        /// <param name="ipaddress">The address.</param>
        /// <param name="netmask">The netmask.</param>
        /// <param name="ipnetwork">The network object.</param>
        /// <returns>true if the address was parsed sucessfully; otherwise, false.</returns>
        public static bool TryParse(IPAddress ipaddress, IPAddress netmask, out IPNetwork ipnetwork)
        {
            IPNetwork ipnetwork2;
            InternalParse(true, ipaddress, netmask, out ipnetwork2);
            bool parsed = ipnetwork2 != null;
            ipnetwork = ipnetwork2;
            return parsed;
        }

        /// <summary>
        /// Gets the supernet for an array of networks.
        /// </summary>
        /// <param name="ipnetworks">An array of networks.</param>
        /// <returns>An array of super networks.</returns>
        public static IPNetwork[] Supernet(IPNetwork[] ipnetworks)
        {
            IPNetwork[] supernet;
            InternalSupernet(false, ipnetworks, out supernet);
            return supernet;
        }

        /// <summary>
        /// Tries to get the supernet for an array of networks.
        /// </summary>
        /// <param name="ipnetworks">An array of networks.</param>
        /// <param name="supernet">An array of super networks.</param>
        /// <returns>true if the operation was performed sucessfully; otherwise, false.</returns>
        public static bool TrySupernet(IPNetwork[] ipnetworks, out IPNetwork[] supernet)
        {
            bool supernetted = InternalSupernet(true, ipnetworks, out supernet);
            return supernetted;
        }

        /// <summary>
        /// Tries to get the internal supernet for an array of networks.
        /// </summary>
        /// <param name="trySupernet">A value indicating whether to try to get the supernet.</param>
        /// <param name="ipnetworks">An array of networks.</param>
        /// <param name="supernet">An array of super networks.</param>
        /// <returns>true if the operation was performed sucessfully; otherwise, false.</returns>
        public static bool InternalSupernet(bool trySupernet, IPNetwork[] ipnetworks, out IPNetwork[] supernet)
        {
            if (ipnetworks == null)
            {
                if (trySupernet == false)
                {
                    throw new ArgumentNullException("ipnetworks");
                }
                supernet = null;
                return false;
            }

            if (ipnetworks.Length <= 0)
            {
                supernet = new IPNetwork[0];
                return true;
            }

            var supernetted = new List<IPNetwork>();
            List<IPNetwork> ipns = Array2List(ipnetworks);
            Stack<IPNetwork> current = List2Stack(ipns);

            int previousCount = 0;
            int currentCount = current.Count;

            while (previousCount != currentCount)
            {
                supernetted.Clear();

                while (current.Count > 1)
                {
                    IPNetwork ipn1 = current.Pop();
                    IPNetwork ipn2 = current.Peek();

                    IPNetwork outNetwork;
                    bool success = TrySupernet(ipn1, ipn2, out outNetwork);
                    if (success)
                    {
                        current.Pop();
                        current.Push(outNetwork);
                    }
                    else
                    {
                        supernetted.Add(ipn1);
                    }
                }

                if (current.Count == 1)
                {
                    supernetted.Add(current.Pop());
                }

                previousCount = currentCount;
                currentCount = supernetted.Count;
                current = List2Stack(supernetted);
            }

            supernet = supernetted.ToArray();
            return true;
        }

        /// <summary>
        /// Converts an IP address to an unsigned integer.
        /// </summary>
        /// <param name="ipaddress">The IP address.</param>
        /// <returns>The <see cref="uint"/> value.</returns>
        public static uint ToUint(IPAddress ipaddress)
        {
            uint? uintIPAddress;
            InternalToUint(false, ipaddress, out uintIPAddress);
            return uintIPAddress.HasValue ? uintIPAddress.Value : 0;
        }

        /// <summary>
        /// Tries to convert an IP address to an unsigned integer.
        /// </summary>
        /// <param name="ipaddress">The IP address.</param>
        /// <param name="uintIPAddress">The <see cref="uint"/> value.</param>
        /// <returns>true if the operation was performed sucessfully; otherwise, false.</returns>
        public static bool TryToUint(IPAddress ipaddress, out uint? uintIPAddress)
        {
            uint? uintIPAddress2;
            InternalToUint(true, ipaddress, out uintIPAddress2);
            bool parsed = uintIPAddress2 != null;
            uintIPAddress = uintIPAddress2;
            return parsed;
        }

        /// <summary>
        /// Gets the CIDR byte from a net mask.
        /// </summary>
        /// <param name="netmask">The net mask.</param>
        /// <returns>The CIDR byte.</returns>
        public static byte ToCidr(IPAddress netmask)
        {
            byte? cidr;
            InternalToCidr(false, netmask, out cidr);
            return cidr.HasValue ? cidr.Value : (byte) 0;
        }

        /// <summary>
        /// Tries to get the CIDR byte from a net mask.
        /// </summary>
        /// <param name="netmask">The net mask.</param>
        /// <param name="cidr">The CIDR byte.</param>
        /// <returns>true if the operation was performed sucessfully; otherwise, false.</returns>
        public static bool TryToCidr(IPAddress netmask, out byte? cidr)
        {
            byte? cidr2;
            InternalToCidr(true, netmask, out cidr2);
            bool parsed = cidr2 != null;
            cidr = cidr2;
            return parsed;
        }

        /// <summary>
        /// Gets the net mask from a CIDR byte.
        /// </summary>
        /// <param name="cidr">The CIDR byte.</param>
        /// <returns>The net mask.</returns>
        public static IPAddress ToNetmask(byte cidr)
        {
            IPAddress netmask;
            InternalToNetmask(false, cidr, out netmask);
            return netmask;
        }

        /// <summary>
        /// Tries to get the net mask from a CIDR byte.
        /// </summary>
        /// <param name="cidr">The CIDR byte.</param>
        /// <param name="netmask">The net mask.</param>
        /// <returns>true if the operation was performed sucessfully; otherwise, false.</returns>
        public static bool TryToNetmask(byte cidr, out IPAddress netmask)
        {
            IPAddress netmask2;
            InternalToNetmask(true, cidr, out netmask2);
            bool parsed = netmask2 != null;
            netmask = netmask2;
            return parsed;
        }

        /// <summary>
        /// Returns the number of bits set for a net mask.
        /// </summary>
        /// <param name="netmask">The net mask.</param>
        /// <returns>A number of bits set.</returns>
        public static byte BitsSet(IPAddress netmask)
        {
            uint uintNetmask = ToUint(netmask);
            byte bits = BitsSet(uintNetmask);
            return bits;
        }

        /// <summary>
        /// Converts a CIDR byte to a unsigned integer notation.
        /// </summary>
        /// <param name="cidr">The CIDR byte.</param>
        /// <returns>The <see cref="uint"/> value.</returns>
        public static uint ToUint(byte cidr)
        {
            uint? uintNetmask;
            InternalToUint(false, cidr, out uintNetmask);
            return uintNetmask.HasValue ? uintNetmask.Value : 0;
        }

        /// <summary>
        /// Tries to convert a CIDR byte to a unsigned integer notation.
        /// </summary>
        /// <param name="cidr">The CIDR byte.</param>
        /// <param name="uintNetmask">The <see cref="uint"/> value.</param>
        /// <returns>true if the operation was performed sucessfully; otherwise, false.</returns>
        public static bool TryToUint(byte cidr, out uint? uintNetmask)
        {
            uint? uintNetmask2;
            InternalToUint(true, cidr, out uintNetmask2);
            bool parsed = uintNetmask2 != null;
            uintNetmask = uintNetmask2;
            return parsed;
        }

        /// <summary>
        /// Returns whether the net mask is valid.
        /// </summary>
        /// <param name="netmask">The nest mask.</param>
        /// <returns>true if valid; otherwise false.</returns>
        public static bool ValidNetmask(IPAddress netmask)
        {
            if (netmask == null)
            {
                throw new ArgumentNullException("netmask");
            }

            uint uintNetmask = ToUint(netmask);
            bool valid = ValidNetmask(uintNetmask);
            return valid;
        }

        /// <summary>
        /// Converts an unsigned integer IP address notation to an <see cref="IPAddress"/> object.
        /// </summary>
        /// <param name="ipaddress">The <see cref="uint"/> containing the IP address.</param>
        /// <returns>The <see cref="IPAddress"/> instance.</returns>
        public static IPAddress ToIPAddress(uint ipaddress)
        {
            byte[] bytes = BitConverter.GetBytes(ipaddress);
            Array.Reverse(bytes);
            var ip = new IPAddress(bytes);
            return ip;
        }

        /// <summary>
        /// Returns whether the network contains the provided IP address.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <param name="ipaddress">The IP address.</param>
        /// <returns>true if the network contains the IP address; otherwise, false.</returns>
        public static bool Contains(IPNetwork network, IPAddress ipaddress)
        {
            if (network == null)
            {
                throw new ArgumentNullException("network");
            }

            if (ipaddress == null)
            {
                throw new ArgumentNullException("ipaddress");
            }

            uint uintNetwork = network.NetworkAsInt;
            uint uintBroadcast = network.BroadcastAsInt;
            uint uintAddress = ToUint(ipaddress);

            bool contains = uintAddress >= uintNetwork && uintAddress <= uintBroadcast;

            return contains;
        }

        /// <summary>
        /// Returns a value indicating whether the network contains the provided sub-network.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <param name="network2">The sub-network.</param>
        /// <returns>true if the network contains the IP addresses in the sub-network; otherwise, false.</returns>
        public static bool Contains(IPNetwork network, IPNetwork network2)
        {
            if (network == null)
            {
                throw new ArgumentNullException("network");
            }

            if (network2 == null)
            {
                throw new ArgumentNullException("network2");
            }

            uint uintNetwork = network.NetworkAsInt;
            uint uintBroadcast = network.BroadcastAsInt;

            uint uintFirst = network2.NetworkAsInt;
            uint uintLast = network2.BroadcastAsInt;

            bool contains = uintFirst >= uintNetwork && uintLast <= uintBroadcast;

            return contains;
        }

        /// <summary>
        /// Returns a value indicating whether there are overlapped IP addresses in the 2 networks provided.
        /// </summary>
        /// <param name="network">The first network.</param>
        /// <param name="network2">The second network.</param>
        /// <returns>true if there is overlap in addresses; otherwise, false.</returns>
        public static bool Overlap(IPNetwork network, IPNetwork network2)
        {
            if (network == null)
            {
                throw new ArgumentNullException("network");
            }

            if (network2 == null)
            {
                throw new ArgumentNullException("network2");
            }

            uint uintNetwork = network.NetworkAsInt;
            uint uintBroadcast = network.BroadcastAsInt;

            uint uintFirst = network2.NetworkAsInt;
            uint uintLast = network2.BroadcastAsInt;

            bool overlap = (uintFirst >= uintNetwork && uintFirst <= uintBroadcast) ||
                           (uintLast >= uintNetwork && uintLast <= uintBroadcast) ||
                           (uintFirst <= uintNetwork && uintLast >= uintBroadcast) ||
                           (uintFirst >= uintNetwork && uintLast <= uintBroadcast);

            return overlap;
        }

        /// <summary>
        /// Gets the subnet from the network and CIDR notation.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <param name="cidr">The CIDR byte.</param>
        /// <returns>The subnet.</returns>
        public static IPNetworkCollection Subnet(IPNetwork network, byte cidr)
        {
            IPNetworkCollection ipnetworkCollection;
            InternalSubnet(false, network, cidr, out ipnetworkCollection);
            return ipnetworkCollection;
        }

        /// <summary>
        /// Tries to get the subnet from the network and CIDR notation.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <param name="cidr">The CIDR byte.</param>
        /// <param name="ipnetworkCollection">The subnet.</param>
        /// <returns>true if the subnet was calculated successfully; otherwise, false.</returns>
        public static bool TrySubnet(IPNetwork network, byte cidr, out IPNetworkCollection ipnetworkCollection)
        {
            IPNetworkCollection inc;
            InternalSubnet(true, network, cidr, out inc);

            if (inc == null)
            {
                ipnetworkCollection = null;
                return false;
            }

            ipnetworkCollection = inc;
            return true;
        }

        /// <summary>
        /// Gets the supernet for the two networks.
        /// </summary>
        /// <param name="network1">The first network.</param>
        /// <param name="network2">The second network.</param>
        /// <returns>The supernet.</returns>
        public static IPNetwork Supernet(IPNetwork network1, IPNetwork network2)
        {
            IPNetwork supernet;
            InternalSupernet(false, network1, network2, out supernet);
            return supernet;
        }

        /// <summary>
        /// Tries to get the supernet for the two networks.
        /// </summary>
        /// <param name="network1">The first network.</param>
        /// <param name="network2">The second network.</param>
        /// <param name="supernet">The supernet.</param>
        /// <returns>true if the supernet exists; otherwise, false.</returns>
        public static bool TrySupernet(IPNetwork network1, IPNetwork network2, out IPNetwork supernet)
        {
            IPNetwork outSupernet;
            InternalSupernet(true, network1, network2, out outSupernet);
            bool parsed = outSupernet != null;
            supernet = outSupernet;
            return parsed;
        }

        /// <summary>
        /// Tries to guess CIDR byte from an IP notation.
        /// </summary>
        /// <param name="ip">A <see cref="string"/> IP notation.</param>
        /// <param name="cidr">The CIDR byte.</param>
        /// <returns>True if the CIDR was guessed correctly; otherwise, false.</returns>
        public static bool TryGuessCidr(string ip, out byte cidr)
        {
            IPAddress ipaddress;
            bool parsed = IPAddress.TryParse(String.Format(CultureInfo.InvariantCulture, "{0}", ip), out ipaddress);

            if (parsed == false)
            {
                cidr = 0;
                return false;
            }

            uint uintIPAddress = ToUint(ipaddress);
            uintIPAddress = uintIPAddress >> 29;

            if (uintIPAddress <= 3)
            {
                cidr = 8;
                return true;
            }

            if (uintIPAddress <= 5)
            {
                cidr = 16;
                return true;
            }

            if (uintIPAddress <= 6)
            {
                cidr = 24;
                return true;
            }

            cidr = 0;
            return false;
        }

        /// <summary>
        /// Tries to parse CIDR byte from string notation..
        /// </summary>
        /// <param name="sidr">A <see cref="string"/> CIDR notation.</param>
        /// <param name="cidr">The CIDR byte.</param>
        /// <returns>True if the CIDR was parsed correctly; otherwise, false.</returns>
        public static bool TryParseCidr(string sidr, out byte? cidr)
        {
            byte b;

            if (!byte.TryParse(sidr, out b))
            {
                cidr = null;
                return false;
            }

            IPAddress netmask;

            if (!TryToNetmask(b, out netmask))
            {
                cidr = null;
                return false;
            }

            cidr = b;
            return true;
        }

        /// <summary>
        /// Lists all IP addresses for the network.
        /// </summary>
        /// <param name="ipnetwork">The network.</param>
        /// <returns>A collection of IP addresses.</returns>
        public static IPAddressCollection ListIPAddress(IPNetwork ipnetwork)
        {
            return new IPAddressCollection(ipnetwork);
        }

        /// <summary>
        /// Returns a network that spans IP addresses from <paramref name="start"/> to <paramref name="end"/>.
        /// </summary>
        /// <param name="start">The first IP address.</param>
        /// <param name="end">The last IP address.</param>
        /// <returns>The <see cref="IPNetwork"/> instance that spans the provided addresses.</returns>
        public static IPNetwork WideSubnet(string start, string end)
        {
            if (string.IsNullOrEmpty(start))
            {
                throw new ArgumentNullException("start");
            }

            if (string.IsNullOrEmpty(end))
            {
                throw new ArgumentNullException("end");
            }

            IPAddress startIP;
            if (!IPAddress.TryParse(start, out startIP))
            {
                throw new ArgumentOutOfRangeException("start");
            }

            IPAddress endIP;
            if (!IPAddress.TryParse(end, out endIP))
            {
                throw new ArgumentOutOfRangeException("end");
            }

            var ipnetwork = new IPNetwork(0, 0);

            for (byte cidr = 32; cidr >= 0; cidr--)
            {
                IPNetwork wideSubnet = Parse(start, cidr);
                if (!Contains(wideSubnet, endIP))
                {
                    continue;
                }

                ipnetwork = wideSubnet;
                break;
            }

            return ipnetwork;
        }

        /// <summary>
        /// Tries to return a network that spans the provided networks.
        /// </summary>
        /// <param name="ipnetworks">An array of networks.</param>
        /// <param name="ipnetwork">The <see cref="IPNetwork"/> instance that spans the provided addresses.</param>
        /// <returns>true if wide subnet was calculated successfully; otherwise, false.</returns>
        public static bool TryWideSubnet(IPNetwork[] ipnetworks, out IPNetwork ipnetwork)
        {
            IPNetwork ipn;
            InternalWideSubnet(true, ipnetworks, out ipn);

            if (ipn == null)
            {
                ipnetwork = null;
                return false;
            }

            ipnetwork = ipn;
            return true;
        }

        /// <summary>
        /// Returns a wide subnet that spans the provided networks.
        /// </summary>
        /// <param name="ipnetworks">An array of networks.</param>
        /// <returns>The <see cref="IPNetwork"/> instance that spans the provided networks.</returns>
        public static IPNetwork WideSubnet(IPNetwork[] ipnetworks)
        {
            IPNetwork ipn;
            InternalWideSubnet(false, ipnetworks, out ipn);
            return ipn;
        }

        /// <summary>
        /// Determines whether the specified <see cref="IPNetwork"/> instances are considered equal.
        /// </summary>
        /// <param name="obj">The <see cref="IPNetwork"/> to compare with the current <see cref="IPNetwork"/>.</param>
        /// <returns>true if the specified System.Object is equal to the current System.Object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var ipNetwork = obj as IPNetwork;

            if (ipNetwork == null)
            {
                return false;
            }

            if (NetworkAsInt != ipNetwork.NetworkAsInt)
            {
                return false;
            }

            return m_cidr == ipNetwork.m_cidr;
        }

        /// <summary>
        /// Serves as a hash function for the <see cref="IPNetwork"/> type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="IPNetwork"/>.</returns>
        public override int GetHashCode()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}", m_ipaddress.GetHashCode(), NetworkAsInt.GetHashCode(), m_cidr.GetHashCode()).GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="IPNetwork"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the current<see cref="IPNetwork"/>.</returns>
        public override string ToString()
        {
            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                writer.WriteLine("IPNetwork   : {0}", String.Format(CultureInfo.InvariantCulture, "{0}/{1}", Network, Cidr));
                writer.WriteLine("Network     : {0}", Network);
                writer.WriteLine("Netmask     : {0}", Netmask);
                writer.WriteLine("Cidr        : {0}", Cidr);
                writer.WriteLine("Broadcast   : {0}", Broadcast);
                writer.WriteLine("FirstUsable : {0}", FirstUsable);
                writer.WriteLine("LastUsable  : {0}", LastUsable);
                writer.WriteLine("Usable      : {0}", Usable);

                return writer.ToString();
            }
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has the following meanings: Value Meaning Less than zero
        /// This object is less than the other parameter.Zero This object is equal to
        /// other. Greater than zero This object is greater than other.
        /// </returns>
        public int CompareTo(IPNetwork other)
        {
            if (other == null)
            {
                return 1;
            }

            int network = NetworkAsInt.CompareTo(other.NetworkAsInt);

            if (network != 0)
            {
                return network;
            }

            return m_cidr.CompareTo(other.m_cidr);
        }

        private static void InternalParse(bool tryParse, string network, out IPNetwork ipnetwork)
        {
            if (string.IsNullOrEmpty(network))
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("network");
                }

                ipnetwork = null;
                return;
            }

            network = Regex.Replace(network, @"[^0-9\.\/\s]+", String.Empty);
            network = Regex.Replace(network, @"\s{2,}", " ");
            network = network.Trim();

            string[] args = network.Split(new[] { ' ', '/' });
            byte cidr;

            if (args.Length == 1)
            {
                if (TryGuessCidr(args[0], out cidr))
                {
                    InternalParse(tryParse, args[0], cidr, out ipnetwork);
                    return;
                }

                if (tryParse == false)
                {
                    throw new ArgumentOutOfRangeException("network");
                }

                ipnetwork = null;
                return;
            }

            if (byte.TryParse(args[1], out cidr))
            {
                InternalParse(tryParse, args[0], cidr, out ipnetwork);
                return;
            }

            InternalParse(tryParse, args[0], args[1], out ipnetwork);
        }

        private static void InternalParse(bool tryParse, string ipaddress, string netmask, out IPNetwork ipnetwork)
        {
            if (string.IsNullOrEmpty(ipaddress))
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("ipaddress");
                }

                ipnetwork = null;
                return;
            }

            if (string.IsNullOrEmpty(netmask))
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("netmask");
                }

                ipnetwork = null;
                return;
            }

            IPAddress ip;
            bool ipaddressParsed = IPAddress.TryParse(ipaddress, out ip);

            if (ipaddressParsed == false)
            {
                if (tryParse == false)
                {
                    throw new ArgumentOutOfRangeException("ipaddress");
                }
                ipnetwork = null;
                return;
            }

            IPAddress mask;
            bool netmaskParsed = IPAddress.TryParse(netmask, out mask);

            if (netmaskParsed == false)
            {
                if (tryParse == false)
                {
                    throw new ArgumentOutOfRangeException("netmask");
                }
                ipnetwork = null;
                return;
            }

            InternalParse(tryParse, ip, mask, out ipnetwork);
        }

        private static void InternalParse(bool tryParse, IPAddress ipaddress, IPAddress netmask, out IPNetwork ipnetwork)
        {
            if (ipaddress == null)
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("ipaddress");
                }
                ipnetwork = null;
                return;
            }

            if (netmask == null)
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("netmask");
                }
                ipnetwork = null;
                return;
            }

            uint uintIPAddress = ToUint(ipaddress);
            byte? cidr2;
            bool parsed = TryToCidr(netmask, out cidr2);
            if (parsed == false)
            {
                if (tryParse == false)
                {
                    throw new ArgumentOutOfRangeException("netmask");
                }
                ipnetwork = null;
                return;
            }

            byte cidr = cidr2.HasValue ? cidr2.Value : (byte) 0;

            var ipnet = new IPNetwork(uintIPAddress, cidr);
            ipnetwork = ipnet;
        }

        private static void InternalParse(bool tryParse, string ipaddress, byte cidr, out IPNetwork ipnetwork)
        {
            if (string.IsNullOrEmpty(ipaddress))
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("ipaddress");
                }
                ipnetwork = null;
                return;
            }

            IPAddress ip;
            bool ipaddressParsed = IPAddress.TryParse(ipaddress, out ip);

            if (ipaddressParsed == false)
            {
                if (tryParse == false)
                {
                    throw new ArgumentOutOfRangeException("ipaddress");
                }
                ipnetwork = null;
                return;
            }

            IPAddress mask;
            bool parsedNetmask = TryToNetmask(cidr, out mask);
            if (parsedNetmask == false)
            {
                if (tryParse == false)
                {
                    throw new ArgumentOutOfRangeException("cidr");
                }
                ipnetwork = null;
                return;
            }

            InternalParse(tryParse, ip, mask, out ipnetwork);
        }

        private static void InternalSubnet(bool trySubnet, IPNetwork network, byte cidr, out IPNetworkCollection ipnetworkCollection)
        {
            if (network == null)
            {
                if (trySubnet == false)
                {
                    throw new ArgumentNullException("network");
                }
                ipnetworkCollection = null;
                return;
            }

            if (cidr > 32)
            {
                if (trySubnet == false)
                {
                    throw new ArgumentOutOfRangeException("cidr");
                }
                ipnetworkCollection = null;
                return;
            }

            if (cidr < network.Cidr)
            {
                if (trySubnet == false)
                {
                    throw new ArgumentOutOfRangeException("cidr");
                }
                ipnetworkCollection = null;
                return;
            }

            ipnetworkCollection = new IPNetworkCollection(network, cidr);
        }

        private static void InternalSupernet(bool trySupernet, IPNetwork network1, IPNetwork network2, out IPNetwork supernet)
        {
            if (network1 == null)
            {
                if (trySupernet == false)
                {
                    throw new ArgumentNullException("network1");
                }
                supernet = null;
                return;
            }

            if (network2 == null)
            {
                if (trySupernet == false)
                {
                    throw new ArgumentNullException("network2");
                }
                supernet = null;
                return;
            }

            if (Contains(network1, network2))
            {
                supernet = new IPNetwork(network1.NetworkAsInt, network1.Cidr);
                return;
            }

            if (Contains(network2, network1))
            {
                supernet = new IPNetwork(network2.NetworkAsInt, network2.Cidr);
                return;
            }

            if (network1.m_cidr != network2.m_cidr)
            {
                if (trySupernet == false)
                {
                    throw new ArgumentOutOfRangeException("network1");
                }
                supernet = null;
                return;
            }

            IPNetwork first = (network1.NetworkAsInt < network2.NetworkAsInt) ? network1 : network2;
            IPNetwork last = (network1.NetworkAsInt > network2.NetworkAsInt) ? network1 : network2;

            if ((first.BroadcastAsInt + 1) != last.NetworkAsInt)
            {
                if (trySupernet == false)
                {
                    throw new ArgumentOutOfRangeException("network1");
                }
                supernet = null;
                return;
            }

            uint uintSupernet = first.NetworkAsInt;
            var cidrSupernet = (byte) (first.m_cidr - 1);

            var networkSupernet = new IPNetwork(uintSupernet, cidrSupernet);

            if (networkSupernet.NetworkAsInt != first.NetworkAsInt)
            {
                if (trySupernet == false)
                {
                    throw new ArgumentOutOfRangeException("network2");
                }
                supernet = null;
                return;
            }

            supernet = networkSupernet;
        }

        private static void InternalToUint(bool tryParse, IPAddress ipaddress, out uint? uintIPAddress)
        {
            if (ipaddress == null)
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("ipaddress");
                }

                uintIPAddress = null;
                return;
            }

            byte[] bytes = ipaddress.GetAddressBytes();

            if (bytes.Length != 4)
            {
                if (tryParse == false)
                {
                    throw new ArgumentOutOfRangeException("ipaddress");
                }

                uintIPAddress = null;
                return;
            }

            Array.Reverse(bytes);
            uint value = BitConverter.ToUInt32(bytes, 0);
            uintIPAddress = value;
        }

        private static Stack<IPNetwork> List2Stack(List<IPNetwork> list)
        {
            var stack = new Stack<IPNetwork>();
            list.ForEach(stack.Push);
            return stack;
        }

        private static List<IPNetwork> Array2List(IEnumerable<IPNetwork> array)
        {
            var ipns = new List<IPNetwork>();
            ipns.AddRange(array);
            RemoveNull(ipns);
            ipns.Sort((ipn1, ipn2) =>
            {
                int networkCompare = ipn1.NetworkAsInt.CompareTo(ipn2.NetworkAsInt);
                if (networkCompare == 0)
                {
                    int cidrCompare = ipn1.m_cidr.CompareTo(ipn2.m_cidr);
                    return cidrCompare;
                }
                return networkCompare;
            });
            ipns.Reverse();

            return ipns;
        }

        private static void RemoveNull(List<IPNetwork> ipns)
        {
            ipns.RemoveAll(ipn => ipn == null);
        }

        private static void InternalWideSubnet(bool tryWide, IPNetwork[] ipnetworks, out IPNetwork ipnetwork)
        {
            if (ipnetworks == null)
            {
                if (tryWide == false)
                {
                    throw new ArgumentNullException("ipnetworks");
                }

                ipnetwork = null;
                return;
            }

            IPNetwork[] nnin = Array.FindAll(ipnetworks, ipnet => ipnet != null);

            if (nnin.Length <= 0)
            {
                if (tryWide == false)
                {
                    throw new ArgumentOutOfRangeException("ipnetworks");
                }

                ipnetwork = null;
                return;
            }

            if (nnin.Length == 1)
            {
                IPNetwork ipn0 = nnin[0];
                ipnetwork = ipn0;
                return;
            }

            Array.Sort(nnin);
            IPNetwork nnin0 = nnin[0];
            uint uintNnin0 = nnin0.m_ipaddress;

            IPNetwork nninX = nnin[nnin.Length - 1];
            IPAddress ipaddressX = nninX.Broadcast;

            var ipn = new IPNetwork(0, 0);

            for (byte cidr = nnin0.m_cidr; cidr >= 0; cidr--)
            {
                var wideSubnet = new IPNetwork(uintNnin0, cidr);

                if (!Contains(wideSubnet, ipaddressX))
                {
                    continue;
                }

                ipn = wideSubnet;
                break;
            }

            ipnetwork = ipn;
        }

        private static void InternalToUint(bool tryParse, byte cidr, out uint? uintNetmask)
        {
            if (cidr > 32)
            {
                if (tryParse == false)
                {
                    throw new ArgumentOutOfRangeException("cidr");
                }
                uintNetmask = null;
                return;
            }
            uint uintNetmask2 = cidr == 0 ? 0 : 0xffffffff << (32 - cidr);
            uintNetmask = uintNetmask2;
        }

        private static byte ToCidr(uint netmask)
        {
            byte? cidr;
            InternalToCidr(false, netmask, out cidr);
            return cidr.HasValue ? cidr.Value : (byte) 0;
        }

        private static void InternalToCidr(bool tryParse, uint netmask, out byte? cidr)
        {
            if (!ValidNetmask(netmask))
            {
                if (tryParse == false)
                {
                    throw new ArgumentOutOfRangeException("netmask");
                }

                cidr = null;
                return;
            }

            byte cidr2 = BitsSet(netmask);
            cidr = cidr2;
        }

        private static void InternalToCidr(bool tryParse, IPAddress netmask, out byte? cidr)
        {
            if (netmask == null)
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("netmask");
                }
                cidr = null;
                return;
            }

            uint? uintNetmask2;
            bool parsed = TryToUint(netmask, out uintNetmask2);

            if (parsed == false)
            {
                if (tryParse == false)
                {
                    throw new ArgumentOutOfRangeException("netmask");
                }
                cidr = null;
                return;
            }

            uint uintNetmask = uintNetmask2.HasValue ? uintNetmask2.Value : 0;

            byte? cidr2;
            InternalToCidr(tryParse, uintNetmask, out cidr2);
            cidr = cidr2;
        }

        private static void InternalToNetmask(bool tryParse, byte cidr, out IPAddress netmask)
        {
            if (cidr > 32)
            {
                if (tryParse == false)
                {
                    throw new ArgumentOutOfRangeException("cidr");
                }
                netmask = null;
                return;
            }
            uint mask = ToUint(cidr);
            IPAddress netmask2 = ToIPAddress(mask);
            netmask = netmask2;
        }

        private static byte BitsSet(uint netmask)
        {
            uint i = netmask;
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            i = ((i + (i >> 4) & 0xf0f0f0f) * 0x1010101) >> 24;

            return (byte) i;
        }

        private static bool ValidNetmask(uint netmask)
        {
            long neg = (~(int) netmask) & 0xffffffff;
            bool isNetmask = ((neg + 1) & neg) == 0;
            return isNetmask;
        }
    }
}