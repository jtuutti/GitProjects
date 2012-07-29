using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RestFoundation.Security
{
    internal sealed class MD5Encoder : IDisposable
    {
        private readonly MD5 m_hash;
        private bool m_isDisposed;

        public MD5Encoder()
        {
            m_hash = MD5.Create();
        }

        public string Encode(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return ConvertToHexString(m_hash.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }

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
