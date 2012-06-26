using System;
using System.IO;
using System.Text;

namespace RestFoundation.Client.Serializers
{
    public class StringSerializer : IRestSerializer
    {
        public int GetContentLength(object obj)
        {
            if (obj == null) return 0;
            if (!(obj is String)) throw new ArgumentOutOfRangeException("obj");

            return obj.ToString().Length;
        }

        public void Serialize(Stream stream, object obj)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (obj == null) return;
            if (!(obj is String)) throw new ArgumentOutOfRangeException("obj");

            byte[] data = Encoding.UTF8.GetBytes(obj.ToString());
            stream.Write(data, 0, data.Length);
        }

        public T Deserialize<T>(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (typeof(T) != typeof(String)) throw new InvalidOperationException("The output type is not a String.");

            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                object serializedObject = reader.ReadToEnd();
                return (T) serializedObject;
            }
        }
    }
}
