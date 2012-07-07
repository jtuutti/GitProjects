using System;
using System.Globalization;
using System.IO;

namespace RestFoundation.Runtime
{
    public class HttpResponseOutput : ContextBase, IHttpResponseOutput
    {
        private const string LineBreak = "<br/>";

        public Stream Stream
        {
            get
            {
                return Context.Response.OutputStream;
            }
        }

        public TextWriter Writer
        {
            get
            {
                return Context.Response.Output;
            }
        }

        public Stream Filter
        {
            get
            {
                return Context.Response.Filter;
            }
            set
            {
                Context.Response.Filter = value;
            }
        }

        public void Flush()
        {
            Context.Response.Flush();
        }

        public void Clear()
        {
            Context.Response.Clear();
        }

        public IHttpResponseOutput Write(string value)
        {
            Context.Response.Write(value);
            return this;
        }

        public IHttpResponseOutput Write(object obj)
        {
            Context.Response.Write(obj);
            return this;
        }

        public IHttpResponseOutput WriteLine()
        {
            Context.Response.Write(LineBreak);
            return this;
        }

        public IHttpResponseOutput WriteLine(string value)
        {
            Context.Response.Write(value);
            Context.Response.Write(LineBreak);
            return this;
        }

        public IHttpResponseOutput WriteLine(byte times)
        {
            for (byte i = 0; i < times; i++)
            {
                WriteLine();
            }

            return this;
        }

        public IHttpResponseOutput WriteFormat(string format, params object[] values)
        {
            if (format == null) throw new ArgumentNullException("format");

            Context.Response.Write(String.Format(CultureInfo.InvariantCulture, format, values));
            return this;
        }

        public IHttpResponseOutput WriteFormat(IFormatProvider provider, string format, params object[] values)
        {
            if (format == null) throw new ArgumentNullException("format");

            Context.Response.Write(String.Format(provider, format, values));
            return this;
        }
    }
}
