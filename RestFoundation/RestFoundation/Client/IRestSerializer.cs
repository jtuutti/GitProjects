using System.IO;

namespace RestFoundation.Client
{
    public interface IRestSerializer
    {
        int GetContentLength(object obj);
        void Serialize(Stream stream, object obj);
        T Deserialize<T>(Stream stream);
    }
}
