using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;
using SampleRestService.Contracts;

namespace RestFoundation.Tests.XmlDoc
{
    [TestFixture]
    public class XmlDocParserTests
    {
        private sealed class MethodXmlDoc
        {
            public MethodXmlDoc()
            {
                Parameters = new SortedDictionary<string, string>();
            }

            public string Summary { get; set; }

            public string Returns { get; set; }

            public IDictionary<string, string> Parameters { get; private set; }
        }

        [Test]
        public void Parse_Test()
        {
            var doc = XDocument.Load(@"D:\Development\GitProjects\RestFoundation\SampleRestService\bin\SampleRestService.XML");

            Type type = typeof(ISampleService);

            var methodDocs = new List<MethodXmlDoc>();

            foreach (MethodInfo method in type.GetMethods())
            {
                string name = "M:" + type.FullName + "." + method.Name;

                ParameterInfo[] parameters = method.GetParameters();

                if (parameters.Length > 0)
                {
                    name += "(";

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        name += parameters[i].ParameterType.FullName;

                        if (i < parameters.Length - 1)
                        {
                            name += ",";
                        }
                    }

                    name += ")";
                }

                XElement methodDoc = doc.Root
                                        .Elements("members")
                                        .Elements("member")
                                        .FirstOrDefault(x => x.Attribute("name") != null && x.Attribute("name").Value == name);

                if (methodDoc != null)
                {
                    XElement summaryDoc = methodDoc.Element("summary");
                    XElement returnsDoc = methodDoc.Element("returns");

                    var documentObject = new MethodXmlDoc
                    {
                        Summary = summaryDoc != null ? summaryDoc.Value.Trim() : String.Empty,
                        Returns = returnsDoc != null ? returnsDoc.Value.Trim() : String.Empty,
                    };

                    foreach (ParameterInfo parameter in parameters)
                    {
                        var parameterDoc = methodDoc.Elements("param")
                                                    .FirstOrDefault(x => x.Attribute("name") != null  && x.Attribute("name").Value == parameter.Name);

                        if (parameterDoc != null)
                        {
                            documentObject.Parameters[parameter.Name] = parameterDoc.Value.Trim();
                        }
                    }

                    methodDocs.Add(documentObject);
                }
            }
        }
    }
}
