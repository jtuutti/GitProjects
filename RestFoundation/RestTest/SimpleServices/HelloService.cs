﻿using System;
using System.Collections.Generic;
using System.Net;
using RestFoundation;
using RestFoundation.Results;
using RestFoundation.Runtime;
using RestFoundation.ServiceProxy;

namespace RestTest.SimpleServices
{
    [ServiceContract]
    public class HelloService : ProxyMetadata<HelloService>
    {
        private readonly Dictionary<string, string> resultLanguageMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "en", "Hello world" },
            { "es", "Hola mundo" },
            { "ru", "Привет мир" },
        };

        [Url(Url.Root)]
        public ContentResult Get(IHttpRequest request)
        {
            if (request.Headers.AcceptLanguageCulture == null)
            {
                return Result.Content(resultLanguageMap["en"], true, "text/plain");
            }

            string value;

            if (!resultLanguageMap.TryGetValue(request.Headers.AcceptLanguageCulture.TwoLetterISOLanguageName, out value))
            {
                throw new HttpResponseException(HttpStatusCode.NotImplemented, "Unsupported Language");
            }

            return Result.Content(value, true, "text/plain");
        }

        public override void Initialize()
        {
            ForMethod(x => x.Get(Arg<IHttpRequest>())).SetDescription("Prints 'Hello world!'")
                                                      .SetResponseResourceExample("Hello world!");
        }
    }
}
