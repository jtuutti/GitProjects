using System;
using System.IO;
using System.Web;

namespace RestFoundation.Runtime
{
    internal sealed class UploadedFile : IUploadedFile
    {
        private readonly HttpPostedFile m_file;

        public UploadedFile(HttpPostedFile file)
        {
            if (file == null) throw new ArgumentNullException("file");

            m_file = file;
        }

        public string ContentType
        {
            get
            {
                return m_file.ContentType;
            }
        }

        public int ContentLength
        {
            get
            {
                return m_file.ContentLength;
            }
        }

        public string Name
        {
            get
            {
                return m_file.FileName;
            }
        }

        public Stream Data
        {
            get
            {
                return m_file.InputStream;
            }
        }

        public void SaveAs(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");

            if (fileName.Trim().Length == 0)
            {
                throw new ArgumentException("File name cannot be empty", "fileName");
            }

            m_file.SaveAs(fileName);
        }

        public byte[] ReadAsByteArray()
        {
            SeekToBeginning();

            using (var memoryStream = new MemoryStream())
            {
                m_file.InputStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public string ReadAsString()
        {
            SeekToBeginning();

            using (var memoryStream = new MemoryStream())
            {
                m_file.InputStream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                var reader = new StreamReader(memoryStream);
                return reader.ReadToEnd();
            }
        }

        private void SeekToBeginning()
        {
            if (m_file.InputStream.CanSeek)
            {
                m_file.InputStream.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}
