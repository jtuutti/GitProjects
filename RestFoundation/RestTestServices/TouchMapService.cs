using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using RestFoundation;
using RestFoundation.Behaviors;
using RestTestContracts;
using RestTestContracts.Resources;
using RestTestServices.Utilities;

namespace RestTestServices
{
    public class TouchMapService : ServiceSecurityBehavior, ITouchMapService
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
            var sessionInfo = (SessionInfo) Context.ItemBag.SessionInfo;

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
                return Result.SetStatus(HttpStatusCode.NotFound, "Touchmap not found");
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

        public override bool OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            var sessionInfo = new SessionInfo(context.Request.Headers.TryGet("X-SpeechCycle-SmartCare-ApplicationID"),
                                              context.Request.Headers.TryGet("X-SpeechCycle-SmartCare-CustomerID"),
                                              context.Request.Headers.TryGet("X-SpeechCycle-SmartCare-SessionID"),
                                              context.Request.Headers.TryGet("X-SpeechCycle-SmartCare-CultureCode"),
                                              context.Request.Headers.TryGet("X-SpeechCycle-SmartCare-Environment"));

            if (String.IsNullOrEmpty(sessionInfo.ApplicationId) ||
                String.IsNullOrEmpty(sessionInfo.CustomerId) ||
                String.IsNullOrEmpty(sessionInfo.Environment) ||
                sessionInfo.SessionId == Guid.Empty)
            {
                SetForbiddenErrorMessage("No valid session context found");
                return false;
            }

            context.ItemBag.SessionInfo = sessionInfo;
            return true;
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
