using System;
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
            string value;

            if (request.Headers.AcceptLanguageCulture == null)
            {
                value = resultLanguageMap["en"];
            }
            else if (!resultLanguageMap.TryGetValue(request.Headers.AcceptLanguageCulture.TwoLetterISOLanguageName, out value))
            {
                throw new HttpResponseException(HttpStatusCode.NotImplemented, "Unsupported Language");
            }

            return Result.Content(value, true, "text/plain");
        }

        public override void Initialize()
        {
            SetAuthentication(AuthenticationType.HMAC, "RestTest", "~/secure-hello");

            ForMethod(x => x.Get(Arg<IHttpRequest>())).SetAuthentication(AuthenticationType.HMAC,
                                                                         "RestTest",
                                                                         "~/secure-hello",
                                                                         "HMAC username=\"RestTest\", sig=\"/JkQTv7Ft3FrcNFvRk6zR7tdVHQ=\"")
                                                      .SetDescription("Prints 'Hello world!'")
                                                      .SetResponseResourceExample("Hello world!")
                                                      .SetHeader("Accept-Language", "en-US")
                                                      .SetHeader("Date", "Thu, 31 Dec 2020 12:00:00 GMT");
        }
    }
}
