using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using RestFoundation;
using RestFoundation.Behaviors;
using RestTestContracts;
using RestTestContracts.Resources;
using RestTestServices.Behaviors;
using RestTestServices.Utilities;

namespace RestTestServices
{
    public class TouchMapService : ITouchMapService, IRestService
    {
        private static readonly Dictionary<string, string> environments = new Dictionary<string, string>
        {
            { "Default", "smartcaredev" },
            { "Development", "smartcaredev" },
            { "Stage", "smartcarestage" },
            { "Production", "smartcare" }
        }; // web.config mocking

        public IServiceContext Context { get; set; }

        public IServiceExceptionHandler ExceptionHandler
        {
            get
            {
                return null;
            }
        }

        public IEnumerable<IServiceBehavior> Behaviors
        {
            get
            {
                yield return new T3ContextBehavior();
            }
        }

        public object Get()
        {
            var sessionInfo = (SessionInfo) Context.HttpItemBag.SessionInfo;

            string relativePath = Context.MapPath(String.Format(@"~\App_Data\{0}\{1}\{2}\TouchMap.xml",
                                                  sessionInfo.CustomerId,
                                                  sessionInfo.ApplicationId,
                                                  sessionInfo.Culture));

            if (File.Exists(relativePath))
            {
                Context.Response.SetFileDependencies(relativePath);
            }
            else
            {
                return Result.ResponseStatus(HttpStatusCode.NotFound, "Touchmap not found");
            }

            using (var fileStream = new FileStream(relativePath, FileMode.Open, FileAccess.Read))
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

            return Regex.Replace(xml,
                                 String.Concat(ProtocolPrefix, environments["Default"], HostSeparatorPattern),
                                 String.Concat(ProtocolPrefix, environments[environment], HostSeparatorReplacement),
                                 RegexOptions.IgnoreCase);
        }
    }
}
