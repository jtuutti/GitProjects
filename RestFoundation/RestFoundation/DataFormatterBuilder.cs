using System;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public sealed class DataFormatterBuilder
    {
        internal DataFormatterBuilder()
        {
        }

        public IDataFormatter Get(string contentType)
        {
            if (String.IsNullOrEmpty(contentType)) throw new ArgumentNullException("contentType");

            return DataFormatterRegistry.GetFormatter(contentType);
        }

        public void Set(string contentType, IDataFormatter formatter)
        {
            if (formatter == null) throw new ArgumentNullException("formatter");
            if (String.IsNullOrEmpty(contentType)) throw new ArgumentNullException("contentType");

            DataFormatterRegistry.SetFormatter(contentType, formatter);
        }

        public bool Remove(string contentType)
        {
            if (String.IsNullOrEmpty(contentType)) throw new ArgumentNullException("contentType");

            return DataFormatterRegistry.RemoveFormatter(contentType);
        }

        public void Clear()
        {
            DataFormatterRegistry.Clear();
        }
    }
}
