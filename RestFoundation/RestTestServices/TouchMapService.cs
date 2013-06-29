using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using RestFoundation;
using RestFoundation.Results;
using RestFoundation.Runtime;
using RestTestContracts;
using RestTestContracts.Resources;
using RestTestServices.Utilities;

namespace RestTestServices
{
    public class TouchMapService : ITouchMapService
    {
        private static readonly Dictionary<string, string> environments = new Dictionary<string, string>
        {
            { "Default", "smartcaredev" },
            { "Development", "smartcaredev" },
            { "Stage", "smartcarestage" },
            { "Production", "smartcare" }
        }; // web.config mocking

        public IServiceContext Context { get; set; }

        public object Get()
        {
            var sessionInfo = (SessionInfo) Context.Request.ResourceBag.SessionInfo;

            string filePath = Context.MapPath(String.Format(@"~\App_Data\{0}\{1}\{2}\TouchMap.xml",
                                              sessionInfo.CustomerId,
                                              sessionInfo.ApplicationId,
                                              sessionInfo.Culture));

            var file = new FileInfo(filePath);

            if (!file.Exists)
            {
                return Result.ResponseStatus(HttpStatusCode.NotFound, "Touchmap not found");
            }

            string etag = Context.Response.GenerateEtag(file);

            if (etag == Context.Request.Headers.TryGet("If-None-Match"))
            {
                return new StatusCodeResult(HttpStatusCode.NotModified);
            }

            Context.Response.SetHeader(Context.Response.HeaderNames.ETag, etag);

            using (FileStream fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new StreamReader(fileStream);
                string xml = ModifyForEnvironment(reader.ReadToEnd(), sessionInfo.Environment);

                var xmlReader = new SafeXmlTextReader(xml, XmlNodeType.Document, null)
                {
                    Normalization = true
                };

                var serializer = new XmlSerializer(typeof(TouchMap));
                return serializer.Deserialize(xmlReader);
            }
        }

        private static string ModifyForEnvironment(string xml, string environment)
        {
            const string ProtocolPrefix = "://";
            const string HostSeparatorPattern = @"([\.|\-])";
            const string HostSeparatorReplacement = "$1";

            string environmentName;

            if (!environments.TryGetValue(environment, out environmentName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid environment provided");
            }

            return Regex.Replace(xml,
                                 String.Concat(ProtocolPrefix, environments["Default"], HostSeparatorPattern),
                                 String.Concat(ProtocolPrefix, environmentName, HostSeparatorReplacement),
                                 RegexOptions.IgnoreCase);
        }
    }
}
