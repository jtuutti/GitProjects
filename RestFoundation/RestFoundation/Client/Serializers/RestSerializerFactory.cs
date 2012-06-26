﻿using System;

namespace RestFoundation.Client.Serializers
{
    public class RestSerializerFactory : IRestSerializerFactory
    {
        public IRestSerializer Create(Type objectType, RestResourceType resourceType)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");

            if (objectType == typeof(String)) return new StringSerializer();

            switch (resourceType)
            {
                case RestResourceType.Json:
                    return new JsonObjectSerializer();
                case RestResourceType.Xml:
                    return new XmlObjectSerializer();
            }

            throw new NotSupportedException();
        }
    }
}