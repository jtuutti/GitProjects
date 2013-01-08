// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RestFoundation.Security
{
    internal sealed class RijndaelEncryptor
    {
        private static readonly byte[] hashBuffer = CreateHashBuffer();

        private readonly byte[] m_key;
        private readonly byte[] m_vector;

        public RijndaelEncryptor()
        {
            m_key = new byte[32];
            m_vector = new byte[16];

            using (var sha = new SHA384Managed())
            {
                byte[] hashArray = sha.ComputeHash(hashBuffer);

                Array.Copy(hashArray, 0, m_key, 0, 32);
                Array.Copy(hashArray, 32, m_vector, 0, 16);
            }
        }

        public string Encrypt(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (m_key == null)
            {
                throw new InvalidOperationException(RestResources.InvalidHashKey);
            }

            byte[] data = Encoding.UTF8.GetBytes(value);

            using (var crypto = new RijndaelManaged())
            using (var encryptor = crypto.CreateEncryptor(m_key, m_vector))
            using (var memoryStream = new MemoryStream())
            {
                var crptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                crptoStream.Write(data, 0, data.Length);
                crptoStream.FlushFinalBlock();

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public string Decrypt(string encryptedValue)
        {
            if (String.IsNullOrEmpty(encryptedValue))
            {
                throw new ArgumentNullException("encryptedValue");
            }

            if (m_key == null)
            {
                throw new InvalidOperationException(RestResources.InvalidHashKey);
            }

            byte[] cipher = Convert.FromBase64String(encryptedValue);

            using (var crypto = new RijndaelManaged())
            using (ICryptoTransform encryptor = crypto.CreateDecryptor(m_key, m_vector))
            using (var memoryStream = new MemoryStream(cipher))
            {
                var crptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Read);

                var data = new byte[cipher.Length];
                int dataLength = crptoStream.Read(data, 0, data.Length);

                return Encoding.UTF8.GetString(data, 0, dataLength);
            }
        }

        private static byte[] CreateHashBuffer()
        {
            var buffer = new byte[8];

            using (RandomNumberGenerator randomGenerator = RNGCryptoServiceProvider.Create())
            {
                randomGenerator.GetBytes(buffer);
            }

            return buffer;
        }
    }
}
