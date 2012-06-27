using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;
using Moq;
using RestFoundation.Collections.Concrete;
using RestFoundation.Collections.Specialized;
using RestFoundation.Runtime;

namespace RestFoundation.Tests
{
    public static class Mocks
    {
        private readonly static DynamicDictionary itemBag = new DynamicDictionary();
        private readonly static NameValueCollection headers = new NameValueCollection();
        private readonly static HttpCookieCollection cookies = new HttpCookieCollection();

        public static IServiceContext ServiceContext()
        {
            var mock = new Mock<IServiceContext>();
            mock.SetupGet(x => x.IsAuthenticated).Returns(true);
            mock.SetupGet(x => x.ItemBag).Returns(itemBag);
            mock.SetupProperty(x => x.User, new GenericPrincipal(new GenericIdentity("Tester"), new[] { "Test Runner" }));

            return mock.Object;
        }

        public static IHttpRequest HttpRequest()
        {
            var mock = new Mock<IHttpRequest>();
            mock.SetupGet(x => x.IsAjax).Returns(false);
            mock.SetupGet(x => x.IsLocal).Returns(true);
            mock.SetupGet(x => x.IsSecure).Returns(false);
            mock.SetupGet(x => x.Url).Returns(new ServiceUri("http://localhost", "test"));
            mock.SetupGet(x => x.Method).Returns(HttpMethod.Get);
            mock.SetupGet(x => x.Body).Returns(new MemoryStream());
            mock.SetupGet(x => x.Credentials).Returns(new NetworkCredential("Tester", "t3st123"));
            mock.SetupGet(x => x.QueryBag).Returns(new DynamicStringCollection());
            mock.SetupGet(x => x.RouteValues).Returns(new ObjectValueCollection());
            mock.SetupGet(x => x.Headers).Returns(new HeaderCollection());
            mock.SetupGet(x => x.QueryString).Returns(new StringValueCollection());
            mock.SetupGet(x => x.ServerVariables).Returns(new StringValueCollection());
            mock.SetupGet(x => x.Cookies).Returns(new CookieValueCollection());

            return mock.Object;
        }

        public static IHttpResponse HttpResponse()
        {
            var outputMock = new Mock<IHttpResponseOutput>();
            outputMock.SetupGet(x => x.Stream).Returns(Console.OpenStandardInput);
            outputMock.SetupGet(x => x.Writer).Returns(Console.Out);
            outputMock.SetupGet(x => x.Filter).Returns(new MemoryStream());
            outputMock.Setup(x => x.Flush()).Callback(() => outputMock.Object.Writer.Flush());
            outputMock.Setup(x => x.Clear()).Callback(Console.Clear);
            outputMock.Setup(x => x.Write(It.IsAny<object>())).Returns(outputMock.Object).Callback<object>(Console.Write);
            outputMock.Setup(x => x.Write(It.IsAny<string>())).Returns(outputMock.Object).Callback<string>(Console.Write);
            outputMock.Setup(x => x.WriteLine()).Returns(outputMock.Object).Callback(Console.WriteLine);
            outputMock.Setup(x => x.WriteLine(It.IsAny<string>())).Returns(outputMock.Object).Callback<string>(Console.WriteLine);
            outputMock.Setup(x => x.WriteLine(It.IsAny<byte>())).Returns(outputMock.Object).Callback<byte>(n =>
                                                                                                           {
                                                                                                               for (byte i = 0; i < n; i++)
                                                                                                               {
                                                                                                                   Console.WriteLine();
                                                                                                               }
                                                                                                           });
            outputMock.Setup(x => x.WriteFormat(It.IsAny<string>(), It.IsAny<object[]>())).Returns(outputMock.Object).Callback<string, object[]>(Console.Write);
            outputMock.Setup(x => x.WriteFormat(It.IsAny<IFormatProvider>(), It.IsAny<string>(), It.IsAny<object[]>()))
                                                  .Returns(outputMock.Object)
                                                  .Callback<IFormatProvider, string, object[]>((c, s, args) => Console.Write(s, args));

            var mock = new Mock<IHttpResponse>();
            mock.SetupGet(x => x.Output).Returns(outputMock.Object);
            mock.SetupProperty(x => x.TrySkipIisCustomErrors, false);

            mock.Setup(x => x.GetHeader(It.IsAny<string>())).Returns<string>(s => headers.Get(s));
            mock.Setup(x => x.SetHeader(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((n, v) => headers.Set(n, v));
            mock.Setup(x => x.RemoveHeader(It.IsAny<string>())).Returns<string>(s =>
                                                                                {
                                                                                    if (Array.IndexOf(headers.AllKeys, s) < 0)
                                                                                    {
                                                                                        return false;
                                                                                    }

                                                                                    headers.Remove(s);
                                                                                    return true;
                                                                                });
            mock.Setup(x => x.ClearHeaders()).Callback(() => headers.Clear());

            mock.Setup(x => x.SetCharsetEncoding(It.IsAny<Encoding>())).Callback<Encoding>(e =>
                                                                                           {
                                                                                               Console.OutputEncoding = e;
                                                                                           });
            mock.Setup(x => x.GetStatusCode()).Returns(HttpStatusCode.OK);
            mock.Setup(x => x.GetStatusDescription()).Returns("OK");
            mock.Setup(x => x.SetStatus(It.IsAny<HttpStatusCode>()));
            mock.Setup(x => x.SetStatus(It.IsAny<HttpStatusCode>(), It.IsAny<string>()));
            
            mock.Setup(x => x.GetCookie(It.IsAny<string>())).Returns<string>(cookies.Get);
            mock.Setup(x => x.SetCookie(It.IsAny<HttpCookie>())).Callback<HttpCookie>(cookies.Add);
            mock.Setup(x => x.ExpireCookie(It.IsAny<HttpCookie>())).Callback<HttpCookie>(c => cookies.Remove(c.Name));

            mock.Setup(x => x.Redirect(It.IsAny<string>()));
            mock.Setup(x => x.Redirect(It.IsAny<string>(), It.IsAny<bool>()));
            mock.Setup(x => x.Redirect(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()));
            mock.Setup(x => x.MapPath(It.IsAny<string>())).Returns<string>(s =>
                                                                           {
                                                                               if (s == null)
                                                                               {
                                                                                   return null;
                                                                               }

                                                                               return s.ToLowerInvariant().Replace("http://localhost/test", Environment.CurrentDirectory)
                                                                                   .Replace("/", @"\")
                                                                                   .TrimStart('~', '\\');
                                                                           });
            mock.Setup(x => x.SetFileDependencies(It.IsAny<string>()));
            mock.Setup(x => x.SetFileDependencies(It.IsAny<string>(), It.IsAny<HttpCacheability>()));
            mock.Setup(x => x.TransmitFile(It.IsAny<string>())).Callback<string>(s =>
                                                                                 {
                                                                                     using (var reader = new StreamReader(s, Encoding.UTF8))
                                                                                     {
                                                                                         string line;
                                                                                         while ((line = reader.ReadLine()) != null)
                                                                                         {
                                                                                             Console.WriteLine(line);
                                                                                         }
                                                                                     }
                                                                                 });

            return mock.Object;
        }
    }
}
