using System;
using RestFoundation.Runtime;

namespace RestFoundation
{
    public sealed class ContentTypeFormatterBuilder
    {
        internal ContentTypeFormatterBuilder()
        {
        }

        public IContentTypeFormatter Get(string contentType)
        {
            if (String.IsNullOrEmpty(contentType)) throw new ArgumentNullException("contentType");

            return ContentTypeFormatterRegistry.GetFormatter(contentType);
        }

        public void Set(string contentType, IContentTypeFormatter formatter)
        {
            if (formatter == null) throw new ArgumentNullException("formatter");
            if (String.IsNullOrEmpty(contentType)) throw new ArgumentNullException("contentType");

            ContentTypeFormatterRegistry.SetFormatter(contentType, formatter);
        }

        public bool Remove(string contentType)
        {
            if (String.IsNullOrEmpty(contentType)) throw new ArgumentNullException("contentType");

            return ContentTypeFormatterRegistry.RemoveFormatter(contentType);
        }

        public void Clear()
        {
            ContentTypeFormatterRegistry.Clear();
        }
    }
}
