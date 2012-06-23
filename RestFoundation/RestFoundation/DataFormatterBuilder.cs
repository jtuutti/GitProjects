using System;
using RestFoundation.DataFormatters;

namespace RestFoundation
{
    public sealed class DataFormatterBuilder
    {
        internal DataFormatterBuilder()
        {
        }

        public IDataFormatter GetByContentType(string contentType)
        {
            if (String.IsNullOrEmpty(contentType)) throw new ArgumentNullException("contentType");

            return DataFormatterRegistry.GetFormatter(contentType);
        }

        public IDataFormatter GetByResourceType(Type resourceType)
        {
            if (resourceType == null) throw new ArgumentNullException("resourceType");

            return DataFormatterRegistry.GetFormatter(resourceType);
        }

        public void SetForContentType(string contentType, IDataFormatter formatter)
        {
            if (formatter == null) throw new ArgumentNullException("formatter");
            if (String.IsNullOrEmpty(contentType)) throw new ArgumentNullException("contentType");

            DataFormatterRegistry.SetFormatter(contentType, formatter);
        }

        public void SetForResourceType(Type resourceType, IDataFormatter formatter)
        {
            if (resourceType == null) throw new ArgumentNullException("resourceType");
            if (formatter == null) throw new ArgumentNullException("formatter");

            DataFormatterRegistry.SetFormatter(resourceType, formatter);
        }

        public bool RemoveByContentType(string contentType)
        {
            if (String.IsNullOrEmpty(contentType)) throw new ArgumentNullException("contentType");

            return DataFormatterRegistry.RemoveFormatter(contentType);
        }

        public bool RemoveByResourceType(Type resourceType)
        {
            if (resourceType == null) throw new ArgumentNullException("resourceType");

            return DataFormatterRegistry.RemoveFormatter(resourceType);
        }

        public void Clear()
        {
            Clear(DataFormatterType.ContentType | DataFormatterType.ResourceType);
        }

        public void Clear(DataFormatterType formatterTypes)
        {
            if ((formatterTypes & DataFormatterType.ContentType) == DataFormatterType.ContentType)
            {
                DataFormatterRegistry.ClearContentTypeFormatters();
            }

            if ((formatterTypes & DataFormatterType.ResourceType) == DataFormatterType.ResourceType)
            {
                DataFormatterRegistry.ClearResourceTypeFormatters();
            }
        }
    }
}
