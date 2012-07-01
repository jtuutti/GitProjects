using System.IO;

namespace RestFoundation
{
    public interface IUploadedFile
    {
        string ContentType { get; }
        int ContentLength { get; }
        string Name { get; }

        Stream Data { get; }

        void SaveAs(string fileName);
        byte[] ReadAsByteArray();
        string ReadAsString();
    }
}
