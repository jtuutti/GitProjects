// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RestFoundation.Security
{
    /// <summary>
    /// Represents an MD5 encoder.
    /// </summary>
    public sealed class MD5Encoder : IDisposable
    {
        private readonly MD5 m_hash;
        private bool m_isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="MD5Encoder"/> class.
        /// </summary>
        public MD5Encoder()
        {
            m_hash = MD5.Create();
        }

        /// <summary>
        /// Encodes a value into a hexidecimal string representationo of its MD5 hash.
        /// </summary>
        /// <param name="value">A <see cref="String"/> containing the value.</param>
        /// <returns>The encoded representation of the string.</returns>
        public string Encode(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return ConvertToHexString(m_hash.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (m_isDisposed)
            {
                return;
            }

            m_hash.Dispose();
            m_isDisposed = true;
        }

        private static string ConvertToHexString(IEnumerable<byte> hasedValue)
        {
            var hexString = new StringBuilder();

            foreach (byte byteFromHash in hasedValue)
            {
                hexString.AppendFormat("{0:x2}", byteFromHash);
            }

            return hexString.ToString();
        }
    }
}
