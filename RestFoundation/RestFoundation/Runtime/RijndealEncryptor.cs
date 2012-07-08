using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RestFoundation.Runtime
{
    internal sealed class RijndaelEncryptor
    {
        private const string InvalidHashKey = "Invalid hash key provided.";

        private static readonly byte[] hashBuffer = new byte[8];

        private readonly byte[] key;
        private readonly byte[] vector;

        static RijndaelEncryptor()
        {
            using (RandomNumberGenerator randomGenerator = RNGCryptoServiceProvider.Create())
            {
                randomGenerator.GetBytes(hashBuffer);
            }
        }

        public RijndaelEncryptor()
        {
            key = new byte[32];
            vector = new byte[16];

            using (var sha = new SHA384Managed())
            {
                byte[] hashArray = sha.ComputeHash(hashBuffer);

                Array.Copy(hashArray, 0, key, 0, 32);
                Array.Copy(hashArray, 32, vector, 0, 16);
            }
        }

        public string Encrypt(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (key == null) throw new InvalidOperationException(InvalidHashKey);

            byte[] data = Encoding.UTF8.GetBytes(value);

            using (var crypto = new RijndaelManaged())
            using (var encryptor = crypto.CreateEncryptor(key, vector))
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
            if (String.IsNullOrEmpty(encryptedValue)) throw new ArgumentNullException("encryptedValue");
            if (key == null) throw new InvalidOperationException(InvalidHashKey);

            byte[] cipher = Convert.FromBase64String(encryptedValue);

            using (var crypto = new RijndaelManaged())
            using (ICryptoTransform encryptor = crypto.CreateDecryptor(key, vector))
            using (var memoryStream = new MemoryStream(cipher))
            {
                var crptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Read);

                var data = new byte[cipher.Length];
                int dataLength = crptoStream.Read(data, 0, data.Length);

                return Encoding.UTF8.GetString(data, 0, dataLength);
            }
        }
    }
}
