using System;
using System.IO;

namespace RestFoundation.Test
{
    public class MockHttpResponseOutput : IHttpResponseOutput
    {
        public MockHttpResponseOutput()
        {
            Stream = Console.OpenStandardInput();
            Writer = Console.Out;
            Filter = new MemoryStream();
        }

        public virtual Stream Stream { get; set; }
        public virtual TextWriter Writer { get; set; }
        public virtual Stream Filter { get; set; }

        public virtual void Flush()
        {
            Writer.Flush();
        }

        public virtual void Clear()
        {
        }

        public virtual IHttpResponseOutput Write(string value)
        {
            if (value == null) throw new ArgumentNullException("value");

            Console.Write(value);
            return this;
        }

        public virtual IHttpResponseOutput Write(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            Console.Write(obj);
            return this;
        }

        public virtual IHttpResponseOutput WriteLine()
        {
            Console.WriteLine();
            return this;
        }

        public virtual IHttpResponseOutput WriteLine(string value)
        {
            if (value == null) throw new ArgumentNullException("value");

            Console.WriteLine(value);
            return this;
        }

        public virtual IHttpResponseOutput WriteLine(byte times)
        {
            for (byte i = 0; i < times; i++)
            {
                Console.WriteLine();
            }
            return this;
        }

        public virtual IHttpResponseOutput WriteFormat(string format, params object[] values)
        {
            return WriteFormat(null, format, values);
        }

        public virtual IHttpResponseOutput WriteFormat(IFormatProvider provider, string format, params object[] values)
        {
            if (format == null) throw new ArgumentNullException("format");

            Console.Write(format, values);
            return this;
        }
    }
}
